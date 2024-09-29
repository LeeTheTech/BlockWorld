using System.Collections.Generic;
using UnityEngine;

public class TextureMapper{
  public Dictionary<byte, TextureMap> map = new(){
      {
          BlockTypes.GRASS, new TextureMap(
              new TextureMap.Face(new Vector2(0, 1)),
              new TextureMap.Face(new Vector2(0, 1)),
              new TextureMap.Face(new Vector2(0, 1)),
              new TextureMap.Face(new Vector2(0, 1)),
              new TextureMap.Face(new Vector2(0, 0)),
              new TextureMap.Face(new Vector2(0, 2)),
              new Color32(255, 255, 255, 255)
          )
      },{
          BlockTypes.DIRT, new TextureMap(
              new TextureMap.Face(new Vector2(0, 2)),
              new TextureMap.Face(new Vector2(0, 2)),
              new TextureMap.Face(new Vector2(0, 2)),
              new TextureMap.Face(new Vector2(0, 2)),
              new TextureMap.Face(new Vector2(0, 2)),
              new TextureMap.Face(new Vector2(0, 2)),
              new Color32(255, 255, 255, 255)
          )
      },{
          BlockTypes.STONE, new TextureMap(
              new TextureMap.Face(new Vector2(1, 0)),
              new TextureMap.Face(new Vector2(1, 0)),
              new TextureMap.Face(new Vector2(1, 0)),
              new TextureMap.Face(new Vector2(1, 0)),
              new TextureMap.Face(new Vector2(1, 0)),
              new TextureMap.Face(new Vector2(1, 0)),
              new Color32(255, 255, 255, 255)
          )
      },{
          BlockTypes.BEDROCK, new TextureMap(
              new TextureMap.Face(new Vector2(2, 0)),
              new TextureMap.Face(new Vector2(2, 0)),
              new TextureMap.Face(new Vector2(2, 0)),
              new TextureMap.Face(new Vector2(2, 0)),
              new TextureMap.Face(new Vector2(2, 0)),
              new TextureMap.Face(new Vector2(2, 0)),
              new Color32(255, 255, 255, 255)
          )
      },{
          BlockTypes.COAL, new TextureMap(
              new TextureMap.Face(new Vector2(1, 1)),
              new TextureMap.Face(new Vector2(1, 1)),
              new TextureMap.Face(new Vector2(1, 1)),
              new TextureMap.Face(new Vector2(1, 1)),
              new TextureMap.Face(new Vector2(1, 1)),
              new TextureMap.Face(new Vector2(1, 1)),
              new Color32(255, 255, 255, 255)
          )
      },{
          BlockTypes.IRON, new TextureMap(
              new TextureMap.Face(new Vector2(1, 2)),
              new TextureMap.Face(new Vector2(1, 2)),
              new TextureMap.Face(new Vector2(1, 2)),
              new TextureMap.Face(new Vector2(1, 2)),
              new TextureMap.Face(new Vector2(1, 2)),
              new TextureMap.Face(new Vector2(1, 2)),
              new Color32(255, 255, 255, 255)
          )
      },{
          BlockTypes.GOLD, new TextureMap(
              new TextureMap.Face(new Vector2(1, 3)),
              new TextureMap.Face(new Vector2(1, 3)),
              new TextureMap.Face(new Vector2(1, 3)),
              new TextureMap.Face(new Vector2(1, 3)),
              new TextureMap.Face(new Vector2(1, 3)),
              new TextureMap.Face(new Vector2(1, 3)),
              new Color32(255, 255, 255, 255)
          )
      },{
          BlockTypes.DIAMOND, new TextureMap(
              new TextureMap.Face(new Vector2(1, 4)),
              new TextureMap.Face(new Vector2(1, 4)),
              new TextureMap.Face(new Vector2(1, 4)),
              new TextureMap.Face(new Vector2(1, 4)),
              new TextureMap.Face(new Vector2(1, 4)),
              new TextureMap.Face(new Vector2(1, 4)),
              new Color32(255, 255, 255, 255)
          )
      },{
          BlockTypes.LOG_OAK, new TextureMap(
              new TextureMap.Face(new Vector2(3, 3)),
              new TextureMap.Face(new Vector2(3, 3)),
              new TextureMap.Face(new Vector2(3, 3)),
              new TextureMap.Face(new Vector2(3, 3)),
              new TextureMap.Face(new Vector2(3, 2)),
              new TextureMap.Face(new Vector2(3, 2)),
              new Color32(255, 255, 255, 255)
          )
      },{
          BlockTypes.PLANKS_OAK, new TextureMap(
              new TextureMap.Face(new Vector2(3, 1)),
              new TextureMap.Face(new Vector2(3, 1)),
              new TextureMap.Face(new Vector2(3, 1)),
              new TextureMap.Face(new Vector2(3, 1)),
              new TextureMap.Face(new Vector2(3, 1)),
              new TextureMap.Face(new Vector2(3, 1)),
              new Color32(255, 255, 255, 255)
          )
      },{
          BlockTypes.LEAVES_OAK, new TextureMap(
              new TextureMap.Face(new Vector2(3, 4)),
              new TextureMap.Face(new Vector2(3, 4)),
              new TextureMap.Face(new Vector2(3, 4)),
              new TextureMap.Face(new Vector2(3, 4)),
              new TextureMap.Face(new Vector2(3, 4)),
              new TextureMap.Face(new Vector2(3, 4)),
              new Color32(168, 255, 68, 255)
          )
      },{
          BlockTypes.GLASS, new TextureMap(
              new TextureMap.Face(new Vector2(0, 3)),
              new TextureMap.Face(new Vector2(0, 3)),
              new TextureMap.Face(new Vector2(0, 3)),
              new TextureMap.Face(new Vector2(0, 3)),
              new TextureMap.Face(new Vector2(0, 3)),
              new TextureMap.Face(new Vector2(0, 3)),
              new Color32(255, 255, 255, 255)
          )
      },{
          BlockTypes.WATER, new TextureMap(
              new TextureMap.Face(new Vector2(0, 5)),
              new TextureMap.Face(new Vector2(0, 5)),
              new TextureMap.Face(new Vector2(0, 5)),
              new TextureMap.Face(new Vector2(0, 5)),
              new TextureMap.Face(new Vector2(0, 5)),
              new TextureMap.Face(new Vector2(0, 5)),
              new Color32(12, 152, 232, 255)
          )
      },{
          BlockTypes.GLOWSTONE, new TextureMap(
              new TextureMap.Face(new Vector2(3, 0)),
              new TextureMap.Face(new Vector2(3, 0)),
              new TextureMap.Face(new Vector2(3, 0)),
              new TextureMap.Face(new Vector2(3, 0)),
              new TextureMap.Face(new Vector2(3, 0)),
              new TextureMap.Face(new Vector2(3, 0)),
              new Color32(255, 255, 255, 255)
          )
      },{
          BlockTypes.ANDESITE, new TextureMap(
              new TextureMap.Face(new Vector2(2, 3)),
              new TextureMap.Face(new Vector2(2, 3)),
              new TextureMap.Face(new Vector2(2, 3)),
              new TextureMap.Face(new Vector2(2, 3)),
              new TextureMap.Face(new Vector2(2, 3)),
              new TextureMap.Face(new Vector2(2, 3)),
              new Color32(255, 255, 255, 255)
          )
      },{
          BlockTypes.DIORITE, new TextureMap(
              new TextureMap.Face(new Vector2(2, 1)),
              new TextureMap.Face(new Vector2(2, 1)),
              new TextureMap.Face(new Vector2(2, 1)),
              new TextureMap.Face(new Vector2(2, 1)),
              new TextureMap.Face(new Vector2(2, 1)),
              new TextureMap.Face(new Vector2(2, 1)),
              new Color32(255, 255, 255, 255)
          )
      },{
          BlockTypes.GRANITE, new TextureMap(
              new TextureMap.Face(new Vector2(2, 2)),
              new TextureMap.Face(new Vector2(2, 2)),
              new TextureMap.Face(new Vector2(2, 2)),
              new TextureMap.Face(new Vector2(2, 2)),
              new TextureMap.Face(new Vector2(2, 2)),
              new TextureMap.Face(new Vector2(2, 2)),
              new Color32(255, 255, 255, 255)
          )
      },{
          BlockTypes.COBBLESTONE, new TextureMap(
              new TextureMap.Face(new Vector2(2, 4)),
              new TextureMap.Face(new Vector2(2, 4)),
              new TextureMap.Face(new Vector2(2, 4)),
              new TextureMap.Face(new Vector2(2, 4)),
              new TextureMap.Face(new Vector2(2, 4)),
              new TextureMap.Face(new Vector2(2, 4)),
              new Color32(255, 255, 255, 255)
          )
      },{
          BlockTypes.SAND, new TextureMap(
              new TextureMap.Face(new Vector2(0, 4)),
              new TextureMap.Face(new Vector2(0, 4)),
              new TextureMap.Face(new Vector2(0, 4)),
              new TextureMap.Face(new Vector2(0, 4)),
              new TextureMap.Face(new Vector2(0, 4)),
              new TextureMap.Face(new Vector2(0, 4)),
              new Color32(255, 255, 255, 255)
          )
      },{
          BlockTypes.CACTUS, new TextureMap(
              new TextureMap.Face(new Vector2(0, 6)),
              new TextureMap.Face(new Vector2(0, 6)),
              new TextureMap.Face(new Vector2(0, 6)),
              new TextureMap.Face(new Vector2(0, 6)),
              new TextureMap.Face(new Vector2(0, 6)),
              new TextureMap.Face(new Vector2(0, 6)),
              new Color32(255, 255, 255, 255)
          )
      },{
          BlockTypes.ICE, new TextureMap(
              new TextureMap.Face(new Vector2(2, 5)),
              new TextureMap.Face(new Vector2(2, 5)),
              new TextureMap.Face(new Vector2(2, 5)),
              new TextureMap.Face(new Vector2(2, 5)),
              new TextureMap.Face(new Vector2(2, 5)),
              new TextureMap.Face(new Vector2(2, 5)),
              new Color32(255, 255, 255, 255)
          )
      },{
          BlockTypes.PLANKS_OAK_SLAB, new TextureMap(
              new TextureMap.Face(new Vector2(3, 1)),
              new TextureMap.Face(new Vector2(3, 1)),
              new TextureMap.Face(new Vector2(3, 1)),
              new TextureMap.Face(new Vector2(3, 1)),
              new TextureMap.Face(new Vector2(3, 1)),
              new TextureMap.Face(new Vector2(3, 1)),
              new Color32(255, 255, 255, 255)
          )
      },{
          BlockTypes.PLANKS_OAK_STAIR, new TextureMap(
              new TextureMap.Face(new Vector2(3, 1)),
              new TextureMap.Face(new Vector2(3, 1)),
              new TextureMap.Face(new Vector2(3, 1)),
              new TextureMap.Face(new Vector2(3, 1)),
              new TextureMap.Face(new Vector2(3, 1)),
              new TextureMap.Face(new Vector2(3, 1)),
              new Color32(255, 255, 255, 255)
          )
      },{
          BlockTypes.TORCH, new TextureMap(
              new TextureMap.Face(new Vector2(3, 1)),
              new TextureMap.Face(new Vector2(3, 1)),
              new TextureMap.Face(new Vector2(3, 1)),
              new TextureMap.Face(new Vector2(3, 1)),
              new TextureMap.Face(new Vector2(3, 1)),
              new TextureMap.Face(new Vector2(3, 1)),
              new Color32(255, 255, 255, 255)
          )
      },{
          BlockTypes.SNOW_GRASS, new TextureMap(
              new TextureMap.Face(new Vector2(1, 7)),
              new TextureMap.Face(new Vector2(1, 7)),
              new TextureMap.Face(new Vector2(1, 7)),
              new TextureMap.Face(new Vector2(1, 7)),
              new TextureMap.Face(new Vector2(2, 7)),
              new TextureMap.Face(new Vector2(0, 7)),
              new Color32(255, 255, 255, 255)
          )
      },{
          BlockTypes.LAVA, new TextureMap(
              new TextureMap.Face(new Vector2(3, 5)),
              new TextureMap.Face(new Vector2(3, 5)),
              new TextureMap.Face(new Vector2(3, 5)),
              new TextureMap.Face(new Vector2(3, 5)),
              new TextureMap.Face(new Vector2(3, 5)),
              new TextureMap.Face(new Vector2(3, 5)),
              new Color32(255, 50, 11, 255)
          )
      },{
          BlockTypes.FOLIAGE, new TextureMap(
              new TextureMap.Face(new Vector2(2, 6)),
              new TextureMap.Face(new Vector2(2, 6)),
              new TextureMap.Face(new Vector2(2, 6)),
              new TextureMap.Face(new Vector2(2, 6)),
              new TextureMap.Face(new Vector2(2, 6)),
              new TextureMap.Face(new Vector2(2, 6)),
              new Color32(48, 255, 0, 255)
          )
      },{
          BlockTypes.TNT, new TextureMap(
              new TextureMap.Face(new Vector2(3, 6)),
              new TextureMap.Face(new Vector2(3, 6)),
              new TextureMap.Face(new Vector2(3, 6)),
              new TextureMap.Face(new Vector2(3, 6)),
              new TextureMap.Face(new Vector2(3, 7)),
              new TextureMap.Face(new Vector2(4, 0)),
              new Color32(255, 255, 255, 255)
          )
      },{
          BlockTypes.GRAVEL, new TextureMap(
              new TextureMap.Face(new Vector2(4, 1)),
              new TextureMap.Face(new Vector2(4, 1)),
              new TextureMap.Face(new Vector2(4, 1)),
              new TextureMap.Face(new Vector2(4, 1)),
              new TextureMap.Face(new Vector2(4, 1)),
              new TextureMap.Face(new Vector2(4, 1)),
              new Color32(255, 255, 255, 255)
          )
      },{
          BlockTypes.POPPY, new TextureMap(
              new TextureMap.Face(new Vector2(4, 2)),
              new TextureMap.Face(new Vector2(4, 2)),
              new TextureMap.Face(new Vector2(4, 2)),
              new TextureMap.Face(new Vector2(4, 2)),
              new TextureMap.Face(new Vector2(4, 2)),
              new TextureMap.Face(new Vector2(4, 2)),
              new Color32(255, 255, 255, 255)
          )
      }
  };

  public class TextureMap{
    public TextureMap(Face front, Face back, Face left, Face right, Face top, Face bottom, Color defaultColor){
      this.front = front;
      this.back = back;
      this.left = left;
      this.right = right;
      this.top = top;
      this.bottom = bottom;
      this.defaultColor = defaultColor;
    }

    public Face front, back, left, right, top, bottom;
    public Color32 defaultColor;

    public class Face{
      public Face(Vector2 tl){
        this.tl = tl;
        tr = tl + new Vector2(1, 0);
        bl = tl + new Vector2(0, 1);
        br = tl + new Vector2(1, 1);
      }

      public Vector2 tl, tr, bl, br;
    }
  }
}