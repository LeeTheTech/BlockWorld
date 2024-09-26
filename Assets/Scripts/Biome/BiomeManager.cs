using System.Collections.Generic;
using UnityEngine;

public static class BiomeManager{
  private static Dictionary<BiomeType, Biome> biomes;
  private static NoiseManager noiseManager;

  private static Dictionary<BiomeType, float> biomeWeights;
  private static List<(BiomeType biome, float minValue, float maxValue)> biomeRanges;

  public const int seaLevel = 100;

  public static void Initialize(int seed){
    biomes = new Dictionary<BiomeType, Biome>();
    noiseManager = new NoiseManager(seed);
    biomeWeights = new();
    biomeRanges = new();
    CreateBiomes();
  }

  private static void CreateBiomes(){
    CreateBiome(BiomeType.GRASSLAND);
    CreateBiome(BiomeType.DESERT);
    CreateBiome(BiomeType.MOUNTAIN);
    CreateBiome(BiomeType.SNOW);
    AssignBiomeWeights();
    CalculateBiomeRanges();
  }

  private static void CreateBiome(BiomeType biomeType){
    switch (biomeType){
      case BiomeType.GRASSLAND:
        biomes[BiomeType.GRASSLAND] = new Biome(BiomeType.GRASSLAND, new BlockLayers(BlockTypes.GRASS, BlockTypes.DIRT, BlockTypes.STONE));
        break;
      case BiomeType.DESERT:
        biomes[BiomeType.DESERT] = new Biome(BiomeType.DESERT, new BlockLayers(BlockTypes.SAND, BlockTypes.SAND, BlockTypes.STONE));
        break;
      case BiomeType.MOUNTAIN:
        biomes[BiomeType.MOUNTAIN] = new Biome(BiomeType.MOUNTAIN, new BlockLayers(BlockTypes.GRASS, BlockTypes.DIRT, BlockTypes.STONE));
        break;
      case BiomeType.SNOW:
        biomes[BiomeType.SNOW] = new Biome(BiomeType.SNOW, new BlockLayers(BlockTypes.SNOW_GRASS, BlockTypes.DIRT, BlockTypes.STONE));
        break;
    }
  }
  
  private static void AssignBiomeWeights(){
    // Define weights for each biome - tweak these values for balance
    biomeWeights[BiomeType.GRASSLAND] = 0.25f;  // 25% of the world
    biomeWeights[BiomeType.DESERT] = 0.25f;     // 25% of the world
    biomeWeights[BiomeType.MOUNTAIN] = 0.25f;   // 25% of the world
    biomeWeights[BiomeType.SNOW] = 0.25f;       // 25% of the world
  }
  
  private static void CalculateBiomeRanges(){
    // This function calculates the min and max values for each biome range
    float cumulativeWeight = 0f;

    foreach (var biome in biomeWeights){
      float minValue = cumulativeWeight;
      cumulativeWeight += biome.Value;  // Increment cumulative weight
      float maxValue = cumulativeWeight;

      biomeRanges.Add((biome.Key, minValue, maxValue));
    }
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
    // Use domain warping to get more scattered and interesting biome borders
    float warpX = noiseManager.domainWarpNoiseX.GetNoise(x, z) * 30.0f;
    float warpZ = noiseManager.domainWarpNoiseZ.GetNoise(x, z) * 30.0f;

    float warpedX = x + warpX;
    float warpedZ = z + warpZ;

    // Get a noise value in the range of [0, 1]
    float noiseValue = Mathf.Clamp01(noiseManager.biomeNoise.GetNoise(warpedX, warpedZ));

    // Find the corresponding biome based on the noise value and biome ranges
    foreach (var range in biomeRanges){
      if (noiseValue >= range.minValue && noiseValue < range.maxValue){
        return range.biome;
      }
    }
    return BiomeType.GRASSLAND; // Default fallback
  }

  public static Biome GetBiome(BiomeType biomeType){
    return biomes[biomeType];
  }

  public static byte DetermineBlockType(int heightGen, int x, int y, int z){
    if (y == 0){
      return BlockTypes.BEDROCK;
    }
    if (y == 1){
      return BlockTypes.LAVA;
    }

    // Check for cave generation
    if (y < seaLevel - 30){
      float caveNoiseValue = noiseManager.caveNoise.GetNoise(x, y, z);
    
      // If caveNoiseValue is below threshold, this is a cave block
      if (caveNoiseValue < 0.0009f) {
        // Water check
        if (y + 2 <= heightGen){
          return BlockTypes.AIR;
        }
      }
    }
    
    // Ore generation based on depth and noise
    if (y < heightGen - 4) {
      byte oreBlock = noiseManager.oreNoiseManager.GetOreBlock(x, y, z);
      if (oreBlock != BlockTypes.AIR) return oreBlock;
    }
    
    Biome biome = GetBiome(GetBiomeType(x, z));

    // Water 
    if (y > heightGen && y <= seaLevel - 1){
      return biome.biomeType == BiomeType.SNOW ? BlockTypes.ICE : BlockTypes.WATER;
    }

    // Beach
    if (y == heightGen && y == seaLevel - 1){
      return BlockTypes.SAND; // Sand block
    }
    
    // Beach into water
    if (y < seaLevel && y >= seaLevel - 4) {
      return BlockTypes.SAND; // Place gravel below water
    }
    
    if (y < seaLevel && y >= seaLevel - 10) {
      return BlockTypes.GRAVEL; // Place gravel at the bottom of any underwater area
    }
    
    if (y < seaLevel) {
      return BlockTypes.STONE; // Place gravel at the bottom of any underwater area
    }

    // General terrain handling
    if (y > heightGen){
      return BlockTypes.AIR; // Air block
    }
    
    if (y == heightGen) return biome.blockLayers.layer1; // Top layer
    if (y < heightGen && y > heightGen - 4) return biome.blockLayers.layer2; // Next layers
    if (y <= heightGen - 4 && y > 0) return biome.blockLayers.layer3; // Deeper layers 
    return BlockTypes.AIR;
  }
}