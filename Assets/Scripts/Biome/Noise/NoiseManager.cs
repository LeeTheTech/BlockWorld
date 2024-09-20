public class NoiseManager{
  public readonly OreNoiseManager oreNoiseManager;
  public readonly NoiseLayerManager noiseLayerManager = new();
  
  public readonly FastNoiseLite biomeNoise = new();
  public readonly FastNoiseLite caveNoise = new();
  
  public readonly FastNoiseLite domainWarpNoiseX = new();
  public readonly FastNoiseLite domainWarpNoiseZ = new();

  public NoiseManager(int seed){
    oreNoiseManager = new OreNoiseManager(seed);
    CreateDomainWarpNoise(seed);
    CreateCaveNoise(seed);
    CreateBiomeNoise(seed);
    CreateBiomePlainsNoise(seed);
    CreateBiomeDesertNoise(seed);
    CreateBiomeMountainsNoise(seed);
    CreateBiomeSnowNoise(seed);
  }
  
  private void CreateCaveNoise(int seed){
    caveNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
    caveNoise.SetFrequency(0.02f);
    caveNoise.SetSeed(seed);
  }
  
  private void CreateBiomeNoise(int seed){
    // Use Cellular noise for distinct biome patches
    biomeNoise.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
    biomeNoise.SetFrequency(0.005f);  // Adjust frequency for biome patch size (higher = smaller patches)

    // Cellular noise configuration (for biome cell distinctness)
    biomeNoise.SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.Euclidean);
    biomeNoise.SetCellularReturnType(FastNoiseLite.CellularReturnType.CellValue);  // Use cell value for distinct patches

    // Seed for consistent biome generation
    biomeNoise.SetSeed(seed);
  }
  
  private void CreateDomainWarpNoise(int seed){
    // Configure domain warping for the X axis
    domainWarpNoiseX.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
    domainWarpNoiseX.SetFrequency(0.01f); // Adjust the frequency for how chaotic the warping should be
    domainWarpNoiseX.SetFractalOctaves(3); // More octaves for more detailed warping
    domainWarpNoiseX.SetSeed(seed);

    // Configure domain warping for the Z axis
    domainWarpNoiseZ.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
    domainWarpNoiseZ.SetFrequency(0.01f);
    domainWarpNoiseZ.SetFractalOctaves(3);
    domainWarpNoiseZ.SetSeed(seed);
  }
  
  private void CreateBiomePlainsNoise(int seed){
    // Base terrain layer
    noiseLayerManager.AddLayer(BiomeType.GRASSLAND, new NoiseLayer(FastNoiseLite.NoiseType.Perlin, 0.000001f, 1f, NoiseBlendMode.ADD, seed));

    // Ground
    noiseLayerManager.AddLayer(BiomeType.GRASSLAND, new NoiseLayer(FastNoiseLite.NoiseType.Perlin, 0.1f, 5f, NoiseBlendMode.MAX, seed));

    // Hill
    noiseLayerManager.AddLayer(BiomeType.GRASSLAND, new NoiseLayer(FastNoiseLite.NoiseType.Perlin, 0.008f, 50f, NoiseBlendMode.MAX, seed));
    
    // Hill
    noiseLayerManager.AddLayer(BiomeType.GRASSLAND, new NoiseLayer(FastNoiseLite.NoiseType.Perlin, 0.007f, 20f, NoiseBlendMode.MAX, seed));

    // Lake layer
    noiseLayerManager.AddLayer(BiomeType.GRASSLAND, new NoiseLayer(FastNoiseLite.NoiseType.Perlin, 0.003f, -50f, NoiseBlendMode.MIN, seed));
  }

  private void CreateBiomeDesertNoise(int seed){
    // Base terrain layer
    noiseLayerManager.AddLayer(BiomeType.DESERT, new NoiseLayer(FastNoiseLite.NoiseType.Perlin, 0.002f, 2, NoiseBlendMode.ADD, seed));
    
    // Hills
    noiseLayerManager.AddLayer(BiomeType.DESERT, new NoiseLayer(FastNoiseLite.NoiseType.Perlin, 0.02f, 5, NoiseBlendMode.MAX, seed));
    
    // Bigger Hills
    noiseLayerManager.AddLayer(BiomeType.DESERT, new NoiseLayer(FastNoiseLite.NoiseType.Perlin, 0.05f, 10, NoiseBlendMode.MAX, seed));
  }
  
  private void CreateBiomeMountainsNoise(int seed){
    // Base terrain layer
    noiseLayerManager.AddLayer(BiomeType.MOUNTAIN, new NoiseLayer(FastNoiseLite.NoiseType.Perlin, 0.000001f, 2f, NoiseBlendMode.ADD, seed));
    // Ground
    noiseLayerManager.AddLayer(BiomeType.MOUNTAIN, new NoiseLayer(FastNoiseLite.NoiseType.Perlin, 0.01f, 10f, NoiseBlendMode.MAX, seed));
    // Mountains
    noiseLayerManager.AddLayer(BiomeType.MOUNTAIN, new NoiseLayer(FastNoiseLite.NoiseType.Perlin, 0.005f, 100, NoiseBlendMode.MAX, seed));
    noiseLayerManager.AddLayer(BiomeType.MOUNTAIN, new NoiseLayer(FastNoiseLite.NoiseType.Perlin, 0.004f, 80, NoiseBlendMode.MAX, seed));
  }
  
  private void CreateBiomeSnowNoise(int seed){
    // Base terrain layer
    noiseLayerManager.AddLayer(BiomeType.SNOW, new NoiseLayer(FastNoiseLite.NoiseType.Perlin, 0.000001f, 1f, NoiseBlendMode.ADD, seed));

    // Ground
    noiseLayerManager.AddLayer(BiomeType.SNOW, new NoiseLayer(FastNoiseLite.NoiseType.Perlin, 0.1f, 5f, NoiseBlendMode.MAX, seed));

    // Hill
    noiseLayerManager.AddLayer(BiomeType.SNOW, new NoiseLayer(FastNoiseLite.NoiseType.Perlin, 0.009f, 70f, NoiseBlendMode.MAX, seed));
    
    // Hill
    noiseLayerManager.AddLayer(BiomeType.SNOW, new NoiseLayer(FastNoiseLite.NoiseType.Perlin, 0.008f, 100, NoiseBlendMode.ADD, seed));
    
    // Lake layer
    noiseLayerManager.AddLayer(BiomeType.GRASSLAND, new NoiseLayer(FastNoiseLite.NoiseType.Perlin, 0.005f, -20f, NoiseBlendMode.MIN, seed));
  }
}
