using System.Collections.Generic;
using UnityEngine;

public static class ChunkLighting{
  
  public static byte[,,] SimulateBlockLighting(ChunkData[,] chunkMap){
    byte[,,] lightMap = new byte[48, 256, 48];
    Queue<Vector3Int> simulateQueue = new Queue<Vector3Int>();

    for (int z = 0; z < 48; ++z){
      for (int x = 0; x < 48; ++x){
        int y = GetHighestNonAir(chunkMap, x, z);
        y = AdjustHeightWithNeighbours(chunkMap, x, z, y);
        y = Mathf.Min(y + 1, 255);
        simulateQueue.Enqueue(new Vector3Int(x, y, z));
      }
    }

    ProcessBlockLightSources(chunkMap, lightMap, simulateQueue);

    while (simulateQueue.Count > 0){
      Vector3Int pos = simulateQueue.Dequeue();
      SpreadBlockLight(chunkMap, lightMap, simulateQueue, pos);
    }
    return lightMap;
  }

  private static void ProcessBlockLightSources(ChunkData[,] chunkMap, byte[,,] lightMap, Queue<Vector3Int> simulateQueue){
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
  }

  private static void SpreadBlockLight(ChunkData[,] chunkMap, byte[,,] lightMap, Queue<Vector3Int> simulateQueue, Vector3Int pos){
    int x = pos.x;
    int y = pos.y;
    int z = pos.z;

    byte blockLight = lightMap[x, y, z];

    // Spread block light
    TrySpreadBlockLight(lightMap, simulateQueue, x + 1, y, z, blockLight, (x == 47 ? BlockTypes.BEDROCK : GetBlockFromMap(chunkMap, x + 1, y, z)));
    TrySpreadBlockLight(lightMap, simulateQueue, x - 1, y, z, blockLight, (x == 0 ? BlockTypes.BEDROCK : GetBlockFromMap(chunkMap, x - 1, y, z)));
    TrySpreadBlockLight(lightMap, simulateQueue, x, y - 1, z, blockLight, (y == 0 ? BlockTypes.BEDROCK : GetBlockFromMap(chunkMap, x, y - 1, z)));
    TrySpreadBlockLight(lightMap, simulateQueue, x, y + 1, z, blockLight, (y == 255 ? BlockTypes.BEDROCK : GetBlockFromMap(chunkMap, x, y + 1, z)));
    TrySpreadBlockLight(lightMap, simulateQueue, x, y, z + 1, blockLight, (z == 47 ? BlockTypes.BEDROCK : GetBlockFromMap(chunkMap, x, y, z + 1)));
    TrySpreadBlockLight(lightMap, simulateQueue, x, y, z - 1, blockLight, (z == 0 ? BlockTypes.BEDROCK : GetBlockFromMap(chunkMap, x, y, z - 1)));
  }

  private static void TrySpreadBlockLight(byte[,,] lightMap, Queue<Vector3Int> simulateQueue, int x, int y, int z, byte blockLight, byte blockType){
    switch (blockType){
      case BlockTypes.POPPY:
      case BlockTypes.FOLIAGE:
      case BlockTypes.AIR:
        byte light = lightMap[x, y, z];
        if (light < blockLight - 1){
          lightMap[x, y, z] = (byte)(blockLight - 1); // Set the block light
          simulateQueue.Enqueue(new Vector3Int(x, y, z));
        }
        break;
      // case BlockTypes.LAVA:
      //   for (int newY = y + 1; newY < 5; newY++){
      //     if (lightMap[x, newY, z] < 12){
      //       lightMap[x, newY, z] = 12;
      //       simulateQueue.Enqueue(new Vector3Int(x, newY, z));
      //     }
      //   }
      //   break;
    }
  }

  public static byte[,,] SimulateSunLighting(ChunkData[,] chunkMap){
    byte[,,] lightMap = new byte[48, 256, 48];
    Queue<Vector3Int> simulateQueue = new Queue<Vector3Int>();

    // Start sunray tracing from the highest non-air block for performance
    for (int z = 0; z < 48; ++z){
      for (int x = 0; x < 48; ++x){
        if ((x % 47) * (z % 47) == 0){
          for (int yy = 0; yy < 256; ++yy){
            lightMap[x, yy, z] = 15; // Set all edges to 15 to stop tracing
          }
          continue;
        }

        int y = GetHighestNonAir(chunkMap, x, z);
        y = AdjustHeightWithNeighbours(chunkMap, x, z, y);
        y = Mathf.Min(y + 1, 255);
        simulateQueue.Enqueue(new Vector3Int(x, y, z));

        while (y < 255){
          lightMap[x, y, z] = 15;
          y++;
        }
      }
    }

    ProcessSunLightSources(chunkMap, lightMap, simulateQueue);

    while (simulateQueue.Count > 0){
      Vector3Int pos = simulateQueue.Dequeue();
      SpreadSunLight(chunkMap, lightMap, simulateQueue, pos);
    }

    return lightMap;
  }
  
  private static void ProcessSunLightSources(ChunkData[,] chunkMap, byte[,,] lightMap, Queue<Vector3Int> simulateQueue){
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
  }

  private static void SpreadSunLight(ChunkData[,] chunkMap, byte[,,] lightMap, Queue<Vector3Int> simulateQueue, Vector3Int pos){
    int x = pos.x;
    int y = pos.y;
    int z = pos.z;

    byte sunLight = lightMap[x, y, z];
    TrySpreadSunLight(lightMap, simulateQueue, x + 1, y, z, sunLight,
        (x == 47 ? BlockTypes.BEDROCK : GetBlockFromMap(chunkMap, x + 1, y, z)));
    TrySpreadSunLight( lightMap, simulateQueue, x - 1, y, z, sunLight,
        (x == 0 ? BlockTypes.BEDROCK : GetBlockFromMap(chunkMap, x - 1, y, z)));
    TrySpreadSunLight(lightMap, simulateQueue, x, y - 1, z, sunLight,
        (y == 0 ? BlockTypes.BEDROCK : GetBlockFromMap(chunkMap, x, y - 1, z)));
    TrySpreadSunLight(lightMap, simulateQueue, x, y + 1, z, sunLight,
        (y == 255 ? BlockTypes.BEDROCK : GetBlockFromMap(chunkMap, x, y + 1, z)));
    TrySpreadSunLight(lightMap, simulateQueue, x, y, z + 1, sunLight,
        (z == 47 ? BlockTypes.BEDROCK : GetBlockFromMap(chunkMap, x, y, z + 1)));
    TrySpreadSunLight(lightMap, simulateQueue, x, y, z - 1, sunLight,
        (z == 0 ? BlockTypes.BEDROCK : GetBlockFromMap(chunkMap, x, y, z - 1)));
  }

  private static void TrySpreadSunLight(byte[,,] lightMap, Queue<Vector3Int> simulateQueue, int x, int y, int z, byte sunLight, byte blockType){
    switch (blockType){
      case BlockTypes.WATER:
      case BlockTypes.FOLIAGE:
      case BlockTypes.POPPY:
      case BlockTypes.AIR:
      case BlockTypes.GLASS:
        byte light = lightMap[x, y, z];
        if (light < sunLight - 1){
          lightMap[x, y, z] = (byte)(sunLight - 1);
          simulateQueue.Enqueue(new Vector3Int(x, y, z));
        }
        break;
      // case BlockTypes.LAVA:
      //   for (int newY = y + 1; newY < 5; newY++){
      //     if (lightMap[x, newY, z] < 12){
      //       lightMap[x, newY, z] = 12;
      //       simulateQueue.Enqueue(new Vector3Int(x, newY, z));
      //     }
      //   }
      //   break;
    }
  }

  private static int AdjustHeightWithNeighbours(ChunkData[,] chunkMap, int x, int z, int y){
    if (x < 46) y = Mathf.Max(y, GetHighestNonAir(chunkMap, x + 1, z));
    if (x > 1) y = Mathf.Max(y, GetHighestNonAir(chunkMap, x - 1, z));
    if (z < 46) y = Mathf.Max(y, GetHighestNonAir(chunkMap, x, z + 1));
    if (z > 1) y = Mathf.Max(y, GetHighestNonAir(chunkMap, x, z - 1));
    return y;
  }

  private static byte GetHighestNonAir(ChunkData[,] chunkData, int x, int z){
    int cX = x < 16 ? 0 : (x < 32 ? 1 : 2);
    int cZ = z < 16 ? 0 : (z < 32 ? 1 : 2);
    return chunkData[cX, cZ].highestNonAirBlock[x - (cX * 16), z - (cZ * 16)];
  }

  private static byte GetBlockFromMap(ChunkData[,] chunkData, int x, int y, int z){
    int cX = x < 16 ? 0 : (x < 32 ? 1 : 2);
    int cZ = z < 16 ? 0 : (z < 32 ? 1 : 2);
    return chunkData[cX, cZ].GetBlocks()[x - (cX * 16), y, z - (cZ * 16)];
  }

  public static bool IsHighestSunBlock(byte blockType){
    switch (blockType){
      case BlockTypes.LAVA:
      case BlockTypes.GLOWSTONE:
      case BlockTypes.FOLIAGE:
      case BlockTypes.POPPY:
      case BlockTypes.AIR:
        return false;
      default:
        return true;
    }
  }
}