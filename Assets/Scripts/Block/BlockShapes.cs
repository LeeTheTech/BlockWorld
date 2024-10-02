using System;
using UnityEngine;

public static class BlockShapes{
  public enum BlockShape{
    CUBE,
    SLAB,
    STAIR,
    TORCH,
    FOLIAGE
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
    public byte[,,] blockLightMap;
    public byte[,,] sunLightMap;
    public byte blockState;
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
    
    public ShapeData(Vector2Int position, TextureMapper.TextureMap textureMap, byte[,,] blockLightMap, byte[,,] sunLightMap, byte blockState, byte blockType, int x, int y, int z, byte bR, byte bL, byte bF, byte bB, byte bU, byte bD, int lx, int ly, int lz){
      this.position = position;
      this.textureMap = textureMap;
      this.blockLightMap = blockLightMap;
      this.sunLightMap = sunLightMap;
      this.blockState = blockState;
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
        AddStairFaces(shapeData, meshData, BlockStateUtil.GetOrientation(shapeData.blockState));
        break;
      case BlockShape.TORCH:
        AddTorchFaces(shapeData, meshData);
        break;
      case BlockShape.FOLIAGE:
        AddFoliageFaces(shapeData, meshData);
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
          Vector3.right,
          BlockTypes.GenerateCollider(shapeData.blockType)
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
          -Vector3.right,
          BlockTypes.GenerateCollider(shapeData.blockType)
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
          Vector3.up,
          BlockTypes.GenerateCollider(shapeData.blockType)
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
          -Vector3.up,
          BlockTypes.GenerateCollider(shapeData.blockType)
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
          Vector3.forward,
          BlockTypes.GenerateCollider(shapeData.blockType)
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
          -Vector3.forward,
          BlockTypes.GenerateCollider(shapeData.blockType)
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
          Vector3.right,
          BlockTypes.GenerateCollider(shapeData.blockType)
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
          -Vector3.right,
          BlockTypes.GenerateCollider(shapeData.blockType)
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
          Vector3.up,
          BlockTypes.GenerateCollider(shapeData.blockType)
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
          -Vector3.up,
          BlockTypes.GenerateCollider(shapeData.blockType)
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
          Vector3.forward,
          BlockTypes.GenerateCollider(shapeData.blockType)
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
          -Vector3.forward,
          BlockTypes.GenerateCollider(shapeData.blockType)
      );
      meshData.AddTextureFace(shapeData.textureMap.back);
      AddLighting(meshData, shapeData, LightFace.BACK);
    }
  }
  
  private static void AddFoliageFaces(ShapeData shapeData, ChunkMeshData meshData){
    meshData.AddFace(
        new Vector3(shapeData.x, shapeData.y, shapeData.z),
        new Vector3(shapeData.x + 1, shapeData.y, shapeData.z + 1), 
        new Vector3(shapeData.x + 1, shapeData.y + 1, shapeData.z + 1),
        new Vector3(shapeData.x, shapeData.y + 1, shapeData.z),
        Vector3.forward,
        BlockTypes.GenerateCollider(shapeData.blockType)
    );
    meshData.AddTextureFace(shapeData.textureMap.top);
    AddLighting(meshData, shapeData, LightFace.TOP);
    
    meshData.AddFace(
        new Vector3(shapeData.x + 1, shapeData.y, shapeData.z),
        new Vector3(shapeData.x, shapeData.y, shapeData.z + 1),
        new Vector3(shapeData.x, shapeData.y + 1, shapeData.z + 1),
        new Vector3(shapeData.x + 1, shapeData.y + 1, shapeData.z),
        Vector3.forward,
        BlockTypes.GenerateCollider(shapeData.blockType)
    );
    meshData.AddTextureFace(shapeData.textureMap.top);
    AddLighting(meshData, shapeData, LightFace.TOP);
  }

  private static void AddTorchFaces(ShapeData shapeData, ChunkMeshData meshData){
    //TODO implement later
    AddSlabFaces(shapeData, meshData);
  }

  private static void AddStairFaces(ShapeData shapeData, ChunkMeshData meshData, Direction direction){
    Vector3 pivot = new Vector3(shapeData.x + 0.5f, shapeData.y, shapeData.z + 0.5f);

    if (ShouldRenderFace(shapeData.blockType, shapeData.bB)){
      meshData.AddFace(
          RotateVertex(new Vector3(shapeData.x, shapeData.y, shapeData.z), direction, pivot),
          RotateVertex(new Vector3(shapeData.x, shapeData.y + 1, shapeData.z), direction, pivot),
          RotateVertex(new Vector3(shapeData.x + 1, shapeData.y + 1, shapeData.z), direction, pivot),
          RotateVertex(new Vector3(shapeData.x + 1, shapeData.y, shapeData.z), direction, pivot),
          -Vector3.forward,
          BlockTypes.GenerateCollider(shapeData.blockType)
      );
      meshData.AddTextureFace(shapeData.textureMap.back);
      AddLighting(meshData, shapeData, LightFace.BACK);
    }

    if (ShouldRenderFace(shapeData.blockType, shapeData.bU)){
      meshData.AddFace(
          RotateVertex(new Vector3(shapeData.x, shapeData.y + 1, shapeData.z), direction, pivot),
          RotateVertex(new Vector3(shapeData.x, shapeData.y + 1, shapeData.z + 0.5f), direction, pivot),
          RotateVertex(new Vector3(shapeData.x + 1, shapeData.y + 1, shapeData.z + 0.5f), direction, pivot),
          RotateVertex(new Vector3(shapeData.x + 1, shapeData.y + 1, shapeData.z), direction, pivot),
          Vector3.up,
          BlockTypes.GenerateCollider(shapeData.blockType)
      );
      meshData.AddTextureFace(shapeData.textureMap.top);
      AddLighting(meshData, shapeData, LightFace.TOP);
    }

    if (ShouldRenderFace(shapeData.blockType, shapeData.bF)){
      meshData.AddFace(
          RotateVertex(new Vector3(shapeData.x + 1, shapeData.y + 0.5f, shapeData.z + 1 - 0.5f), direction, pivot),
          RotateVertex(new Vector3(shapeData.x + 1, shapeData.y + 1, shapeData.z + 1 - 0.5f), direction, pivot),
          RotateVertex(new Vector3(shapeData.x, shapeData.y + 1, shapeData.z + 1 - 0.5f), direction, pivot),
          RotateVertex(new Vector3(shapeData.x, shapeData.y + 0.5f, shapeData.z + 1 - 0.5f), direction, pivot),
          Vector3.forward,
          BlockTypes.GenerateCollider(shapeData.blockType)
      );
      meshData.AddTextureFace(shapeData.textureMap.front);
      AddLighting(meshData, shapeData, LightFace.FRONT);
    }

    if (ShouldRenderFace(shapeData.blockType, shapeData.bU)){
      meshData.AddFace(
          RotateVertex(new Vector3(shapeData.x, shapeData.y + 1 - 0.5f, shapeData.z + 0.5f), direction, pivot),
          RotateVertex(new Vector3(shapeData.x, shapeData.y + 1 - 0.5f, shapeData.z + 1), direction, pivot),
          RotateVertex(new Vector3(shapeData.x + 1, shapeData.y + 1 - 0.5f, shapeData.z + 1), direction, pivot),
          RotateVertex(new Vector3(shapeData.x + 1, shapeData.y + 1 - 0.5f, shapeData.z + 0.5f), direction, pivot),
          Vector3.up,
          BlockTypes.GenerateCollider(shapeData.blockType)
      );
      meshData.AddTextureFace(shapeData.textureMap.top);
      AddLighting(meshData, shapeData, LightFace.TOP);
    }

    if (ShouldRenderFace(shapeData.blockType, shapeData.bF)){
      meshData.AddFace(
          RotateVertex(new Vector3(shapeData.x + 1, shapeData.y, shapeData.z + 1), direction, pivot),
          RotateVertex(new Vector3(shapeData.x + 1, shapeData.y + 0.5f, shapeData.z + 1), direction, pivot),
          RotateVertex(new Vector3(shapeData.x, shapeData.y + 0.5f, shapeData.z + 1), direction, pivot),
          RotateVertex(new Vector3(shapeData.x, shapeData.y, shapeData.z + 1), direction, pivot),
          Vector3.forward,
          BlockTypes.GenerateCollider(shapeData.blockType)
      );
      meshData.AddTextureFace(shapeData.textureMap.front);
      AddLighting(meshData, shapeData, LightFace.FRONT);
    }

    if (ShouldRenderFace(shapeData.blockType, shapeData.bR)){
      meshData.AddFace(
          RotateVertex(new Vector3(shapeData.x + 1, shapeData.y, shapeData.z), direction, pivot),
          RotateVertex(new Vector3(shapeData.x + 1, shapeData.y + 0.5f, shapeData.z), direction, pivot),
          RotateVertex(new Vector3(shapeData.x + 1, shapeData.y + 0.5f, shapeData.z + 1), direction, pivot),
          RotateVertex(new Vector3(shapeData.x + 1, shapeData.y, shapeData.z + 1), direction, pivot),
          Vector3.right,
          BlockTypes.GenerateCollider(shapeData.blockType)
      );
      meshData.AddTextureFace(shapeData.textureMap.right);
      AddLighting(meshData, shapeData, LightFace.RIGHT);
    }

    if (ShouldRenderFace(shapeData.blockType, shapeData.bR)){
      meshData.AddFace(
          RotateVertex(new Vector3(shapeData.x + 1, shapeData.y + 0.5f, shapeData.z), direction, pivot),
          RotateVertex(new Vector3(shapeData.x + 1, shapeData.y + 0.5f + 0.5f, shapeData.z), direction, pivot),
          RotateVertex(new Vector3(shapeData.x + 1, shapeData.y + 0.5f + 0.5f, shapeData.z + 0.5f), direction, pivot),
          RotateVertex(new Vector3(shapeData.x + 1, shapeData.y + 0.5f, shapeData.z + 0.5f), direction, pivot),
          Vector3.right,
          BlockTypes.GenerateCollider(shapeData.blockType)
      );
      meshData.AddTextureFace(shapeData.textureMap.right);
      AddLighting(meshData, shapeData, LightFace.RIGHT);
    }

    if (ShouldRenderFace(shapeData.blockType, shapeData.bL)){
      meshData.AddFace(
          RotateVertex(new Vector3(shapeData.x, shapeData.y, shapeData.z + 1), direction, pivot),
          RotateVertex(new Vector3(shapeData.x, shapeData.y + 0.5f, shapeData.z + 1), direction, pivot),
          RotateVertex(new Vector3(shapeData.x, shapeData.y + 0.5f, shapeData.z), direction, pivot),
          RotateVertex(new Vector3(shapeData.x, shapeData.y, shapeData.z), direction, pivot),
          -Vector3.right,
          BlockTypes.GenerateCollider(shapeData.blockType)
      );
      meshData.AddTextureFace(shapeData.textureMap.left);
      AddLighting(meshData, shapeData, LightFace.LEFT);
    }
    
    if (ShouldRenderFace(shapeData.blockType, shapeData.bL)){
      meshData.AddFace(
          RotateVertex(new Vector3(shapeData.x, shapeData.y + 0.5f, shapeData.z + 0.5f), direction, pivot),
          RotateVertex(new Vector3(shapeData.x, shapeData.y + 0.5f + 0.5f, shapeData.z + 0.5f), direction, pivot),
          RotateVertex(new Vector3(shapeData.x, shapeData.y + 0.5f + 0.5f, shapeData.z), direction, pivot),
          RotateVertex(new Vector3(shapeData.x, shapeData.y + 0.5f, shapeData.z), direction, pivot),
          -Vector3.right,
          BlockTypes.GenerateCollider(shapeData.blockType)
      );
      meshData.AddTextureFace(shapeData.textureMap.left);
      AddLighting(meshData, shapeData, LightFace.LEFT);
    }

    if (ShouldRenderFace(shapeData.blockType, shapeData.bD)){
      meshData.AddFace(
          RotateVertex(new Vector3(shapeData.x, shapeData.y, shapeData.z + 1), direction, pivot),
          RotateVertex(new Vector3(shapeData.x, shapeData.y, shapeData.z), direction, pivot),
          RotateVertex(new Vector3(shapeData.x + 1, shapeData.y, shapeData.z), direction, pivot),
          RotateVertex(new Vector3(shapeData.x + 1, shapeData.y, shapeData.z + 1), direction, pivot),
          -Vector3.up,
          BlockTypes.GenerateCollider(shapeData.blockType)
      );
      meshData.AddTextureFace(shapeData.textureMap.bottom);
      AddLighting(meshData, shapeData, LightFace.BOTTOM);
    }
  }

  private static Vector3 RotateVertex(Vector3 vertex, Direction direction, Vector3 pivot){
    switch (direction){
      case Direction.EAST:
        return new Vector3(pivot.z - vertex.z + pivot.x, vertex.y, vertex.x - pivot.x + pivot.z); // 90 degrees rotation
      case Direction.SOUTH:
        return new Vector3(pivot.x - vertex.x + pivot.x, vertex.y, pivot.z - vertex.z + pivot.z); // 180 degrees rotation
      case Direction.WEST:
        return new Vector3(vertex.z - pivot.z + pivot.x, vertex.y, pivot.x - vertex.x + pivot.z); // 270 degrees rotation
      default: // North (no rotation)
        return vertex;
    }
  }


  private static void AddLighting(ChunkMeshData meshData, ShapeData shapeData, LightFace lightFace){
    int b;
    int t;

    byte sbl, stl, str, sbr;  // Sunlight corners
    byte bbl, btl, btr, bbr;  // Block light corners
    
    switch (lightFace){
      case LightFace.RIGHT:
        b = shapeData.y == 0 ? 0 : 1;
        t = shapeData.y == 255 ? 0 : 1;
        sbl = (byte)((shapeData.sunLightMap[shapeData.lx + 1, shapeData.ly, shapeData.lz] + shapeData.sunLightMap[shapeData.lx + 1, shapeData.ly, shapeData.lz - 1] + shapeData.sunLightMap[shapeData.lx + 1, shapeData.ly - b, shapeData.lz] + shapeData.sunLightMap[shapeData.lx + 1, shapeData.ly - b, shapeData.lz - 1]) / 4);
        stl = (byte)((shapeData.sunLightMap[shapeData.lx + 1, shapeData.ly, shapeData.lz] + shapeData.sunLightMap[shapeData.lx + 1, shapeData.ly, shapeData.lz - 1] + shapeData.sunLightMap[shapeData.lx + 1, shapeData.ly + t, shapeData.lz] + shapeData.sunLightMap[shapeData.lx + 1, shapeData.ly + t, shapeData.lz - 1]) / 4);
        str = (byte)((shapeData.sunLightMap[shapeData.lx + 1, shapeData.ly, shapeData.lz] + shapeData.sunLightMap[shapeData.lx + 1, shapeData.ly, shapeData.lz + 1] + shapeData.sunLightMap[shapeData.lx + 1, shapeData.ly + t, shapeData.lz] + shapeData.sunLightMap[shapeData.lx + 1, shapeData.ly + t, shapeData.lz + 1]) / 4);
        sbr = (byte)((shapeData.sunLightMap[shapeData.lx + 1, shapeData.ly, shapeData.lz] + shapeData.sunLightMap[shapeData.lx + 1, shapeData.ly, shapeData.lz + 1] + shapeData.sunLightMap[shapeData.lx + 1, shapeData.ly - b, shapeData.lz] + shapeData.sunLightMap[shapeData.lx + 1, shapeData.ly - b, shapeData.lz + 1]) / 4);
        
        bbl = (byte)((shapeData.blockLightMap[shapeData.lx + 1, shapeData.ly, shapeData.lz] + shapeData.blockLightMap[shapeData.lx + 1, shapeData.ly, shapeData.lz - 1] + shapeData.blockLightMap[shapeData.lx + 1, shapeData.ly - b, shapeData.lz] + shapeData.blockLightMap[shapeData.lx + 1, shapeData.ly - b, shapeData.lz - 1]) / 4);
        btl = (byte)((shapeData.blockLightMap[shapeData.lx + 1, shapeData.ly, shapeData.lz] + shapeData.blockLightMap[shapeData.lx + 1, shapeData.ly, shapeData.lz - 1] + shapeData.blockLightMap[shapeData.lx + 1, shapeData.ly + t, shapeData.lz] + shapeData.blockLightMap[shapeData.lx + 1, shapeData.ly + t, shapeData.lz - 1]) / 4);
        btr = (byte)((shapeData.blockLightMap[shapeData.lx + 1, shapeData.ly, shapeData.lz] + shapeData.blockLightMap[shapeData.lx + 1, shapeData.ly, shapeData.lz + 1] + shapeData.blockLightMap[shapeData.lx + 1, shapeData.ly + t, shapeData.lz] + shapeData.blockLightMap[shapeData.lx + 1, shapeData.ly + t, shapeData.lz + 1]) / 4);
        bbr = (byte)((shapeData.blockLightMap[shapeData.lx + 1, shapeData.ly, shapeData.lz] + shapeData.blockLightMap[shapeData.lx + 1, shapeData.ly, shapeData.lz + 1] + shapeData.blockLightMap[shapeData.lx + 1, shapeData.ly - b, shapeData.lz] + shapeData.blockLightMap[shapeData.lx + 1, shapeData.ly - b, shapeData.lz + 1]) / 4);
        meshData.AddColors(shapeData.textureMap, sbl, stl, str, sbr, bbl, btl, btr, bbr);
        return;
      case LightFace.LEFT:
        b = (shapeData.y == 0 ? 0 : 1);
        t = (shapeData.y == 255 ? 0 : 1);
        sbr = (byte)((shapeData.sunLightMap[shapeData.lx - 1, shapeData.ly, shapeData.lz] + shapeData.sunLightMap[shapeData.lx - 1, shapeData.ly, shapeData.lz - 1] + shapeData.sunLightMap[shapeData.lx - 1, shapeData.ly - b, shapeData.lz] + shapeData.sunLightMap[shapeData.lx - 1, shapeData.ly - b, shapeData.lz - 1]) / 4);
        str = (byte)((shapeData.sunLightMap[shapeData.lx - 1, shapeData.ly, shapeData.lz] + shapeData.sunLightMap[shapeData.lx - 1, shapeData.ly, shapeData.lz - 1] + shapeData.sunLightMap[shapeData.lx - 1, shapeData.ly + t, shapeData.lz] + shapeData.sunLightMap[shapeData.lx - 1, shapeData.ly + t, shapeData.lz - 1]) / 4);
        stl = (byte)((shapeData.sunLightMap[shapeData.lx - 1, shapeData.ly, shapeData.lz] + shapeData.sunLightMap[shapeData.lx - 1, shapeData.ly, shapeData.lz + 1] + shapeData.sunLightMap[shapeData.lx - 1, shapeData.ly + t, shapeData.lz] + shapeData.sunLightMap[shapeData.lx - 1, shapeData.ly + t, shapeData.lz + 1]) / 4);
        sbl = (byte)((shapeData.sunLightMap[shapeData.lx - 1, shapeData.ly, shapeData.lz] + shapeData.sunLightMap[shapeData.lx - 1, shapeData.ly, shapeData.lz + 1] + shapeData.sunLightMap[shapeData.lx - 1, shapeData.ly - b, shapeData.lz] + shapeData.sunLightMap[shapeData.lx - 1, shapeData.ly - b, shapeData.lz + 1]) / 4);
        
        bbr = (byte)((shapeData.blockLightMap[shapeData.lx - 1, shapeData.ly, shapeData.lz] + shapeData.blockLightMap[shapeData.lx - 1, shapeData.ly, shapeData.lz - 1] + shapeData.blockLightMap[shapeData.lx - 1, shapeData.ly - b, shapeData.lz] + shapeData.blockLightMap[shapeData.lx - 1, shapeData.ly - b, shapeData.lz - 1]) / 4);
        btr = (byte)((shapeData.blockLightMap[shapeData.lx - 1, shapeData.ly, shapeData.lz] + shapeData.blockLightMap[shapeData.lx - 1, shapeData.ly, shapeData.lz - 1] + shapeData.blockLightMap[shapeData.lx - 1, shapeData.ly + t, shapeData.lz] + shapeData.blockLightMap[shapeData.lx - 1, shapeData.ly + t, shapeData.lz - 1]) / 4);
        btl = (byte)((shapeData.blockLightMap[shapeData.lx - 1, shapeData.ly, shapeData.lz] + shapeData.blockLightMap[shapeData.lx - 1, shapeData.ly, shapeData.lz + 1] + shapeData.blockLightMap[shapeData.lx - 1, shapeData.ly + t, shapeData.lz] + shapeData.blockLightMap[shapeData.lx - 1, shapeData.ly + t, shapeData.lz + 1]) / 4);
        bbl = (byte)((shapeData.blockLightMap[shapeData.lx - 1, shapeData.ly, shapeData.lz] + shapeData.blockLightMap[shapeData.lx - 1, shapeData.ly, shapeData.lz + 1] + shapeData.blockLightMap[shapeData.lx - 1, shapeData.ly - b, shapeData.lz] + shapeData.blockLightMap[shapeData.lx - 1, shapeData.ly - b, shapeData.lz + 1]) / 4);
        meshData.AddColors(shapeData.textureMap, sbl, stl, str, sbr, bbl, btl, btr, bbr);
        return;
      case LightFace.TOP:
        b = (shapeData.y == 0 ? 0 : 1);
        t = (shapeData.y == 255 ? 0 : 1);
        sbl = (byte)((shapeData.sunLightMap[shapeData.lx, shapeData.ly + t, shapeData.lz] + shapeData.sunLightMap[shapeData.lx - 1, shapeData.ly + t, shapeData.lz] + shapeData.sunLightMap[shapeData.lx, shapeData.ly + t, shapeData.lz - 1] + shapeData.sunLightMap[shapeData.lx - 1, shapeData.ly + t, shapeData.lz - 1]) / 4);
        stl = (byte)((shapeData.sunLightMap[shapeData.lx, shapeData.ly + t, shapeData.lz] + shapeData.sunLightMap[shapeData.lx - 1, shapeData.ly + t, shapeData.lz] + shapeData.sunLightMap[shapeData.lx, shapeData.ly + t, shapeData.lz + 1] + shapeData.sunLightMap[shapeData.lx - 1, shapeData.ly + t, shapeData.lz + 1]) / 4);
        str = (byte)((shapeData.sunLightMap[shapeData.lx, shapeData.ly + t, shapeData.lz] + shapeData.sunLightMap[shapeData.lx + 1, shapeData.ly + t, shapeData.lz] + shapeData.sunLightMap[shapeData.lx, shapeData.ly + t, shapeData.lz + 1] + shapeData.sunLightMap[shapeData.lx + 1, shapeData.ly + t, shapeData.lz + 1]) / 4);
        sbr = (byte)((shapeData.sunLightMap[shapeData.lx, shapeData.ly + t, shapeData.lz] + shapeData.sunLightMap[shapeData.lx + 1, shapeData.ly + t, shapeData.lz] + shapeData.sunLightMap[shapeData.lx, shapeData.ly + t, shapeData.lz - 1] + shapeData.sunLightMap[shapeData.lx + 1, shapeData.ly + t, shapeData.lz - 1]) / 4);
        
        bbl = (byte)((shapeData.blockLightMap[shapeData.lx, shapeData.ly + t, shapeData.lz] + shapeData.blockLightMap[shapeData.lx - 1, shapeData.ly + t, shapeData.lz] + shapeData.blockLightMap[shapeData.lx, shapeData.ly + t, shapeData.lz - 1] + shapeData.blockLightMap[shapeData.lx - 1, shapeData.ly + t, shapeData.lz - 1]) / 4);
        btl = (byte)((shapeData.blockLightMap[shapeData.lx, shapeData.ly + t, shapeData.lz] + shapeData.blockLightMap[shapeData.lx - 1, shapeData.ly + t, shapeData.lz] + shapeData.blockLightMap[shapeData.lx, shapeData.ly + t, shapeData.lz + 1] + shapeData.blockLightMap[shapeData.lx - 1, shapeData.ly + t, shapeData.lz + 1]) / 4);
        btr = (byte)((shapeData.blockLightMap[shapeData.lx, shapeData.ly + t, shapeData.lz] + shapeData.blockLightMap[shapeData.lx + 1, shapeData.ly + t, shapeData.lz] + shapeData.blockLightMap[shapeData.lx, shapeData.ly + t, shapeData.lz + 1] + shapeData.blockLightMap[shapeData.lx + 1, shapeData.ly + t, shapeData.lz + 1]) / 4);
        bbr = (byte)((shapeData.blockLightMap[shapeData.lx, shapeData.ly + t, shapeData.lz] + shapeData.blockLightMap[shapeData.lx + 1, shapeData.ly + t, shapeData.lz] + shapeData.blockLightMap[shapeData.lx, shapeData.ly + t, shapeData.lz - 1] + shapeData.blockLightMap[shapeData.lx + 1, shapeData.ly + t, shapeData.lz - 1]) / 4);
        meshData.AddColors(shapeData.textureMap, sbl, stl, str, sbr, bbl, btl, btr, bbr);
        return;
      case LightFace.BOTTOM:
        b = (shapeData.y == 0 ? 0 : 1);
        t = (shapeData.y == 255 ? 0 : 1);
        stl = (byte)((shapeData.sunLightMap[shapeData.lx, shapeData.ly - b, shapeData.lz] + shapeData.sunLightMap[shapeData.lx - 1, shapeData.ly - b, shapeData.lz] + shapeData.sunLightMap[shapeData.lx, shapeData.ly - b, shapeData.lz - 1] + shapeData.sunLightMap[shapeData.lx - 1, shapeData.ly - b, shapeData.lz - 1]) / 4);
        sbl = (byte)((shapeData.sunLightMap[shapeData.lx, shapeData.ly - b, shapeData.lz] + shapeData.sunLightMap[shapeData.lx - 1, shapeData.ly - b, shapeData.lz] + shapeData.sunLightMap[shapeData.lx, shapeData.ly - b, shapeData.lz + 1] + shapeData.sunLightMap[shapeData.lx - 1, shapeData.ly - b, shapeData.lz + 1]) / 4);
        sbr = (byte)((shapeData.sunLightMap[shapeData.lx, shapeData.ly - b, shapeData.lz] + shapeData.sunLightMap[shapeData.lx + 1, shapeData.ly - b, shapeData.lz] + shapeData.sunLightMap[shapeData.lx, shapeData.ly - b, shapeData.lz + 1] + shapeData.sunLightMap[shapeData.lx + 1, shapeData.ly - b, shapeData.lz + 1]) / 4);
        str = (byte)((shapeData.sunLightMap[shapeData.lx, shapeData.ly - b, shapeData.lz] + shapeData.sunLightMap[shapeData.lx + 1, shapeData.ly - b, shapeData.lz] + shapeData.sunLightMap[shapeData.lx, shapeData.ly - b, shapeData.lz - 1] + shapeData.sunLightMap[shapeData.lx + 1, shapeData.ly - b, shapeData.lz - 1]) / 4);
        
        btl = (byte)((shapeData.blockLightMap[shapeData.lx, shapeData.ly - b, shapeData.lz] + shapeData.blockLightMap[shapeData.lx - 1, shapeData.ly - b, shapeData.lz] + shapeData.blockLightMap[shapeData.lx, shapeData.ly - b, shapeData.lz - 1] + shapeData.blockLightMap[shapeData.lx - 1, shapeData.ly - b, shapeData.lz - 1]) / 4);
        bbl = (byte)((shapeData.blockLightMap[shapeData.lx, shapeData.ly - b, shapeData.lz] + shapeData.blockLightMap[shapeData.lx - 1, shapeData.ly - b, shapeData.lz] + shapeData.blockLightMap[shapeData.lx, shapeData.ly - b, shapeData.lz + 1] + shapeData.blockLightMap[shapeData.lx - 1, shapeData.ly - b, shapeData.lz + 1]) / 4);
        bbr = (byte)((shapeData.blockLightMap[shapeData.lx, shapeData.ly - b, shapeData.lz] + shapeData.blockLightMap[shapeData.lx + 1, shapeData.ly - b, shapeData.lz] + shapeData.blockLightMap[shapeData.lx, shapeData.ly - b, shapeData.lz + 1] + shapeData.blockLightMap[shapeData.lx + 1, shapeData.ly - b, shapeData.lz + 1]) / 4);
        btr = (byte)((shapeData.blockLightMap[shapeData.lx, shapeData.ly - b, shapeData.lz] + shapeData.blockLightMap[shapeData.lx + 1, shapeData.ly - b, shapeData.lz] + shapeData.blockLightMap[shapeData.lx, shapeData.ly - b, shapeData.lz - 1] + shapeData.blockLightMap[shapeData.lx + 1, shapeData.ly - b, shapeData.lz - 1]) / 4);
        meshData.AddColors(shapeData.textureMap, sbl, stl, str, sbr, bbl, btl, btr, bbr);
        return;
      case LightFace.FRONT:
        b = (shapeData.y == 0 ? 0 : 1);
        t = (shapeData.y == 255 ? 0 : 1);
        sbr = (byte)((shapeData.sunLightMap[shapeData.lx, shapeData.ly, shapeData.lz + 1] + shapeData.sunLightMap[shapeData.lx - 1, shapeData.ly, shapeData.lz + 1] + shapeData.sunLightMap[shapeData.lx, shapeData.ly - b, shapeData.lz + 1] + shapeData.sunLightMap[shapeData.lx - 1, shapeData.ly - b, shapeData.lz + 1]) / 4);
        str = (byte)((shapeData.sunLightMap[shapeData.lx, shapeData.ly, shapeData.lz + 1] + shapeData.sunLightMap[shapeData.lx - 1, shapeData.ly, shapeData.lz + 1] + shapeData.sunLightMap[shapeData.lx, shapeData.ly + t, shapeData.lz + 1] + shapeData.sunLightMap[shapeData.lx - 1, shapeData.ly + t, shapeData.lz + 1]) / 4);
        stl = (byte)((shapeData.sunLightMap[shapeData.lx, shapeData.ly, shapeData.lz + 1] + shapeData.sunLightMap[shapeData.lx + 1, shapeData.ly, shapeData.lz + 1] + shapeData.sunLightMap[shapeData.lx, shapeData.ly + t, shapeData.lz + 1] + shapeData.sunLightMap[shapeData.lx + 1, shapeData.ly + t, shapeData.lz + 1]) / 4);
        sbl = (byte)((shapeData.sunLightMap[shapeData.lx, shapeData.ly, shapeData.lz + 1] + shapeData.sunLightMap[shapeData.lx + 1, shapeData.ly, shapeData.lz + 1] + shapeData.sunLightMap[shapeData.lx, shapeData.ly - b, shapeData.lz + 1] + shapeData.sunLightMap[shapeData.lx + 1, shapeData.ly - b, shapeData.lz + 1]) / 4);
        
        bbr = (byte)((shapeData.blockLightMap[shapeData.lx, shapeData.ly, shapeData.lz + 1] + shapeData.blockLightMap[shapeData.lx - 1, shapeData.ly, shapeData.lz + 1] + shapeData.blockLightMap[shapeData.lx, shapeData.ly - b, shapeData.lz + 1] + shapeData.blockLightMap[shapeData.lx - 1, shapeData.ly - b, shapeData.lz + 1]) / 4);
        btr = (byte)((shapeData.blockLightMap[shapeData.lx, shapeData.ly, shapeData.lz + 1] + shapeData.blockLightMap[shapeData.lx - 1, shapeData.ly, shapeData.lz + 1] + shapeData.blockLightMap[shapeData.lx, shapeData.ly + t, shapeData.lz + 1] + shapeData.blockLightMap[shapeData.lx - 1, shapeData.ly + t, shapeData.lz + 1]) / 4);
        btl = (byte)((shapeData.blockLightMap[shapeData.lx, shapeData.ly, shapeData.lz + 1] + shapeData.blockLightMap[shapeData.lx + 1, shapeData.ly, shapeData.lz + 1] + shapeData.blockLightMap[shapeData.lx, shapeData.ly + t, shapeData.lz + 1] + shapeData.blockLightMap[shapeData.lx + 1, shapeData.ly + t, shapeData.lz + 1]) / 4);
        bbl = (byte)((shapeData.blockLightMap[shapeData.lx, shapeData.ly, shapeData.lz + 1] + shapeData.blockLightMap[shapeData.lx + 1, shapeData.ly, shapeData.lz + 1] + shapeData.blockLightMap[shapeData.lx, shapeData.ly - b, shapeData.lz + 1] + shapeData.blockLightMap[shapeData.lx + 1, shapeData.ly - b, shapeData.lz + 1]) / 4);
        meshData.AddColors(shapeData.textureMap, sbl, stl, str, sbr, bbl, btl, btr, bbr);
        return;
      case LightFace.BACK:
        b = (shapeData.y == 0 ? 0 : 1);
        t = (shapeData.y == 255 ? 0 : 1);
        sbl = (byte)((shapeData.sunLightMap[shapeData.lx, shapeData.ly, shapeData.lz - 1] + shapeData.sunLightMap[shapeData.lx - 1, shapeData.ly, shapeData.lz - 1] + shapeData.sunLightMap[shapeData.lx, shapeData.ly - b, shapeData.lz - 1] + shapeData.sunLightMap[shapeData.lx - 1, shapeData.ly - b, shapeData.lz - 1]) / 4);
        stl = (byte)((shapeData.sunLightMap[shapeData.lx, shapeData.ly, shapeData.lz - 1] + shapeData.sunLightMap[shapeData.lx - 1, shapeData.ly, shapeData.lz - 1] + shapeData.sunLightMap[shapeData.lx, shapeData.ly + t, shapeData.lz - 1] + shapeData.sunLightMap[shapeData.lx - 1, shapeData.ly + t, shapeData.lz - 1]) / 4);
        str = (byte)((shapeData.sunLightMap[shapeData.lx, shapeData.ly, shapeData.lz - 1] + shapeData.sunLightMap[shapeData.lx + 1, shapeData.ly, shapeData.lz - 1] + shapeData.sunLightMap[shapeData.lx, shapeData.ly + t, shapeData.lz - 1] + shapeData.sunLightMap[shapeData.lx + 1, shapeData.ly + t, shapeData.lz - 1]) / 4);
        sbr = (byte)((shapeData.sunLightMap[shapeData.lx, shapeData.ly, shapeData.lz - 1] + shapeData.sunLightMap[shapeData.lx + 1, shapeData.ly, shapeData.lz - 1] + shapeData.sunLightMap[shapeData.lx, shapeData.ly - b, shapeData.lz - 1] + shapeData.sunLightMap[shapeData.lx + 1, shapeData.ly - b, shapeData.lz - 1]) / 4);
        
        bbl = (byte)((shapeData.blockLightMap[shapeData.lx, shapeData.ly, shapeData.lz - 1] + shapeData.blockLightMap[shapeData.lx - 1, shapeData.ly, shapeData.lz - 1] + shapeData.blockLightMap[shapeData.lx, shapeData.ly - b, shapeData.lz - 1] + shapeData.blockLightMap[shapeData.lx - 1, shapeData.ly - b, shapeData.lz - 1]) / 4);
        btl = (byte)((shapeData.blockLightMap[shapeData.lx, shapeData.ly, shapeData.lz - 1] + shapeData.blockLightMap[shapeData.lx - 1, shapeData.ly, shapeData.lz - 1] + shapeData.blockLightMap[shapeData.lx, shapeData.ly + t, shapeData.lz - 1] + shapeData.blockLightMap[shapeData.lx - 1, shapeData.ly + t, shapeData.lz - 1]) / 4);
        btr = (byte)((shapeData.blockLightMap[shapeData.lx, shapeData.ly, shapeData.lz - 1] + shapeData.blockLightMap[shapeData.lx + 1, shapeData.ly, shapeData.lz - 1] + shapeData.blockLightMap[shapeData.lx, shapeData.ly + t, shapeData.lz - 1] + shapeData.blockLightMap[shapeData.lx + 1, shapeData.ly + t, shapeData.lz - 1]) / 4);
        bbr = (byte)((shapeData.blockLightMap[shapeData.lx, shapeData.ly, shapeData.lz - 1] + shapeData.blockLightMap[shapeData.lx + 1, shapeData.ly, shapeData.lz - 1] + shapeData.blockLightMap[shapeData.lx, shapeData.ly - b, shapeData.lz - 1] + shapeData.blockLightMap[shapeData.lx + 1, shapeData.ly - b, shapeData.lz - 1]) / 4);
        meshData.AddColors(shapeData.textureMap, sbl, stl, str, sbr, bbl, btl, btr, bbr);
        return;
      default:
        throw new ArgumentOutOfRangeException(nameof(LightFace), lightFace, "Invalid Light Face Direction!");
    }
  }
  
  private static bool ShouldRenderFace(byte block, byte targetBlock){
    if (BlockTypes.IsTransparentBlock(block) && BlockTypes.IsTransparentBlock(targetBlock)){
      return block != targetBlock;
    }
    if (BlockTypes.IsTransparentCutoutBlock(block) && BlockTypes.IsTransparentCutoutBlock(targetBlock)) return false;
    if (BlockTypes.IsSlab(targetBlock)) return true;
    //TODO fix issue between chunks
    if (BlockTypes.IsSolidLiquid(block) && BlockTypes.IsSolidLiquid(targetBlock)) return false;
    if (BlockTypes.IsSolidLiquid(targetBlock)) return true;
    if (BlockTypes.IsStair(block) || BlockTypes.IsStair(targetBlock)) return true;
    return targetBlock > 127;
  }
  
  public static Direction GetStairPlaceOrientation(Direction direction) {
    switch (direction) {
      case Direction.NORTH:
        return Direction.NORTH;
      case Direction.EAST:
        return Direction.WEST;
      case Direction.SOUTH:
        return Direction.SOUTH;
      case Direction.WEST:
        return Direction.EAST;
      default:
        throw new ArgumentOutOfRangeException("Invalid orientation value");
    }
  }
}