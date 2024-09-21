using UnityEngine;
using System.Threading;
using System.Collections.Generic;

public class ChunkData{
  public Vector2Int position;
  private byte[,,] blocks, light;
  public bool terrainReady{ get; private set; }
  public bool startedLoadingDetails{ get; private set; }
  public bool chunkReady{ get; private set; }
  public bool isDirty;

  //devide by chance ( 1 in X )
  const int STRUCTURE_CHANCE_TREE = (int.MaxValue / 100);
  const int STRUCTURE_CHANCE_WELL = (int.MaxValue / 512);
  const int STRUCTURE_CHANCE_CAVE_ENTRANCE = (int.MaxValue / 50);
  const int STRUCTURE_CHANCE_CACTUS = (int.MaxValue / 100);

  private Thread loadTerrainThread;
  private Thread loadDetailsThread;

  public HashSet<Vector2Int> references;
  public List<StructureInfo> structures;
  public Dictionary<Vector3Int, byte> lightSources;
  public byte[,] highestNonAirBlock;
  public ChunkSaveData saveData;
  private ChunkData front, left, back, right; //neighbours (only exist while loading structures)

  public struct StructureInfo{
    public StructureInfo(Vector3Int position, Structure.Type type, int seed){
      this.position = position;
      this.type = type;
      this.seed = seed;
    }

    public Vector3Int position;
    public Structure.Type type;
    public int seed;
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

  public byte[,,] GetBlocks(){
    //if (!chunkReady) throw new System.Exception($"Chunk {position} has not finished loading");
    return blocks;
  }

  public byte[,,] GetLights(){
    //if (!chunkReady) throw new System.Exception($"Chunk {position} has not finished loading");
    return light;
  }

  public byte[,,] NewLights(){
    //if (!chunkReady) throw new System.Exception($"Chunk {position} has not finished loading");
    light = new byte[16, 256, 16];
    return light;
  }

  public void StartTerrainLoading(){
    //Debug.Log($"Chunk {position} start terrain loading");
    loadTerrainThread = new Thread(LoadTerrain){
        IsBackground = true
    };
    loadTerrainThread.Start();
  }

  public void StartDetailsLoading(ChunkData f, ChunkData l, ChunkData b, ChunkData r){
    //Debug.Log($"Chunk {position} start structure loading");
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
    light = new byte[16, 256, 16];
    Vector2Int worldPos = position * 16;

    // Terrain
    for (int z = 0; z < 16; ++z){
      for (int x = 0; x < 16; ++x){
        int noiseX = worldPos.x + x;
        int noiseZ = worldPos.y + z;

        float biomeNoise = BiomeManager.GenerateTerrainHeight(noiseX, noiseZ);
        int heightGen = Mathf.Clamp(Mathf.RoundToInt(BiomeManager.seaLevel + biomeNoise), 0, 256);

        for (int y = 0; y < 256; ++y){
          blocks[x, y, z] = BiomeManager.DetermineBlockType(heightGen, noiseX, y, noiseZ);
          if (BlockTypes.lightLevel[blocks[x, y, z]] > 0){
            lightSources[new Vector3Int(x, y, z)] = BlockTypes.lightLevel[blocks[x, y, z]];
          }
        }
      }
    }

    // Structures
    string hash = World.activeWorld.info.seed.ToString() + position.x.ToString() + position.y.ToString();
    int structuresSeed = hash.GetHashCode();
    //Debug.Log("Chunk structures seed is " + structuresSeed);
    System.Random rnd = new System.Random(structuresSeed);
    structures = new List<StructureInfo>();
    bool[,] spotsTaken = new bool[16, 16];

    // Cave Entrances
    if (rnd.Next() < STRUCTURE_CHANCE_CAVE_ENTRANCE) {
      int h = 255;
      while (h > 0) {
        if (blocks[8, h, 8] != BlockTypes.AIR) {
          structures.Add(new StructureInfo(new Vector3Int(0, h + 6, 0), Structure.Type.CAVE_ENTRANCE, rnd.Next()));
            
          // Mark the area above the cave entrance as taken (radius of 2)
          for (int z = 6; z <= 10; ++z) {
            for (int x = 6; x <= 10; ++x) {
              spotsTaken[x, z] = true;
            }
          }
          break;
        }
        h--;
      }
    }

    // Trees
    for (int y = 2; y < 14; ++y){
      for (int x = 2; x < 14; ++x){
        BiomeType biomeType = BiomeManager.GetBiomeType(worldPos.x + x, worldPos.y + y);
        switch (biomeType){
          case BiomeType.MOUNTAIN:
          case BiomeType.GRASSLAND:
          case BiomeType.SNOW:
            if (rnd.Next() < STRUCTURE_CHANCE_TREE){
              if (IsSpotFree(spotsTaken, new Vector2Int(x, y), 2)){
                spotsTaken[x, y] = true;
                int height = 255;
                while (height > 0){
                  if (blocks[x, height, y] != BlockTypes.AIR){
                    if (blocks[x, height, y] != BlockTypes.GRASS) break;
                    structures.Add(new StructureInfo(new Vector3Int(x, height + 1, y), Structure.Type.OAK_TREE, rnd.Next()));
                    break;
                  }
                  height--;
                }
              }
            }
            break;

          case BiomeType.DESERT:
            if (rnd.Next() < STRUCTURE_CHANCE_CACTUS){
              if (IsSpotFree(spotsTaken, new Vector2Int(x, y), 2)){
                spotsTaken[x, y] = true;
                int height = 255;
                while (height > 0){
                  if (blocks[x, height, y] != BlockTypes.AIR){
                    if (blocks[x, height, y] != BlockTypes.SAND) break;
                    structures.Add(new StructureInfo(new Vector3Int(x, height + 1, y), Structure.Type.CACTUS, rnd.Next()));
                    break;
                  }
                  height--;
                }
              }
            }
            break;
        }
      }
    }

    // Wells
    if (rnd.Next() < STRUCTURE_CHANCE_WELL){
      if (IsSpotFree(spotsTaken, new Vector2Int(7, 7), 3)){
        //Debug.Log("Spot is free");

        int minH = 255;
        int maxH = 0;
        bool canPlace = true;
        for (int y = 5; y < 11; ++y){
          for (int x = 5; x < 11; ++x){
            for (int h = 255; h > -1; h--){
              byte b = blocks[x, h, y];
              if (b != BlockTypes.AIR){
                //Debug.Log(b);
                canPlace &= (b == BlockTypes.GRASS);
                minH = Mathf.Min(minH, h);
                maxH = Mathf.Max(maxH, h);
                break;
              }
            }
          }
        }

        canPlace &= Mathf.Abs(minH - maxH) < 2;
        if (canPlace){
          //Debug.Log("spawning well structure");
          for (int y = 5; y < 11; ++y){
            for (int x = 5; x < 11; ++x){
              spotsTaken[x, y] = true;
            }
          }
          int h = 255;
          while (h > 0){
            if (blocks[7, h, 7] != BlockTypes.AIR){
              structures.Add(new StructureInfo(new Vector3Int(7, h + 1, 7), Structure.Type.WELL, rnd.Next()));
              break;
            }
            h--;
          }
        }
      }
    }


    //already load changes from disk here (apply later)
    saveData = SaveDataManager.instance.Load(position);

    terrainReady = true;
    //Debug.Log($"Chunk {position} terrain ready");
  }

  private bool IsSpotFree(bool[,] spotsTaken, Vector2Int pos, int size) //x area is for example size + 1 + size
  {
    bool spotTaken = false;
    for (int y = Mathf.Max(0, pos.y - size); y < Mathf.Min(15, pos.y + size + 1); ++y){
      for (int x = Mathf.Max(0, pos.x - size); x < Mathf.Min(15, pos.x + size + 1); ++x){
        spotTaken |= spotsTaken[x, y];
      }
    }

    return !spotTaken;
  }

  private void LoadDetails(){
    //load structures

    for (int i = 0; i < structures.Count; ++i){
      StructureInfo structure = structures[i];
      bool overwritesEverything = Structure.OverwritesEverything(structure.type);
      Vector3Int p = structure.position;
      int x = p.x;
      int y = p.y;
      int z = p.z;
      List<Structure.Change> changeList = Structure.Generate(structure.type, structure.seed);
      //Debug.Log($"placing {structure.type} wich has {changeList.Count} blocks");
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
      }
    }

    //remove all references to neighbors to avoid them staying in memory when unloading chunks
    front = null;
    left = null;
    right = null;
    back = null;

    //add sky light
    // for (int z = 0; z < 16; ++z)
    // {
    // 	for (int x = 0; x < 16; ++x)
    // 	{
    // 		byte ray = 15;
    // 		for (int y = 255; y > -1; --y)
    // 		{
    // 			byte block = blocks[x, y, z];
    // 			if (block == 0)
    // 			{
    // 				light[x, y, z] = ray;
    // 			}
    // 			else
    // 			{
    // 				if (block < 128) break;
    // 				light[x, y, z] = ray;
    // 			}
    // 		}
    // 	}
    // }
    //Debug.Log($"Chunk {position} structures ready");
    //load changes
    List<ChunkSaveData.C> changes = saveData.changes;
    for (int i = 0; i < changes.Count; ++i){
      ChunkSaveData.C c = changes[i];
      blocks[c.x, c.y, c.z] = c.b;
      byte lightLevel = BlockTypes.lightLevel[c.b];
      if (lightLevel > 0){
        lightSources[new Vector3Int(c.x, c.y, c.z)] = lightLevel;
      }
    }

    //get highest non-air blocks to speed up light simulation
    for (int z = 0; z < 16; ++z){
      for (int x = 0; x < 16; ++x){
        highestNonAirBlock[x, z] = 0;
        for (int y = 255; y > -1; --y){
          if (blocks[x, y, z] != BlockTypes.AIR){
            highestNonAirBlock[x, z] = (byte)y;
            break;
          }
        }
      }
    }

    chunkReady = true;
    //Debug.Log($"Chunk {position} ready");
  }

  public void Modify(int x, int y, int z, byte blockType){
    if (!chunkReady) throw new System.Exception("Chunk has not finished loading");
    Debug.Log($"Current highest block at {x}x{z} is {highestNonAirBlock[x, z]}");

    saveData.changes.Add(new ChunkSaveData.C((byte)x, (byte)y, (byte)z, blockType));
    blocks[x, y, z] = blockType;
    if (blockType == BlockTypes.AIR){
      if (highestNonAirBlock[x, z] == y){
        highestNonAirBlock[x, z] = 0;
        for (int yy = y; yy > -1; yy--){
          if (blocks[x, yy, z] != BlockTypes.AIR){
            highestNonAirBlock[x, z] = (byte)yy;
            break;
          }
        }
      }
    }
    else{
      highestNonAirBlock[x, z] = (byte)Mathf.Max(highestNonAirBlock[x, z], y);
    }

    Debug.Log($"New highest block at {x}x{z} is {highestNonAirBlock[x, z]}");
  }

  public void Unload(){
    if (isDirty){
      SaveDataManager.instance.Save(saveData);
    }
  }
}