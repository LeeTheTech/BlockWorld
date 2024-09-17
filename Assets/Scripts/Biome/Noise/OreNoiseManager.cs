using System.Collections.Generic;

public class OreNoiseManager{
  private readonly Dictionary<byte, OreNoiseData> oreNoiseMap = new();

  public OreNoiseManager(int seed){
    CreateOreNoise(seed);
  }
  
  private void CreateOreNoise(int seed){
    const float frequency = 0.05f;
    const int octaves = 3;
    const FastNoiseLite.NoiseType noiseType = FastNoiseLite.NoiseType.Perlin;
    
    FastNoiseLite diamondOreNoise = new FastNoiseLite();
    diamondOreNoise.SetNoiseType(noiseType);
    diamondOreNoise.SetFrequency(frequency);
    diamondOreNoise.SetFractalOctaves(octaves);
    diamondOreNoise.SetSeed(seed + oreNoiseMap.Count);

    oreNoiseMap[BlockTypes.DIAMOND] = new OreNoiseData(diamondOreNoise, 20, 0.7f);
    
    FastNoiseLite ironOreNoise = new FastNoiseLite();
    ironOreNoise.SetNoiseType(noiseType);
    ironOreNoise.SetFrequency(frequency);
    ironOreNoise.SetFractalOctaves(octaves);
    ironOreNoise.SetSeed(seed + oreNoiseMap.Count);

    oreNoiseMap[BlockTypes.IRON] = new OreNoiseData(ironOreNoise, 50, 0.6f);
    
    FastNoiseLite coalOreNoise = new FastNoiseLite();
    coalOreNoise.SetNoiseType(noiseType);
    coalOreNoise.SetFrequency(frequency);
    coalOreNoise.SetFractalOctaves(octaves);
    coalOreNoise.SetSeed(seed + oreNoiseMap.Count);

    oreNoiseMap[BlockTypes.COAL] = new OreNoiseData(coalOreNoise, 70, 0.5f);
  }

  public byte GetOreBlock(float x, int y, float z){
    foreach (byte ore in oreNoiseMap.Keys){
      OreNoiseData oreNoiseData = oreNoiseMap[ore];
      float noise = oreNoiseData.oreNoise.GetNoise(x, y, z);
      if (y < oreNoiseData.depth && noise > oreNoiseData.noise){
        return ore;
      }
    }
    return BlockTypes.AIR;
  }
}