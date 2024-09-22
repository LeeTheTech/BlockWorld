using System.Collections.Generic;
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
    meshCollider.sharedMesh = mesh;
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

    byte[,,] lightMap = new byte[48, 256, 48];

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

    Queue<Vector3Int> simulateQueue = new Queue<Vector3Int>();
    //sunray tracing needs to start above the highest non-air block to increase performance
    //all blocks above that block need to be set to 15
    for (int z = 0; z < 48; ++z){
      for (int x = 0; x < 48; ++x){
        if ((x % 47) * (z % 47) == 0) //filters outer edges
        {
          //Debug.Log($"these should at least 0 or 47  ->  {x} {z}"); 
          for (int yy = 0; yy < 256; ++yy) //dont do outer edges
          {
            lightMap[x, yy, z] = 15; //set all edges to 15 to stop tracing at edges
          }
          continue;
        }

        int y = GetHighestNonAir(chunkMap, x, z);
        if (x < 46) y = Mathf.Max(y, GetHighestNonAir(chunkMap, x + 1, z));
        if (x > 1) y = Mathf.Max(y, GetHighestNonAir(chunkMap, x - 1, z));
        if (z < 46) y = Mathf.Max(y, GetHighestNonAir(chunkMap, x, z + 1));
        if (z > 1) y = Mathf.Max(y, GetHighestNonAir(chunkMap, x, z - 1));
        y = Mathf.Min(y + 1, 255);
        simulateQueue.Enqueue(new Vector3Int(x, y, z));

        while (y < 255){
          lightMap[x, y, z] = 15;
          y++;
        }
      }
    }

    for (int y = 0; y < 3; ++y){
      for (int x = 0; x < 3; ++x){
        foreach (KeyValuePair<Vector3Int, byte> kv in chunkMap[x, y].lightSources){
          Vector3Int pos = kv.Key;
          int lX = (16 * x) + pos.x;
          int lY = pos.y;
          int lZ = (16 * y) + pos.z;
          lightMap[lX, lY, lZ] = kv.Value;
          simulateQueue.Enqueue(new Vector3Int(lX, lY, lZ));
        }
      }
    }

    while (simulateQueue.Count > 0){
      Vector3Int pos = simulateQueue.Dequeue();
      int x = pos.x;
      int y = pos.y;
      int z = pos.z;

      byte bR = (x == 47 ? BlockTypes.BEDROCK : GetBlockFromMap(chunkMap, x + 1, y, z));
      byte bL = (x == 0 ? BlockTypes.BEDROCK : GetBlockFromMap(chunkMap, x - 1, y, z));
      byte bF = (z == 47 ? BlockTypes.BEDROCK : GetBlockFromMap(chunkMap, x, y, z + 1));
      byte bB = (z == 0 ? BlockTypes.BEDROCK : GetBlockFromMap(chunkMap, x, y, z - 1));
      byte bU = (y == 255 ? BlockTypes.BEDROCK : GetBlockFromMap(chunkMap, x, y + 1, z));
      byte bD = (y == 0 ? BlockTypes.BEDROCK : GetBlockFromMap(chunkMap, x, y - 1, z));

      byte blockLight = lightMap[x, y, z];

      if (bR == BlockTypes.AIR){
        byte lightR = lightMap[x + 1, y, z];
        if (lightR < blockLight - 1){
          lightMap[x + 1, y, z] = (byte)(blockLight - 1);
          simulateQueue.Enqueue(new Vector3Int(x + 1, y, z));
        }
      }

      if (bL == BlockTypes.AIR){
        byte lightL = lightMap[x - 1, y, z];
        if (lightL < blockLight - 1){
          lightMap[x - 1, y, z] = (byte)(blockLight - 1);
          //if (x - 1 == 0) Debug.LogError("THIS SHOULD NOT HAPPEN");
          simulateQueue.Enqueue(new Vector3Int(x - 1, y, z));
        }
      }

      if (bD == BlockTypes.AIR){
        if (blockLight == 15){
          lightMap[x, y - 1, z] = blockLight;
          simulateQueue.Enqueue(new Vector3Int(x, y - 1, z));
        }
        else{
          byte lightD = lightMap[x, y - 1, z];
          if (lightD < blockLight - 1){
            lightMap[x, y - 1, z] = (byte)(blockLight - 1);
            simulateQueue.Enqueue(new Vector3Int(x, y - 1, z));
          }
        }
      }

      if (bU == BlockTypes.AIR){
        byte lightU = lightMap[x, y + 1, z];
        if (lightU < blockLight - 1){
          lightMap[x, y + 1, z] = (byte)(blockLight - 1);
          simulateQueue.Enqueue(new Vector3Int(x, y + 1, z));
        }
      }

      if (bF == BlockTypes.AIR){
        byte lightF = lightMap[x, y, z + 1];
        if (lightF < blockLight - 1){
          lightMap[x, y, z + 1] = (byte)(blockLight - 1);
          simulateQueue.Enqueue(new Vector3Int(x, y, z + 1));
        }
      }

      if (bB == BlockTypes.AIR){
        byte lightB = lightMap[x, y, z - 1];
        if (lightB < blockLight - 1){
          lightMap[x, y, z - 1] = (byte)(blockLight - 1);
          simulateQueue.Enqueue(new Vector3Int(x, y, z - 1));
        }
      }
    }
    //Debug.Log("Did " + simulateCount + " light simulations");

    UnityEngine.Profiling.Profiler.EndSample();


    UnityEngine.Profiling.Profiler.BeginSample("CREATING FACES");
    TextureMapper textureMapper = GameManager.instance.textureMapper;
    for (int z = 0; z < 16; ++z){
      for (int y = 0; y < 256; ++y){
        for (int x = 0; x < 16; ++x){
          byte blockType = chunkData.GetBlocks()[x, y, z];
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
          
          BlockShapes.AddFaces(new BlockShapes.ShapeData(position, textureMapper.map[blockType], lightMap, blockType, x, y, z, bR, bL, bF, bB, bU, bD, lx, ly, lz), GetCorrectMeshData(blockType));
        }
      }
    }

    UnityEngine.Profiling.Profiler.EndSample();
    UnityEngine.Profiling.Profiler.BeginSample("APPLYING MESH DATA");
    chunkMeshData.SetupMesh(mesh);
    gameObject.SetActive(true);
    meshCollider.sharedMesh = mesh;
    UnityEngine.Profiling.Profiler.EndSample();
  }

  private static byte GetHighestNonAir(ChunkData[,] chunkData, int x, int z){
    int cX = x < 16 ? 0 : (x < 32 ? 1 : 2);
    int cZ = z < 16 ? 0 : (z < 32 ? 1 : 2);
    return chunkData[cX, cZ].highestNonAirBlock[x - (cX * 16), z - (cZ * 16)];
  }

  private static byte GetBlockFromMap(ChunkData[,] chunkData, int x, int y, int z){
    try{
      int cX = x < 16 ? 0 : (x < 32 ? 1 : 2);
      int cZ = z < 16 ? 0 : (z < 32 ? 1 : 2);
      return chunkData[cX, cZ].GetBlocks()[x - (cX * 16), y, z - (cZ * 16)];
    }
    catch (System.Exception){
      Debug.LogWarning($"{x} {y} {z}");
      throw;
    }
  }
  
  private ChunkMeshData GetCorrectMeshData(byte block){
    switch (block){
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