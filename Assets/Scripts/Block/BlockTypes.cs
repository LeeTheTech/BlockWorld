using System.Collections.Generic;

public static class BlockTypes{
  //SOLID
  public const byte BEDROCK = 0;
  public const byte GRASS = 1;
  public const byte DIRT = 2;
  public const byte STONE = 3;
  public const byte COBBLESTONE = 4;
  public const byte COAL = 5;
  public const byte IRON = 6;
  public const byte GOLD = 7;
  public const byte DIAMOND = 8;
  public const byte LOG_OAK = 9;
  public const byte PLANKS_OAK = 10;
  public const byte GLOWSTONE = 11;
  public const byte DIORITE = 12;
  public const byte GRANITE = 13;
  public const byte ANDESITE = 14;
  public const byte SAND = 15;
  public const byte CACTUS = 17;
  public const byte PLANKS_OAK_SLAB = 18;
  public const byte PLANKS_OAK_STAIR = 19;
  public const byte TORCH = 20;
  public const byte SNOW_GRASS = 21;
  public const byte LAVA = 22;
  public const byte TNT = 23;
  public const byte GRAVEL = 24;

  //TRANSPARENT
  public const byte LEAVES_OAK = 128;
  public const byte GLASS = 129;
  public const byte WATER = 130;
  public const byte ICE = 131;
  public const byte FOLIAGE = 132;
  public const byte POPPY = 133;
  public const byte FIRE = 134;
  public const byte AIR = 255;

  public static Dictionary<byte, byte> lightLevel;
  public static Dictionary<byte, byte> density;
  public static Dictionary<byte, AudioManager.Dig.Type> digSound;
  public static Dictionary<byte, BlockShapes.BlockShape> blockShape;

  public static void Initialize(){
    lightLevel = new Dictionary<byte, byte>{
        { BEDROCK, 0 },
        { GRASS, 0 },
        { DIRT, 0 },
        { STONE, 0 },
        { COAL, 0 },
        { IRON, 0 },
        { GOLD, 0 },
        { DIAMOND, 0 },
        { LOG_OAK, 0 },
        { PLANKS_OAK, 0 },
        { GLOWSTONE, 14 },
        { LEAVES_OAK, 0 },
        { GLASS, 0 },
        { WATER, 0 },
        { ICE, 0 },
        { CACTUS, 0 },
        { AIR, 0 },
        { ANDESITE, 0 },
        { DIORITE, 0 },
        { GRANITE, 0 },
        { COBBLESTONE, 0 },
        { SAND, 0 },
        { PLANKS_OAK_SLAB, 0 },
        { PLANKS_OAK_STAIR, 0 },
        { TORCH, 0 },
        { SNOW_GRASS, 0 },
        { LAVA, 15 },
        { FOLIAGE, 0 },
        { TNT, 0 },
        { GRAVEL, 0 },
        { POPPY, 0 },
        { FIRE, 15 }
    };

    density = new Dictionary<byte, byte>{
        { BEDROCK, 255 },
        { GRASS, 255 },
        { DIRT, 255 },
        { STONE, 255 },
        { COAL, 255 },
        { IRON, 255 },
        { GOLD, 255 },
        { DIAMOND, 255 },
        { LOG_OAK, 255 },
        { PLANKS_OAK, 255 },
        { GLOWSTONE, 255 },
        { LEAVES_OAK, 0 },
        { GLASS, 0 },
        { WATER, 63 },
        { ICE, 63 },
        { CACTUS, 255 },
        { AIR, 0 },
        { ANDESITE, 255 },
        { DIORITE, 255 },
        { GRANITE, 255 },
        { COBBLESTONE, 255 },
        { SAND, 255 },
        { PLANKS_OAK_SLAB, 255 },
        { PLANKS_OAK_STAIR, 255 },
        { TORCH, 255 },
        { SNOW_GRASS, 255 },
        { LAVA, 255 },
        { FOLIAGE, 60 },
        { TNT, 255 },
        { GRAVEL, 255 },
        { POPPY, 255 },
        { FIRE, 255 }
    };

    digSound = new Dictionary<byte, AudioManager.Dig.Type>{
        { BEDROCK, AudioManager.Dig.Type.Stone },
        { GRASS, AudioManager.Dig.Type.Grass },
        { DIRT, AudioManager.Dig.Type.Gravel },
        { STONE, AudioManager.Dig.Type.Stone },
        { COAL, AudioManager.Dig.Type.Stone },
        { IRON, AudioManager.Dig.Type.Stone },
        { GOLD, AudioManager.Dig.Type.Stone },
        { DIAMOND, AudioManager.Dig.Type.Stone },
        { LOG_OAK, AudioManager.Dig.Type.Wood },
        { PLANKS_OAK, AudioManager.Dig.Type.Wood },
        { GLOWSTONE, AudioManager.Dig.Type.Stone },
        { LEAVES_OAK, AudioManager.Dig.Type.Grass },
        { GLASS, AudioManager.Dig.Type.Grass },
        { WATER, AudioManager.Dig.Type.Grass },
        { ICE, AudioManager.Dig.Type.Grass },
        { CACTUS, AudioManager.Dig.Type.Grass },
        { AIR, AudioManager.Dig.Type.Silent },
        { ANDESITE, AudioManager.Dig.Type.Stone },
        { DIORITE, AudioManager.Dig.Type.Stone },
        { GRANITE, AudioManager.Dig.Type.Stone },
        { COBBLESTONE, AudioManager.Dig.Type.Stone },
        { SAND, AudioManager.Dig.Type.Stone },
        { PLANKS_OAK_SLAB, AudioManager.Dig.Type.Stone },
        { PLANKS_OAK_STAIR, AudioManager.Dig.Type.Stone },
        { TORCH, AudioManager.Dig.Type.Stone },
        { SNOW_GRASS, AudioManager.Dig.Type.Grass },
        { LAVA, AudioManager.Dig.Type.Grass },
        { FOLIAGE, AudioManager.Dig.Type.Grass },
        { TNT, AudioManager.Dig.Type.Grass },
        { GRAVEL, AudioManager.Dig.Type.Grass },
        { POPPY, AudioManager.Dig.Type.Grass },
        { FIRE, AudioManager.Dig.Type.Grass }
    };

    blockShape = new Dictionary<byte, BlockShapes.BlockShape>{
        { BEDROCK, BlockShapes.BlockShape.CUBE },
        { GRASS, BlockShapes.BlockShape.CUBE },
        { DIRT, BlockShapes.BlockShape.CUBE },
        { STONE, BlockShapes.BlockShape.CUBE },
        { COAL, BlockShapes.BlockShape.CUBE },
        { IRON, BlockShapes.BlockShape.CUBE },
        { GOLD, BlockShapes.BlockShape.CUBE },
        { DIAMOND, BlockShapes.BlockShape.CUBE },
        { LOG_OAK, BlockShapes.BlockShape.CUBE },
        { PLANKS_OAK, BlockShapes.BlockShape.CUBE },
        { GLOWSTONE, BlockShapes.BlockShape.CUBE },
        { LEAVES_OAK, BlockShapes.BlockShape.CUBE },
        { GLASS, BlockShapes.BlockShape.CUBE },
        { ICE, BlockShapes.BlockShape.CUBE },
        { CACTUS, BlockShapes.BlockShape.CUBE },
        { AIR, BlockShapes.BlockShape.CUBE },
        { ANDESITE, BlockShapes.BlockShape.CUBE },
        { DIORITE, BlockShapes.BlockShape.CUBE },
        { GRANITE, BlockShapes.BlockShape.CUBE },
        { COBBLESTONE, BlockShapes.BlockShape.CUBE },
        { SAND, BlockShapes.BlockShape.CUBE },
        { PLANKS_OAK_SLAB, BlockShapes.BlockShape.SLAB },
        { PLANKS_OAK_STAIR, BlockShapes.BlockShape.STAIR },
        { TORCH, BlockShapes.BlockShape.TORCH },
        { SNOW_GRASS, BlockShapes.BlockShape.CUBE },
        { LAVA, BlockShapes.BlockShape.CUBE },
        { WATER, BlockShapes.BlockShape.CUBE },
        { FOLIAGE, BlockShapes.BlockShape.FOLIAGE },
        { TNT, BlockShapes.BlockShape.CUBE },
        { GRAVEL, BlockShapes.BlockShape.CUBE },
        { POPPY, BlockShapes.BlockShape.FOLIAGE },
        { FIRE, BlockShapes.BlockShape.FIRE }
    };
  }

  public static bool IsTransparentBlock(byte blockType){
    switch (blockType){
      case WATER:
      case ICE:
        return true;
      default:
        return false;
    }
  }
  
  public static bool IsTransparentCutoutBlock(byte blockType){
    switch (blockType){
      case LEAVES_OAK:
      case GLASS:
        return true;
      default:
        return false;
    }
  }
  
  public static bool IsNoCullCutoutBlock(byte blockType){
    switch (blockType){
      case FOLIAGE:
        return true;
      default:
        return false;
    }
  }

  public static bool IsSlab(byte blockType){
    switch (blockType){
      case PLANKS_OAK_SLAB:
        return true;
      default:
        return false;
    }
  }
  
  public static bool IsStair(byte blockType){
    switch (blockType){
      case PLANKS_OAK_STAIR:
        return true;
      default:
        return false;
    }
  }

  public static bool IsSolidLiquid(byte blockType){
    switch (blockType){
      case LAVA:
        return true;
      default:
        return false;
    }
  }

  public static bool IsLiquid(byte blockType){
    switch (blockType){
      case LAVA:
      case WATER:
        return true;
      default:
        return false;
    }
  }

  public static bool IsExplosive(byte blockType){
    switch (blockType){
      case TNT:
        return true;
      default:
        return false;
    }
  }

  public static bool GenerateCollider(byte blockType){
    switch (blockType){
      case POPPY:
      case FOLIAGE:
      case LAVA:
      case WATER:
      case FIRE:
        return false;
      default:
        return true;
    }
  }
}