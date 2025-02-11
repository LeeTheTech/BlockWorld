﻿using UnityEngine;
using System.Threading;
using System.Collections.Generic;

public class ChunkData{
  public Vector2Int position;
  private byte[,,] blocks, blockStates;
  public bool terrainReady{ get; private set; }
  public bool startedLoadingDetails{ get; private set; }
  public bool chunkReady{ get; private set; }
  public bool isDirty;
  
  private Thread loadTerrainThread;
  private Thread loadDetailsThread;

  public HashSet<Vector2Int> references;
  public List<StructureInfo> structures;
  private readonly StructureChance structureChance = new((int.MaxValue / 100), (int.MaxValue / 512), (int.MaxValue / 50), (int.MaxValue / 100), (int.MaxValue / 20));
  public Dictionary<Vector3Int, byte> lightSources;
  public byte[,] highestNonAirBlock;
  public ChunkSaveData saveData;
  private ChunkData front, left, back, right;

  public struct StructureInfo{
    public Vector3Int position;
    public Structure.Type type;
    public int seed;
    
    public StructureInfo(Vector3Int position, Structure.Type type, int seed){
      this.position = position;
      this.type = type;
      this.seed = seed;
    }
  }
  
  public struct StructureChance{
    public int tree;
    public int well;
    public int caveEntrance;
    public int cactus;
    public int foliage;

    public StructureChance(int treeChance, int wellChance, int caveEntranceChance, int cactusChance, int foliageChance){
      tree = treeChance;
      well = wellChance;
      caveEntrance = caveEntranceChance;
      cactus = cactusChance;
      foliage = foliageChance;
    }
  }

  public ChunkData(Vector2Int position){
    this.position = position;
    terrainReady = false;
    startedLoadingDetails = false;
    chunkReady = false;
    isDirty = false;
    references = new HashSet<Vector2Int>();
    lightSources = new Dictionary<Vector3Int, byte>();
    highestNonAirBlock = new byte[16, 16];
  }
  
  public byte[,,] GetBlockStates(){
    return blockStates;
  }

  public byte[,,] GetBlocks(){
    return blocks;
  }

  public void StartTerrainLoading(){
    loadTerrainThread = new Thread(LoadTerrain){
        IsBackground = true
    };
    loadTerrainThread.Start();
  }

  public void StartDetailsLoading(ChunkData f, ChunkData l, ChunkData b, ChunkData r){
    //need to temporarily cache chunkdata of neighbors since generation is on another thread
    this.front = f;
    this.left = l;
    this.back = b;
    this.right = r;

    loadDetailsThread = new Thread(LoadDetails){
        IsBackground = true
    };
    loadDetailsThread.Start();
    startedLoadingDetails = true;
  }

  public void LoadTerrain(){
    blocks = new byte[16, 256, 16];
    blockStates = new byte[16, 256, 16];
    Vector2Int worldPos = position * 16;

    // Terrain
    for (int z = 0; z < 16; ++z){
      for (int x = 0; x < 16; ++x){
        int noiseX = worldPos.x + x;
        int noiseZ = worldPos.y + z;

        float biomeNoise = BiomeManager.GenerateTerrainHeight(noiseX, noiseZ);
        int heightGen = Mathf.Clamp(Mathf.RoundToInt(BiomeManager.seaLevel + biomeNoise), 0, 256);

        for (int y = 0; y < 256; ++y){
          byte blockType = BiomeManager.DetermineBlockType(heightGen, noiseX, y, noiseZ);
          blocks[x, y, z] = blockType;
          blockStates[x, y, z] = BlockStateUtil.CreateDefaultStateData();
          if (BlockTypes.lightLevel[blockType] > 0){
            lightSources[new Vector3Int(x, y, z)] = BlockTypes.lightLevel[blockType];
          }
        }
      }
    }

    // Structures
    string hash = World.activeWorld.info.seed.ToString() + position.x.ToString() + position.y.ToString();
    int structuresSeed = hash.GetHashCode();
    System.Random rnd = new System.Random(structuresSeed);
    structures = StructureGenerator.GenerateStructures(blocks, worldPos, rnd, structureChance);

    // Load changes from disk
    saveData = SaveDataManager.instance.Load(position);
    terrainReady = true;
  }

  private void LoadDetails(){
    for (int i = 0; i < structures.Count; ++i){
      StructureInfo structure = structures[i];
      bool overwritesEverything = Structure.OverwritesEverything(structure.type);
      Vector3Int p = structure.position;
      int x = p.x;
      int y = p.y;
      int z = p.z;
      List<Structure.Change> changeList = Structure.Generate(structure.type, structure.seed);
      for (int j = 0; j < changeList.Count; ++j){
        Structure.Change c = changeList[j];
        int placeX = x + c.x;
        int placeY = y + c.y;
        int placeZ = z + c.z;

        if (!overwritesEverything){
          //only place new blocks if density is higher or the same (leaves can't replace dirt for example)
          if (blocks[placeX, placeY, placeZ] < BlockTypes.density[c.b]) continue;
        }
        blocks[placeX, placeY, placeZ] = c.b;
        blockStates[placeX, placeY, placeZ] = c.bs;
      }
    }
    //remove all references to neighbors to avoid them staying in memory when unloading chunks
    front = null;
    left = null;
    right = null;
    back = null;
    
    List<ChunkSaveData.C> changes = saveData.changes;
    for (int i = 0; i < changes.Count; ++i){
      ChunkSaveData.C c = changes[i];
      blocks[c.x, c.y, c.z] = c.b;
      blockStates[c.x, c.y, c.z] = c.bs;
      
      Vector3Int lightSource = new Vector3Int(c.x, c.y, c.z);
      if (BlockTypes.lightLevel[c.b] > 0){
        lightSources[lightSource] = BlockTypes.lightLevel[c.b];
      }
      else if (lightSources.ContainsKey(lightSource)){
        lightSources.Remove(lightSource);
      }
    }

    //get highest non-air blocks to speed up light simulation
    for (int z = 0; z < 16; ++z){
      for (int x = 0; x < 16; ++x){
        highestNonAirBlock[x, z] = 0;
        for (int y = 255; y > -1; --y){
          if (ChunkLighting.IsHighestSunBlock(blocks[x, y, z])){
            highestNonAirBlock[x, z] = (byte)y;
            break;
          }
        }
      }
    }

    chunkReady = true;
  }

  public void Modify(int x, int y, int z, byte blockType, byte blockState){
    if (!chunkReady) throw new System.Exception("Chunk has not finished loading");
    
    saveData.changes.Add(new ChunkSaveData.C((byte)x, (byte)y, (byte)z, blockType, blockState));
    blocks[x, y, z] = blockType;
    blockStates[x, y, z] = blockState;
    
    if (!ChunkLighting.IsHighestSunBlock(blockType)){
      if (highestNonAirBlock[x, z] == y){
        highestNonAirBlock[x, z] = 0;
        for (int yy = y; yy > -1; yy--){
          if (ChunkLighting.IsHighestSunBlock(blocks[x, yy, z])){
            highestNonAirBlock[x, z] = (byte)yy;
            break;
          }
        }
      }
    }
    else{
      highestNonAirBlock[x, z] = (byte)Mathf.Max(highestNonAirBlock[x, z], y);
    }
  }

  public void Unload(){
    if (isDirty){
      SaveDataManager.instance.Save(saveData);
    }
  }
}