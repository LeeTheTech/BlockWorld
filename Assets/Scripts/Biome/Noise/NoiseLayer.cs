using UnityEngine;

[System.Serializable]
public class NoiseLayer{
  public FastNoiseLite noise;
  public float frequency;
  public float amplitude;
  public NoiseBlendMode blendMode;

  public NoiseLayer(FastNoiseLite.NoiseType noiseType, float frequency, float amplitude, NoiseBlendMode blendMode, int seed){
    this.noise = new FastNoiseLite();
    this.noise.SetSeed(seed);
    this.noise.SetNoiseType(noiseType);
    this.noise.SetFrequency(frequency);
    this.frequency = frequency;
    this.amplitude = amplitude;
    this.blendMode = blendMode;
  }

  public float GetNoise(float x, float z){
    return noise.GetNoise(x, z) * amplitude;
  }

  public float ApplyNoise(float currentHeight, float x, float z){
    float newHeight = GetNoise(x, z);

    switch (blendMode){
      case NoiseBlendMode.ADD:
        return currentHeight + newHeight;
      case NoiseBlendMode.SUBTRACT:
        return currentHeight - newHeight;
      case NoiseBlendMode.MAX:
        return Mathf.Max(currentHeight, newHeight);
      case NoiseBlendMode.MIN:
        return Mathf.Min(currentHeight, newHeight);
      default:
        return currentHeight;
    }
  }
}