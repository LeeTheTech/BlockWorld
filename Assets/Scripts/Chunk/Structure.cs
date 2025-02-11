﻿using UnityEngine;
using System.Collections.Generic;

public static class Structure{
  public enum Type{
    OAK_TREE,
    WELL,
    CAVE_ENTRANCE,
    CACTUS,
    FOLIAGE
  }

  private static Dictionary<Type, List<Change>> templates;

  public struct Change{
    public int x, y, z;
    public byte b, bs;
    
    public Change(int x, int y, int z, byte b, byte bs){
      this.x = x;
      this.y = y;
      this.z = z;
      this.b = b;
      this.bs = bs;
    }
  }

  public static void Initialize(){
    templates = new Dictionary<Type, List<Change>>();
    foreach (Type type in System.Enum.GetValues(typeof(Type))){
      GenerateTemplate(type);
    }
  }

  public static bool OverwritesEverything(Type type){
    switch (type){
      case Type.FOLIAGE: return false;
      case Type.CACTUS: return false;
      case Type.OAK_TREE: return false;
      case Type.WELL: return true;
      case Type.CAVE_ENTRANCE: return true;
      default:
        return false;
    }
  }


  public static List<Change> Generate(Type type, int seed){
    System.Random rnd = new System.Random(seed);
    List<Change> template = templates[type];
    List<Change> result = template;
    switch (type){
      case Type.OAK_TREE:
        result = new List<Change>{ new(0, -1, 0, BlockTypes.DIRT, BlockStateUtil.CreateDefaultStateData()) };
        bool cutOff = rnd.Next(100) == 0;
        if (cutOff){
          result.Add(new Change(0, 0, 0, BlockTypes.LOG_OAK, BlockStateUtil.CreateDefaultStateData()));
          return result;
        }

        int height = (byte)rnd.Next(4, 7);
        bool superHigh = rnd.Next(100) == 0;
        if (superHigh) height = 10;

        for (int i = 0; i < height; ++i){
          result.Add(new Change(0, i, 0, BlockTypes.LOG_OAK, BlockStateUtil.CreateDefaultStateData()));
        }

        result.Add(new Change(0, height, 0, BlockTypes.LEAVES_OAK, BlockStateUtil.CreateDefaultStateData()));

        for (int i = 0; i < 4; ++i){
          result.Add(new Change(1, height - i, 0, BlockTypes.LEAVES_OAK, BlockStateUtil.CreateDefaultStateData()));
          result.Add(new Change(0, height - i, 1, BlockTypes.LEAVES_OAK, BlockStateUtil.CreateDefaultStateData()));
          result.Add(new Change(-1, height - i, 0, BlockTypes.LEAVES_OAK, BlockStateUtil.CreateDefaultStateData()));
          result.Add(new Change(0, height - i, -1, BlockTypes.LEAVES_OAK, BlockStateUtil.CreateDefaultStateData()));
        }


        if (rnd.Next(0, 2) == 0) result.Add(new Change(1, height - 1, 1, BlockTypes.LEAVES_OAK, BlockStateUtil.CreateDefaultStateData()));
        if (rnd.Next(0, 2) == 0) result.Add(new Change(-1, height - 1, 1, BlockTypes.LEAVES_OAK, BlockStateUtil.CreateDefaultStateData()));
        if (rnd.Next(0, 2) == 0) result.Add(new Change(1, height - 1, -1, BlockTypes.LEAVES_OAK, BlockStateUtil.CreateDefaultStateData()));
        if (rnd.Next(0, 2) == 0) result.Add(new Change(-1, height - 1, -1, BlockTypes.LEAVES_OAK, BlockStateUtil.CreateDefaultStateData()));


        for (int i = 2; i < 4; ++i){
          result.Add(new Change(2, height - i, -1, BlockTypes.LEAVES_OAK, BlockStateUtil.CreateDefaultStateData()));
          result.Add(new Change(2, height - i, 0, BlockTypes.LEAVES_OAK, BlockStateUtil.CreateDefaultStateData()));
          result.Add(new Change(2, height - i, 1, BlockTypes.LEAVES_OAK, BlockStateUtil.CreateDefaultStateData()));

          result.Add(new Change(-2, height - i, -1, BlockTypes.LEAVES_OAK, BlockStateUtil.CreateDefaultStateData()));
          result.Add(new Change(-2, height - i, 0, BlockTypes.LEAVES_OAK, BlockStateUtil.CreateDefaultStateData()));
          result.Add(new Change(-2, height - i, 1, BlockTypes.LEAVES_OAK, BlockStateUtil.CreateDefaultStateData()));

          result.Add(new Change(-1, height - i, 2, BlockTypes.LEAVES_OAK, BlockStateUtil.CreateDefaultStateData()));
          result.Add(new Change(0, height - i, 2, BlockTypes.LEAVES_OAK, BlockStateUtil.CreateDefaultStateData()));
          result.Add(new Change(1, height - i, 2, BlockTypes.LEAVES_OAK, BlockStateUtil.CreateDefaultStateData()));

          result.Add(new Change(-1, height - i, -2, BlockTypes.LEAVES_OAK, BlockStateUtil.CreateDefaultStateData()));
          result.Add(new Change(0, height - i, -2, BlockTypes.LEAVES_OAK, BlockStateUtil.CreateDefaultStateData()));
          result.Add(new Change(1, height - i, -2, BlockTypes.LEAVES_OAK, BlockStateUtil.CreateDefaultStateData()));

          result.Add(new Change(1, height - i, 1, BlockTypes.LEAVES_OAK, BlockStateUtil.CreateDefaultStateData()));
          result.Add(new Change(-1, height - i, 1, BlockTypes.LEAVES_OAK, BlockStateUtil.CreateDefaultStateData()));
          result.Add(new Change(1, height - i, -1, BlockTypes.LEAVES_OAK, BlockStateUtil.CreateDefaultStateData()));
          result.Add(new Change(-1, height - i, -1, BlockTypes.LEAVES_OAK, BlockStateUtil.CreateDefaultStateData()));

          if (rnd.Next(0, 2) == 0) result.Add(new Change(2, height - i, 2, BlockTypes.LEAVES_OAK, BlockStateUtil.CreateDefaultStateData()));
          if (rnd.Next(0, 2) == 0) result.Add(new Change(-2, height - i, 2, BlockTypes.LEAVES_OAK, BlockStateUtil.CreateDefaultStateData()));
          if (rnd.Next(0, 2) == 0) result.Add(new Change(2, height - i, -2, BlockTypes.LEAVES_OAK, BlockStateUtil.CreateDefaultStateData()));
          if (rnd.Next(0, 2) == 0) result.Add(new Change(-2, height - i, -2, BlockTypes.LEAVES_OAK, BlockStateUtil.CreateDefaultStateData()));
        }
        break;
      case Type.CAVE_ENTRANCE:
        result = new List<Change>();
        byte[,,] caveMap = new byte[16, 48, 16];
        Queue<Vector3Int> path = new Queue<Vector3Int>();
        int depth = rnd.Next(5, 11);
        for (int i = 0; i < depth; i++){
          path.Enqueue(new Vector3Int(
              rnd.Next(2, 13),
              44 - (i * 4),
              rnd.Next(2, 13)
          ));
        }

        Vector3Int nextPos = path.Dequeue();
        float d = 0;
        while (path.Count > 0){
          Vector3Int currentPos = nextPos;
          nextPos = path.Dequeue();
          float size = Mathf.Lerp(2, 0.75f, d / depth);

          for (int i = 0; i < 16; ++i){
            float lerpPos = i / 15f;
            Vector3 lerped = Vector3.Lerp(currentPos, nextPos, lerpPos);
            Vector3Int p = new Vector3Int((int)lerped.x, (int)lerped.y, (int)lerped.z);
            for (int z = -2; z < 3; ++z){
              for (int y = -2; y < 3; ++y){
                for (int x = -2; x < 3; ++x){
                  Vector3Int b = new Vector3Int(p.x + x, p.y + y, p.z + z);
                  if (Vector3Int.Distance(p, b) > size) continue;
                  if (b.x < 0 || b.x > 15) continue;
                  if (b.y < 0 || b.y > 47) continue;
                  if (b.z < 0 || b.z > 15) continue;

                  caveMap[b.x, b.y, b.z] = (byte)1;
                }
              }
            }
          }
          d++;
        }

        for (int z = 0; z < 16; ++z){
          for (int y = 0; y < 48; ++y){
            for (int x = 0; x < 16; ++x){
              if (caveMap[x, y, z] == 1){
                result.Add(new Change(x, y - 48, z, BlockTypes.AIR, 0));
              }
            }
          }
        }
        break;
    }
    return result;
  }

  private static void GenerateTemplate(Type type){
    List<Change> result = new List<Change>();
    switch (type){
      case Type.OAK_TREE:
        break;
      case Type.CAVE_ENTRANCE:
        break;
      case Type.WELL:
        for (int z = -2; z < 4; ++z){
          for (int x = -2; x < 4; ++x){
            result.Add(new Change(x, -1, z, BlockTypes.COBBLESTONE, BlockStateUtil.CreateDefaultStateData()));
          }
        }

        for (int z = -1; z < 3; ++z){
          for (int x = -1; x < 3; ++x){
            result.Add(new Change(x, 0, z, BlockTypes.COBBLESTONE, BlockStateUtil.CreateDefaultStateData()));
          }
        }

        for (int z = -1; z < 3; ++z){
          for (int x = -1; x < 3; ++x){
            result.Add(new Change(x, 3, z, BlockTypes.PLANKS_OAK, BlockStateUtil.CreateDefaultStateData()));
          }
        }

        result.Add(new Change(-1, 1, -1, BlockTypes.PLANKS_OAK, BlockStateUtil.CreateDefaultStateData()));
        result.Add(new Change(2, 1, -1, BlockTypes.PLANKS_OAK, BlockStateUtil.CreateDefaultStateData()));
        result.Add(new Change(-1, 1, 2, BlockTypes.PLANKS_OAK, BlockStateUtil.CreateDefaultStateData()));
        result.Add(new Change(2, 1, 2, BlockTypes.PLANKS_OAK, BlockStateUtil.CreateDefaultStateData()));
        result.Add(new Change(-1, 2, -1, BlockTypes.PLANKS_OAK, BlockStateUtil.CreateDefaultStateData()));
        result.Add(new Change(2, 2, -1, BlockTypes.PLANKS_OAK, BlockStateUtil.CreateDefaultStateData()));
        result.Add(new Change(-1, 2, 2, BlockTypes.PLANKS_OAK, BlockStateUtil.CreateDefaultStateData()));
        result.Add(new Change(2, 2, 2, BlockTypes.PLANKS_OAK, BlockStateUtil.CreateDefaultStateData()));

        for (int i = 0; i < 16; ++i){
          result.Add(new Change(0, -i, 0, BlockTypes.AIR, 0));
          result.Add(new Change(1, -i, 0, BlockTypes.AIR, 0));
          result.Add(new Change(0, -i, 1, BlockTypes.AIR, 0));
          result.Add(new Change(1, -i, 1, BlockTypes.AIR, 0));
        }
        break;
      case Type.CACTUS:
        result.Add(new Change(0, 0, 0, BlockTypes.CACTUS, BlockStateUtil.CreateDefaultStateData()));
        result.Add(new Change(0, 1, 0, BlockTypes.CACTUS, BlockStateUtil.CreateDefaultStateData()));
        result.Add(new Change(0, 2, 0, BlockTypes.CACTUS, BlockStateUtil.CreateDefaultStateData()));
        break;
      case Type.FOLIAGE:
        result.Add(new Change(0, 0, 0, BlockTypes.POPPY, BlockStateUtil.CreateDefaultStateData()));
        break;
    }
    templates.Add(type, result);
  }
}