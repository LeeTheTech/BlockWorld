using System.Collections.Generic;
using UnityEngine;

public static class StructureGenerator{
  
  public static List<ChunkData.StructureInfo> GenerateStructures(byte[,,] blocks, Vector2Int worldPos, System.Random rnd, ChunkData.StructureChance structureChance){
    List<ChunkData.StructureInfo> structures = new List<ChunkData.StructureInfo>();
    bool[,] spotsTaken = new bool[16, 16];
    GenerateCaveEntrances(structureChance, structures, blocks, rnd, spotsTaken);
    GenerateTrees(structureChance, structures, blocks, worldPos, rnd, spotsTaken);
    GenerateFoliage(structureChance, structures, blocks, worldPos, rnd, spotsTaken);
    GenerateWells(structureChance, structures, blocks, rnd, spotsTaken);
    return structures;
  }

  private static void GenerateCaveEntrances(ChunkData.StructureChance structureChance, List<ChunkData.StructureInfo> structures, byte[,,] blocks, System.Random rnd, bool[,] spotsTaken){
    if (rnd.Next() < structureChance.caveEntrance){
      int h = 255;
      while (h > 0) {
        if (blocks[8, h, 8] != BlockTypes.AIR) {
          structures.Add(new ChunkData.StructureInfo(new Vector3Int(0, h + 6, 0), Structure.Type.CAVE_ENTRANCE, rnd.Next()));
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
  }

  private static void GenerateTrees(ChunkData.StructureChance structureChance, List<ChunkData.StructureInfo> structures, byte[,,] blocks, Vector2Int worldPos, System.Random rnd, bool[,] spotsTaken){
    for (int y = 2; y < 14; ++y){
      for (int x = 2; x < 14; ++x){
        BiomeType biomeType = BiomeManager.GetBiomeType(worldPos.x + x, worldPos.y + y);
        switch (biomeType){
          case BiomeType.MOUNTAIN:
          case BiomeType.GRASSLAND:
          case BiomeType.SNOW:
            if (rnd.Next() < structureChance.tree){
              if (IsSpotFree(spotsTaken, new Vector2Int(x, y), 2)){
                spotsTaken[x, y] = true;
                int height = 255;
                while (height > 0){
                  if (blocks[x, height, y] != BlockTypes.AIR){
                    if (blocks[x, height, y] != BlockTypes.GRASS) break;
                    structures.Add(new ChunkData.StructureInfo(new Vector3Int(x, height + 1, y), Structure.Type.OAK_TREE, rnd.Next()));
                    break;
                  }
                  height--;
                }
              }
            }
            break;
    
          case BiomeType.DESERT:
            if (rnd.Next() <  structureChance.cactus){
              if (IsSpotFree(spotsTaken, new Vector2Int(x, y), 2)){
                spotsTaken[x, y] = true;
                int height = 255;
                while (height > 0){
                  if (blocks[x, height, y] != BlockTypes.AIR){
                    if (blocks[x, height, y] != BlockTypes.SAND) break;
                    structures.Add(new ChunkData.StructureInfo(new Vector3Int(x, height + 1, y), Structure.Type.CACTUS, rnd.Next()));
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
  }

  private static void GenerateFoliage(ChunkData.StructureChance structureChance, List<ChunkData.StructureInfo> structures, byte[,,] blocks, Vector2Int worldPos, System.Random rnd, bool[,] spotsTaken){
    for (int y = 2; y < 14; ++y){
      for (int x = 2; x < 14; ++x){
        BiomeType biomeType = BiomeManager.GetBiomeType(worldPos.x + x, worldPos.y + y);
        switch (biomeType){
          case BiomeType.MOUNTAIN:
          case BiomeType.GRASSLAND:
            if (rnd.Next() < structureChance.foliage){
              if (IsSpotFree(spotsTaken, new Vector2Int(x, y), 1)){
                spotsTaken[x, y] = true;
                int height = 255;
                while (height > 0){
                  if (blocks[x, height, y] != BlockTypes.AIR){
                    if (blocks[x, height, y] != BlockTypes.GRASS) break;
                    structures.Add(new ChunkData.StructureInfo(new Vector3Int(x, height + 1, y), Structure.Type.FOLIAGE, rnd.Next()));
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
  }

  private static void GenerateWells(ChunkData.StructureChance structureChance, List<ChunkData.StructureInfo> structures, byte[,,] blocks, System.Random rnd, bool[,] spotsTaken){
    if (rnd.Next() < structureChance.well){
      if (IsSpotFree(spotsTaken, new Vector2Int(7, 7), 3)){
        int minH = 255;
        int maxH = 0;
        bool canPlace = true;
        for (int y = 5; y < 11; ++y){
          for (int x = 5; x < 11; ++x){
            for (int h = 255; h > -1; h--){
              byte b = blocks[x, h, y];
              if (b != BlockTypes.AIR){
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
          for (int y = 5; y < 11; ++y){
            for (int x = 5; x < 11; ++x){
              spotsTaken[x, y] = true;
            }
          }
          int h = 255;
          while (h > 0){
            if (blocks[7, h, 7] != BlockTypes.AIR){
              structures.Add(new ChunkData.StructureInfo(new Vector3Int(7, h + 1, 7), Structure.Type.WELL, rnd.Next()));
              break;
            }
            h--;
          }
        }
      }
    }
  }

  private static bool IsSpotFree(bool[,] spotsTaken, Vector2Int pos, int size){
    bool spotTaken = false;
    for (int y = Mathf.Max(0, pos.y - size); y < Mathf.Min(15, pos.y + size + 1); ++y){
      for (int x = Mathf.Max(0, pos.x - size); x < Mathf.Min(15, pos.x + size + 1); ++x){
        spotTaken |= spotsTaken[x, y];
      }
    }
    return !spotTaken;
  }
}