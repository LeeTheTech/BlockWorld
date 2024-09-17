using System.Collections.Generic;
using UnityEngine;

public static class BiomeManager{
  private static Dictionary<BiomeType, Biome> biomes;
  private static NoiseManager noiseManager;

  public const int seaLevel = 100;

  public static void Initialize(int seed){
    biomes = new Dictionary<BiomeType, Biome>();
    noiseManager = new NoiseManager(seed);
    CreateBiomes();
  }

  private static void CreateBiomes(){
    biomes.Add(BiomeType.GRASSLAND, new Biome(BiomeType.GRASSLAND, new BlockLayers(BlockTypes.GRASS, BlockTypes.DIRT, BlockTypes.STONE)));
    biomes.Add(BiomeType.DESERT, new Biome(BiomeType.DESERT, new BlockLayers(BlockTypes.SAND, BlockTypes.SAND, BlockTypes.STONE)));
    biomes.Add(BiomeType.MOUNTAIN, new Biome(BiomeType.MOUNTAIN, new BlockLayers(BlockTypes.GRASS, BlockTypes.DIRT, BlockTypes.STONE)));
    biomes.Add(BiomeType.SNOW, new Biome(BiomeType.SNOW, new BlockLayers(BlockTypes.GRASS, BlockTypes.DIRT, BlockTypes.STONE)));
  }

  public static float GenerateTerrainHeight(int x, int z){
    const int interpolationRadius = 5;
    float totalHeight = 0f;
    float totalWeight = 0f;
    
    // VERY SLOW but works
    // Sample heights from neighboring points within the radius
    for (int dx = -interpolationRadius; dx <= interpolationRadius; dx++){
      for (int dz = -interpolationRadius; dz <= interpolationRadius; dz++){
        float neighborX = x + dx;
        float neighborZ = z + dz;
        float weight = 1.0f / (Mathf.Sqrt(dx * dx + dz * dz) + 1); // Weight based on distance
    
        float neighborHeight = noiseManager.noiseLayerManager.GenerateHeight(GetBiomeType(neighborX, neighborZ), neighborX, neighborZ);
    
        totalHeight += neighborHeight * weight;
        totalWeight += weight;
      }
    }
    
    return totalHeight / totalWeight;
  }

  public static BiomeType GetBiomeType(float x, float z){
    float warpX = noiseManager.domainWarpNoiseX.GetNoise(x, z) * 30.0f;
    float warpZ = noiseManager.domainWarpNoiseZ.GetNoise(x, z) * 30.0f;

    float warpedX = x + warpX;
    float warpedZ = z + warpZ;

    float noiseValue = noiseManager.biomeNoise.GetNoise(warpedX, warpedZ);
    return noiseValue switch {
        < 0.33f => BiomeType.DESERT,
        < 0.66f => BiomeType.GRASSLAND,
        < 0.88f => BiomeType.MOUNTAIN,
        < 0.99f => BiomeType.SNOW,
        _ => BiomeType.DESERT
    };
  }

  public static Biome GetBiome(BiomeType biomeType){
    return biomes[biomeType];
  }

  public static byte DetermineBlockType(int heightGen, int x, int y, int z){
    if (y == 0){
      return BlockTypes.BEDROCK;
    }

    // Check for cave generation
    if (y < seaLevel - 10){
      float caveNoiseValue = noiseManager.caveNoise.GetNoise(x, y, z);
    
      // If caveNoiseValue is below threshold, this is a cave block
      if (caveNoiseValue < 0.0009f) {
        // Water check
        if (y <= heightGen){
          return BlockTypes.AIR;
        }
      }
    }
    
    // Ore generation based on depth and noise
    if (y < heightGen - 4) {
      byte oreBlock = noiseManager.oreNoiseManager.GetOreBlock(x, y, z);
      if (oreBlock != BlockTypes.AIR) return oreBlock;
    }

    // Ocean 
    if (y > heightGen && y <= seaLevel){
      return BlockTypes.WATER; // Water block
    }

    // Beach
    if (y == heightGen && y == seaLevel){
      return BlockTypes.SAND; // Sand block
    }

    // General terrain handling
    if (y > heightGen){
      return BlockTypes.AIR; // Air block
    }
    
    Biome biome = GetBiome(GetBiomeType(x, z));
    if (y == heightGen) return biome.blockLayers.layer1; // Top layer
    if (y < heightGen && y > heightGen - 4) return biome.blockLayers.layer2; // Next layers
    if (y <= heightGen - 4 && y > 0) return biome.blockLayers.layer3; // Deeper layers 
    return BlockTypes.AIR;
  }
}