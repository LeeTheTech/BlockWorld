using System;
using UnityEngine;

public static class BlockShapes{
  public enum BlockShape{
    CUBE,
    SLAB,
    STAIR,
    TORCH
  }
  
  private enum LightFace{
    LEFT,
    RIGHT,
    TOP,
    BOTTOM,
    FRONT,
    BACK
  }
  
  public struct ShapeData{
    public Vector2Int position;
    public TextureMapper.TextureMap textureMap;
    public byte[,,] lightMap;
    public byte blockType;
    public int x;
    public int y;
    public int z;
    public byte bR;
    public byte bL;
    public byte bF;
    public byte bB;
    public byte bU;
    public byte bD;
    public int lx;
    public int ly;
    public int lz;
    
    public ShapeData(Vector2Int position, TextureMapper.TextureMap textureMap, byte[,,] lightMap, byte blockType, int x, int y, int z, byte bR, byte bL, byte bF, byte bB, byte bU, byte bD, int lx, int ly, int lz){
      this.position = position;
      this.textureMap = textureMap;
      this.lightMap = lightMap;
      this.blockType = blockType;
      this.x = x;
      this.y = y;
      this.z = z;
      this.bR = bR;
      this.bL = bL;
      this.bF = bF;
      this.bB = bB;
      this.bU = bU;
      this.bD = bD;
      this.lx = lx;
      this.ly = ly;
      this.lz = lz;
    }
  }

  public static void AddFaces(ShapeData shapeData, ChunkMeshData meshData){
    switch (BlockTypes.blockShape[shapeData.blockType]){
      case BlockShape.CUBE:
        AddCubeFaces(shapeData, meshData);
        break;
      case BlockShape.SLAB:
        AddSlabFaces(shapeData, meshData);
        break;
      case BlockShape.STAIR:
        AddStairFaces(shapeData, meshData);
        break;
      case BlockShape.TORCH:
        AddTorchFaces(shapeData, meshData);
        break;
      default: 
        AddCubeFaces(shapeData, meshData);
        break;
    }
  }

  private static void AddCubeFaces(ShapeData shapeData, ChunkMeshData meshData){
    if (ShouldRenderFace(shapeData.blockType, shapeData.bR)){
      meshData.AddFace(
          new Vector3(shapeData.x + 1, shapeData.y, shapeData.z),
          new Vector3(shapeData.x + 1, shapeData.y + 1, shapeData.z),
          new Vector3(shapeData.x + 1, shapeData.y + 1, shapeData.z + 1),
          new Vector3(shapeData.x + 1, shapeData.y, shapeData.z + 1),
          Vector3.right
      );
      meshData.AddTextureFace(shapeData.textureMap.right);
      AddLighting(meshData, shapeData, LightFace.RIGHT);
    }

    if (ShouldRenderFace(shapeData.blockType, shapeData.bL)){
      meshData.AddFace(
          new Vector3(shapeData.x, shapeData.y, shapeData.z + 1),
          new Vector3(shapeData.x, shapeData.y + 1, shapeData.z + 1),
          new Vector3(shapeData.x, shapeData.y + 1, shapeData.z),
          new Vector3(shapeData.x, shapeData.y, shapeData.z),
          -Vector3.right
      );
      meshData.AddTextureFace(shapeData.textureMap.left);
      AddLighting(meshData, shapeData, LightFace.LEFT);
    }

    if (ShouldRenderFace(shapeData.blockType, shapeData.bU)){
      meshData.AddFace(
          new Vector3(shapeData.x, shapeData.y + 1, shapeData.z),
          new Vector3(shapeData.x, shapeData.y + 1, shapeData.z + 1),
          new Vector3(shapeData.x + 1, shapeData.y + 1, shapeData.z + 1),
          new Vector3(shapeData.x + 1, shapeData.y + 1, shapeData.z),
          Vector3.up
      );
      meshData.AddTextureFace(shapeData.textureMap.top);
      AddLighting(meshData, shapeData, LightFace.TOP);
    }

    if (ShouldRenderFace(shapeData.blockType, shapeData.bD)){
      meshData.AddFace(
          new Vector3(shapeData.x, shapeData.y, shapeData.z + 1),
          new Vector3(shapeData.x, shapeData.y, shapeData.z),
          new Vector3(shapeData.x + 1, shapeData.y, shapeData.z),
          new Vector3(shapeData.x + 1, shapeData.y, shapeData.z + 1),
          -Vector3.up
      );
      meshData.AddTextureFace(shapeData.textureMap.bottom);
      AddLighting(meshData, shapeData, LightFace.BOTTOM);
    }

    if (ShouldRenderFace(shapeData.blockType, shapeData.bF)){
      meshData.AddFace(
          new Vector3(shapeData.x + 1, shapeData.y, shapeData.z + 1),
          new Vector3(shapeData.x + 1, shapeData.y + 1, shapeData.z + 1),
          new Vector3(shapeData.x, shapeData.y + 1, shapeData.z + 1),
          new Vector3(shapeData.x, shapeData.y, shapeData.z + 1),
          Vector3.forward
      );
      meshData.AddTextureFace(shapeData.textureMap.front);
      AddLighting(meshData, shapeData, LightFace.FRONT);
    }

    if (ShouldRenderFace(shapeData.blockType, shapeData.bB)){
      meshData.AddFace(
          new Vector3(shapeData.x, shapeData.y, shapeData.z),
          new Vector3(shapeData.x, shapeData.y + 1, shapeData.z),
          new Vector3(shapeData.x + 1, shapeData.y + 1, shapeData.z),
          new Vector3(shapeData.x + 1, shapeData.y, shapeData.z),
          -Vector3.forward
      );
      meshData.AddTextureFace(shapeData.textureMap.back);
      AddLighting(meshData, shapeData, LightFace.BACK);
    }
  }

  private static void AddSlabFaces(ShapeData shapeData, ChunkMeshData meshData){
    if (ShouldRenderFace(shapeData.blockType, shapeData.bR)){
      meshData.AddFace(
          new Vector3(shapeData.x + 1, shapeData.y, shapeData.z),
          new Vector3(shapeData.x + 1, shapeData.y + 0.5f, shapeData.z),
          new Vector3(shapeData.x + 1, shapeData.y + 0.5f, shapeData.z + 1),
          new Vector3(shapeData.x + 1, shapeData.y, shapeData.z + 1),
          Vector3.right
      );
      meshData.AddTextureFace(shapeData.textureMap.right);
      AddLighting(meshData, shapeData, LightFace.RIGHT);
    }
    
    if (ShouldRenderFace(shapeData.blockType, shapeData.bL)){
      meshData.AddFace(
          new Vector3(shapeData.x, shapeData.y, shapeData.z + 1),
          new Vector3(shapeData.x, shapeData.y + 0.5f, shapeData.z + 1),
          new Vector3(shapeData.x, shapeData.y + 0.5f, shapeData.z),
          new Vector3(shapeData.x, shapeData.y, shapeData.z),
          -Vector3.right
      );
      meshData.AddTextureFace(shapeData.textureMap.left);
      AddLighting(meshData, shapeData, LightFace.LEFT);
    }
    
    if (ShouldRenderFace(shapeData.blockType, shapeData.bU)){
      meshData.AddFace(
          new Vector3(shapeData.x, shapeData.y + 0.5f, shapeData.z),
          new Vector3(shapeData.x, shapeData.y + 0.5f, shapeData.z + 1),
          new Vector3(shapeData.x + 1, shapeData.y + 0.5f, shapeData.z + 1),
          new Vector3(shapeData.x + 1, shapeData.y + 0.5f, shapeData.z),
          Vector3.up
      );
      meshData.AddTextureFace(shapeData.textureMap.top);
      AddLighting(meshData, shapeData, LightFace.TOP);
    }
    
    if (ShouldRenderFace(shapeData.blockType, shapeData.bD)){
      meshData.AddFace(
          new Vector3(shapeData.x, shapeData.y, shapeData.z + 1),
          new Vector3(shapeData.x, shapeData.y, shapeData.z),
          new Vector3(shapeData.x + 1, shapeData.y, shapeData.z),
          new Vector3(shapeData.x + 1, shapeData.y, shapeData.z + 1),
          -Vector3.up
      );
      meshData.AddTextureFace(shapeData.textureMap.bottom);
      AddLighting(meshData, shapeData, LightFace.BOTTOM);
    }
    
    if (ShouldRenderFace(shapeData.blockType, shapeData.bF)){
      meshData.AddFace(
          new Vector3(shapeData.x + 1, shapeData.y, shapeData.z + 1),
          new Vector3(shapeData.x + 1, shapeData.y + 0.5f, shapeData.z + 1),
          new Vector3(shapeData.x, shapeData.y + 0.5f, shapeData.z + 1),
          new Vector3(shapeData.x, shapeData.y, shapeData.z + 1),
          Vector3.forward
      );
      meshData.AddTextureFace(shapeData.textureMap.front);
      AddLighting(meshData, shapeData, LightFace.FRONT);
    }
    
    if (ShouldRenderFace(shapeData.blockType, shapeData.bB)){
      meshData.AddFace(
          new Vector3(shapeData.x, shapeData.y, shapeData.z),
          new Vector3(shapeData.x, shapeData.y + 0.5f, shapeData.z),
          new Vector3(shapeData.x + 1, shapeData.y + 0.5f, shapeData.z),
          new Vector3(shapeData.x + 1, shapeData.y, shapeData.z),
          -Vector3.forward
      );
      meshData.AddTextureFace(shapeData.textureMap.back);
      AddLighting(meshData, shapeData, LightFace.BACK);
    }
  }

  private static void AddTorchFaces(ShapeData shapeData, ChunkMeshData meshData){
    //TODO implement later
    AddSlabFaces(shapeData, meshData);
  }

  private static void AddStairFaces(ShapeData shapeData, ChunkMeshData meshData){
    //TODO implement later
    AddSlabFaces(shapeData, meshData);
  }


  private static void AddLighting(ChunkMeshData meshData, ShapeData shapeData, LightFace LightFace){
    int b;
    int t;
    byte bl;
    byte tl;
    byte tr;
    byte br;
    switch (LightFace){
      case LightFace.RIGHT:
        b = shapeData.y == 0 ? 0 : 1;
        t = shapeData.y == 255 ? 0 : 1;
        bl = (byte)((shapeData.lightMap[shapeData.lx + 1, shapeData.ly, shapeData.lz] + shapeData.lightMap[shapeData.lx + 1, shapeData.ly, shapeData.lz - 1] + shapeData.lightMap[shapeData.lx + 1, shapeData.ly - b, shapeData.lz] + shapeData.lightMap[shapeData.lx + 1, shapeData.ly - b, shapeData.lz - 1]) / 4);
        tl = (byte)((shapeData.lightMap[shapeData.lx + 1, shapeData.ly, shapeData.lz] + shapeData.lightMap[shapeData.lx + 1, shapeData.ly, shapeData.lz - 1] + shapeData.lightMap[shapeData.lx + 1, shapeData.ly + t, shapeData.lz] + shapeData.lightMap[shapeData.lx + 1, shapeData.ly + t, shapeData.lz - 1]) / 4);
        tr = (byte)((shapeData.lightMap[shapeData.lx + 1, shapeData.ly, shapeData.lz] + shapeData.lightMap[shapeData.lx + 1, shapeData.ly, shapeData.lz + 1] + shapeData.lightMap[shapeData.lx + 1, shapeData.ly + t, shapeData.lz] + shapeData.lightMap[shapeData.lx + 1, shapeData.ly + t, shapeData.lz + 1]) / 4);
        br = (byte)((shapeData.lightMap[shapeData.lx + 1, shapeData.ly, shapeData.lz] + shapeData.lightMap[shapeData.lx + 1, shapeData.ly, shapeData.lz + 1] + shapeData.lightMap[shapeData.lx + 1, shapeData.ly - b, shapeData.lz] + shapeData.lightMap[shapeData.lx + 1, shapeData.ly - b, shapeData.lz + 1]) / 4);
        meshData.AddColors(shapeData.textureMap, bl, tl, tr, br);
        return;
      case LightFace.LEFT:
        b = (shapeData.y == 0 ? 0 : 1);
        t = (shapeData.y == 255 ? 0 : 1);
        br = (byte)((shapeData.lightMap[shapeData.lx - 1, shapeData.ly, shapeData.lz] + shapeData.lightMap[shapeData.lx - 1, shapeData.ly, shapeData.lz - 1] + shapeData.lightMap[shapeData.lx - 1, shapeData.ly - b, shapeData.lz] + shapeData.lightMap[shapeData.lx - 1, shapeData.ly - b, shapeData.lz - 1]) / 4);
        tr = (byte)((shapeData.lightMap[shapeData.lx - 1, shapeData.ly, shapeData.lz] + shapeData.lightMap[shapeData.lx - 1, shapeData.ly, shapeData.lz - 1] + shapeData.lightMap[shapeData.lx - 1, shapeData.ly + t, shapeData.lz] + shapeData.lightMap[shapeData.lx - 1, shapeData.ly + t, shapeData.lz - 1]) / 4);
        tl = (byte)((shapeData.lightMap[shapeData.lx - 1, shapeData.ly, shapeData.lz] + shapeData.lightMap[shapeData.lx - 1, shapeData.ly, shapeData.lz + 1] + shapeData.lightMap[shapeData.lx - 1, shapeData.ly + t, shapeData.lz] + shapeData.lightMap[shapeData.lx - 1, shapeData.ly + t, shapeData.lz + 1]) / 4);
        bl = (byte)((shapeData.lightMap[shapeData.lx - 1, shapeData.ly, shapeData.lz] + shapeData.lightMap[shapeData.lx - 1, shapeData.ly, shapeData.lz + 1] + shapeData.lightMap[shapeData.lx - 1, shapeData.ly - b, shapeData.lz] + shapeData.lightMap[shapeData.lx - 1, shapeData.ly - b, shapeData.lz + 1]) / 4);
        meshData.AddColors(shapeData.textureMap, bl, tl, tr, br);
        return;
      case LightFace.TOP:
        b = (shapeData.y == 0 ? 0 : 1);
        t = (shapeData.y == 255 ? 0 : 1);
        bl = (byte)((shapeData.lightMap[shapeData.lx, shapeData.ly + t, shapeData.lz] + shapeData.lightMap[shapeData.lx - 1, shapeData.ly + t, shapeData.lz] + shapeData.lightMap[shapeData.lx, shapeData.ly + t, shapeData.lz - 1] + shapeData.lightMap[shapeData.lx - 1, shapeData.ly + t, shapeData.lz - 1]) / 4);
        tl = (byte)((shapeData.lightMap[shapeData.lx, shapeData.ly + t, shapeData.lz] + shapeData.lightMap[shapeData.lx - 1, shapeData.ly + t, shapeData.lz] + shapeData.lightMap[shapeData.lx, shapeData.ly + t, shapeData.lz + 1] + shapeData.lightMap[shapeData.lx - 1, shapeData.ly + t, shapeData.lz + 1]) / 4);
        tr = (byte)((shapeData.lightMap[shapeData.lx, shapeData.ly + t, shapeData.lz] + shapeData.lightMap[shapeData.lx + 1, shapeData.ly + t, shapeData.lz] + shapeData.lightMap[shapeData.lx, shapeData.ly + t, shapeData.lz + 1] + shapeData.lightMap[shapeData.lx + 1, shapeData.ly + t, shapeData.lz + 1]) / 4);
        br = (byte)((shapeData.lightMap[shapeData.lx, shapeData.ly + t, shapeData.lz] + shapeData.lightMap[shapeData.lx + 1, shapeData.ly + t, shapeData.lz] + shapeData.lightMap[shapeData.lx, shapeData.ly + t, shapeData.lz - 1] + shapeData.lightMap[shapeData.lx + 1, shapeData.ly + t, shapeData.lz - 1]) / 4);
        meshData.AddColors(shapeData.textureMap, bl, tl, tr, br);
        return;
      case LightFace.BOTTOM:
        b = (shapeData.y == 0 ? 0 : 1);
        t = (shapeData.y == 255 ? 0 : 1);
        tl = (byte)((shapeData.lightMap[shapeData.lx, shapeData.ly - b, shapeData.lz] + shapeData.lightMap[shapeData.lx - 1, shapeData.ly - b, shapeData.lz] + shapeData.lightMap[shapeData.lx, shapeData.ly - b, shapeData.lz - 1] + shapeData.lightMap[shapeData.lx - 1, shapeData.ly - b, shapeData.lz - 1]) / 4);
        bl = (byte)((shapeData.lightMap[shapeData.lx, shapeData.ly - b, shapeData.lz] + shapeData.lightMap[shapeData.lx - 1, shapeData.ly - b, shapeData.lz] + shapeData.lightMap[shapeData.lx, shapeData.ly - b, shapeData.lz + 1] + shapeData.lightMap[shapeData.lx - 1, shapeData.ly - b, shapeData.lz + 1]) / 4);
        br = (byte)((shapeData.lightMap[shapeData.lx, shapeData.ly - b, shapeData.lz] + shapeData.lightMap[shapeData.lx + 1, shapeData.ly - b, shapeData.lz] + shapeData.lightMap[shapeData.lx, shapeData.ly - b, shapeData.lz + 1] + shapeData.lightMap[shapeData.lx + 1, shapeData.ly - b, shapeData.lz + 1]) / 4);
        tr = (byte)((shapeData.lightMap[shapeData.lx, shapeData.ly - b, shapeData.lz] + shapeData.lightMap[shapeData.lx + 1, shapeData.ly - b, shapeData.lz] + shapeData.lightMap[shapeData.lx, shapeData.ly - b, shapeData.lz - 1] + shapeData.lightMap[shapeData.lx + 1, shapeData.ly - b, shapeData.lz - 1]) / 4);
        meshData.AddColors(shapeData.textureMap, bl, tl, tr, br);
        return;
      case LightFace.FRONT:
        b = (shapeData.y == 0 ? 0 : 1);
        t = (shapeData.y == 255 ? 0 : 1);
        br = (byte)((shapeData.lightMap[shapeData.lx, shapeData.ly, shapeData.lz + 1] + shapeData.lightMap[shapeData.lx - 1, shapeData.ly, shapeData.lz + 1] + shapeData.lightMap[shapeData.lx, shapeData.ly - b, shapeData.lz + 1] + shapeData.lightMap[shapeData.lx - 1, shapeData.ly - b, shapeData.lz + 1]) / 4);
        tr = (byte)((shapeData.lightMap[shapeData.lx, shapeData.ly, shapeData.lz + 1] + shapeData.lightMap[shapeData.lx - 1, shapeData.ly, shapeData.lz + 1] + shapeData.lightMap[shapeData.lx, shapeData.ly + t, shapeData.lz + 1] + shapeData.lightMap[shapeData.lx - 1, shapeData.ly + t, shapeData.lz + 1]) / 4);
        tl = (byte)((shapeData.lightMap[shapeData.lx, shapeData.ly, shapeData.lz + 1] + shapeData.lightMap[shapeData.lx + 1, shapeData.ly, shapeData.lz + 1] + shapeData.lightMap[shapeData.lx, shapeData.ly + t, shapeData.lz + 1] + shapeData.lightMap[shapeData.lx + 1, shapeData.ly + t, shapeData.lz + 1]) / 4);
        bl = (byte)((shapeData.lightMap[shapeData.lx, shapeData.ly, shapeData.lz + 1] + shapeData.lightMap[shapeData.lx + 1, shapeData.ly, shapeData.lz + 1] + shapeData.lightMap[shapeData.lx, shapeData.ly - b, shapeData.lz + 1] + shapeData.lightMap[shapeData.lx + 1, shapeData.ly - b, shapeData.lz + 1]) / 4);
        meshData.AddColors(shapeData.textureMap, bl, tl, tr, br);
        return;
      case LightFace.BACK:
        b = (shapeData.y == 0 ? 0 : 1);
        t = (shapeData.y == 255 ? 0 : 1);
        bl = (byte)((shapeData.lightMap[shapeData.lx, shapeData.ly, shapeData.lz - 1] + shapeData.lightMap[shapeData.lx - 1, shapeData.ly, shapeData.lz - 1] + shapeData.lightMap[shapeData.lx, shapeData.ly - b, shapeData.lz - 1] + shapeData.lightMap[shapeData.lx - 1, shapeData.ly - b, shapeData.lz - 1]) / 4);
        tl = (byte)((shapeData.lightMap[shapeData.lx, shapeData.ly, shapeData.lz - 1] + shapeData.lightMap[shapeData.lx - 1, shapeData.ly, shapeData.lz - 1] + shapeData.lightMap[shapeData.lx, shapeData.ly + t, shapeData.lz - 1] + shapeData.lightMap[shapeData.lx - 1, shapeData.ly + t, shapeData.lz - 1]) / 4);
        tr = (byte)((shapeData.lightMap[shapeData.lx, shapeData.ly, shapeData.lz - 1] + shapeData.lightMap[shapeData.lx + 1, shapeData.ly, shapeData.lz - 1] + shapeData.lightMap[shapeData.lx, shapeData.ly + t, shapeData.lz - 1] + shapeData.lightMap[shapeData.lx + 1, shapeData.ly + t, shapeData.lz - 1]) / 4);
        br = (byte)((shapeData.lightMap[shapeData.lx, shapeData.ly, shapeData.lz - 1] + shapeData.lightMap[shapeData.lx + 1, shapeData.ly, shapeData.lz - 1] + shapeData.lightMap[shapeData.lx, shapeData.ly - b, shapeData.lz - 1] + shapeData.lightMap[shapeData.lx + 1, shapeData.ly - b, shapeData.lz - 1]) / 4);
        meshData.AddColors(shapeData.textureMap, bl, tl, tr, br);
        return;
      default:
        throw new ArgumentOutOfRangeException(nameof(LightFace), LightFace, "Invalid Light Face Direction!");
    }
  }
  
  private static bool ShouldRenderFace(byte block, byte targetBlock){
    if (BlockTypes.IsTransparentBlock(block) && BlockTypes.IsTransparentBlock(targetBlock)) return false;
    if (BlockTypes.IsTransparentCutoutBlock(block) && BlockTypes.IsTransparentCutoutBlock(targetBlock)) return false;
    if (BlockTypes.IsSlab(targetBlock)) return true;
    return targetBlock > 127;
  }
}