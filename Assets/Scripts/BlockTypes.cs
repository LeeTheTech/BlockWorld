﻿using System.Collections.Generic;

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

  //TRANSPARENT
  public const byte LEAVES_OAK = 128;
  public const byte GLASS = 129;
  public const byte WATER = 130;
  public const byte AIR = 255;

  public static Dictionary<byte, byte> lightLevel;
  public static Dictionary<byte, byte> density;
  public static Dictionary<byte, AudioManager.Dig.Type> digSound;

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
        { CACTUS, 0 },
        { AIR, 0 },
        { ANDESITE, 0 },
        { DIORITE, 0 },
        { GRANITE, 0 },
        { COBBLESTONE, 0 },
        { SAND, 0 }
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
        { LEAVES_OAK, 63 },
        { GLASS, 0 },
        { WATER, 63 },
        { CACTUS, 255 },
        { AIR, 0 },
        { ANDESITE, 255 },
        { DIORITE, 255 },
        { GRANITE, 255 },
        { COBBLESTONE, 255 },
        { SAND, 255 }
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
        { CACTUS, AudioManager.Dig.Type.Grass },
        { AIR, AudioManager.Dig.Type.Silent },
        { ANDESITE, AudioManager.Dig.Type.Stone },
        { DIORITE, AudioManager.Dig.Type.Stone },
        { GRANITE, AudioManager.Dig.Type.Stone },
        { COBBLESTONE, AudioManager.Dig.Type.Stone },
        { SAND, AudioManager.Dig.Type.Stone }
    };
  }
}