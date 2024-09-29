using UnityEngine;

public class Chunk : MonoBehaviour{
  public MeshFilter meshFilter;
  public MeshCollider meshCollider;
  public Vector2Int position;
  private Mesh mesh;
  private ChunkMeshData chunkMeshData;

  //for generating
  private ChunkData[,] chunkMap;
  private readonly Vector2Int nFront = new(0, 1);
  private readonly Vector2Int nBack = new(0, -1);
  private readonly Vector2Int nLeft = new(-1, 0);
  private readonly Vector2Int nRight = new(1, 0);

  public void Awake(){
    mesh = new Mesh();
    meshFilter.sharedMesh = mesh;
    mesh.name = "ChunkMesh";
    mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
    mesh.MarkDynamic();
    chunkMeshData = new ChunkMeshData();
    chunkMap = new ChunkData[3, 3]; //start at backleft
  }

  public void Initialize(Vector2Int chunkPos){
    this.position = chunkPos;
  }

  public void Build(ChunkDataManager chunkDataManager){
    UnityEngine.Profiling.Profiler.BeginSample("BUILDING CHUNK");
    Vector2Int renderPosition = 16 * position;
    transform.position = new Vector3(renderPosition.x, 0, renderPosition.y);
    mesh.Clear();

    UnityEngine.Profiling.Profiler.BeginSample("GRABBING BLOCK DATA");

    ChunkData chunkData = chunkDataManager.data[position];
    ChunkData front = chunkDataManager.data[position + nFront];
    ChunkData back = chunkDataManager.data[position + nBack];
    ChunkData left = chunkDataManager.data[position + nLeft];
    ChunkData right = chunkDataManager.data[position + nRight];
    ChunkData frontLeft = chunkDataManager.data[position + nFront + nLeft];
    ChunkData frontRight = chunkDataManager.data[position + nFront + nRight];
    ChunkData backLeft = chunkDataManager.data[position + nBack + nLeft];
    ChunkData backRight = chunkDataManager.data[position + nBack + nRight];

    chunkMap[0, 0] = backLeft;
    chunkMap[1, 0] = back;
    chunkMap[2, 0] = backRight;
    chunkMap[0, 1] = left;
    chunkMap[1, 1] = chunkData;
    chunkMap[2, 1] = right;
    chunkMap[0, 2] = frontLeft;
    chunkMap[1, 2] = front;
    chunkMap[2, 2] = frontRight;

    UnityEngine.Profiling.Profiler.EndSample();

    UnityEngine.Profiling.Profiler.BeginSample("SIMULATING LIGHT");
    
    byte[,,] blockLightMap = ChunkLighting.SimulateBlockLighting(chunkMap);
    byte[,,] sunLightMap = ChunkLighting.SimulateSunLighting(chunkMap);
    
    UnityEngine.Profiling.Profiler.EndSample();

    UnityEngine.Profiling.Profiler.BeginSample("CREATING FACES");
    TextureMapper textureMapper = GameManager.instance.textureMapper;
    for (int z = 0; z < 16; ++z){
      for (int y = 0; y < 256; ++y){
        for (int x = 0; x < 16; ++x){
          byte blockType = chunkData.GetBlocks()[x, y, z];
          byte blockState = chunkData.GetBlockStates()[x, y, z];
          if (blockType == BlockTypes.AIR) continue;
          int lx = x + 16;
          int ly = y;
          int lz = z + 16;

          byte bR = x == 15 ? right.GetBlocks()[0, y, z] : chunkData.GetBlocks()[x + 1, y, z];
          byte bL = x == 0 ? left.GetBlocks()[15, y, z] : chunkData.GetBlocks()[x - 1, y, z];
          byte bF = z == 15 ? front.GetBlocks()[x, y, 0] : chunkData.GetBlocks()[x, y, z + 1];
          byte bB = z == 0 ? back.GetBlocks()[x, y, 15] : chunkData.GetBlocks()[x, y, z - 1];
          byte bU = y == 255 ? BlockTypes.AIR : chunkData.GetBlocks()[x, y + 1, z];
          byte bD = y == 0 ? BlockTypes.AIR : chunkData.GetBlocks()[x, y - 1, z];
          
          BlockShapes.AddFaces(new BlockShapes.ShapeData(position, textureMapper.map[blockType], blockLightMap, sunLightMap, blockState, blockType, x, y, z, bR, bL, bF, bB, bU, bD, lx, ly, lz), GetCorrectMeshData(blockType));
        }
      }
    }

    UnityEngine.Profiling.Profiler.EndSample();
    UnityEngine.Profiling.Profiler.BeginSample("APPLYING MESH DATA");
    
    Mesh colliderMesh = new Mesh{
        name = "ChunkColliderMesh",
        indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
    };
    
    chunkMeshData.SetupColliderMesh(colliderMesh);
    chunkMeshData.SetupMesh(mesh);
    chunkMeshData.ClearCachedMeshData();
    gameObject.SetActive(true);
    meshCollider.sharedMesh = colliderMesh;
    UnityEngine.Profiling.Profiler.EndSample();
  }


  public void Tick(ChunkDataManager chunkDataManager){
    ChunkData chunkData = chunkDataManager.data[position];
    byte[,,] blocks = chunkData.GetBlocks();
    byte[,,] blockStates = chunkData.GetBlockStates();

    bool rebuild = false;
    for (int z = 0; z < 16; ++z){
      for (int y = 0; y < 256; ++y){
        for (int x = 0; x < 16; ++x){
          byte block = blocks[x, y, z];
          if (block == BlockTypes.AIR) continue;
          byte state = blockStates[x, y, z];
          if (BlockTypes.IsLiquid(block) && BlockStateUtil.IsTicking(state)){
            rebuild = true;
            LiquidUtil.HandleLiquidTick(chunkDataManager, position, block, state, x, y, z, blocks, blockStates);
          } 
          else if (BlockTypes.IsExplosive(block) && BlockStateUtil.IsTicking(state)){
            rebuild = true;
            ExplosiveUtil.HandleExplosiveTick(chunkDataManager, position, block, state, x, y, z, blocks, blockStates);
          } 
          // else if (BlockStateUtil.ShouldBreak(state)){
          //   rebuild = true;
          //   blocks[x, y, z] = BlockTypes.AIR;
          //   blockStates[x, y, z] = 0;
          // }
        }
      }
    }
    if (rebuild) Build(chunkDataManager);
  }
  
  private ChunkMeshData GetCorrectMeshData(byte block){
    switch (block){
      case BlockTypes.POPPY:
      case BlockTypes.FOLIAGE:
        return chunkMeshData.noCullMeshData;
      case BlockTypes.ICE:
      case BlockTypes.WATER:
        return chunkMeshData.transparentMeshData;
      default:
        return chunkMeshData;
    }
  }

  public void Unload(){
    mesh.Clear();
    gameObject.SetActive(false);
  }
}