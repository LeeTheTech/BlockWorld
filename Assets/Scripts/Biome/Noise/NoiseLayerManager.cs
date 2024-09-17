using System.Collections.Generic;
using UnityEngine;

public class NoiseLayerManager {
  private readonly Dictionary<BiomeType, List<NoiseLayer>> noiseLayers = new();

  public void AddLayer(BiomeType biomeType, NoiseLayer layer) {
    if (noiseLayers.TryGetValue(biomeType, out List<NoiseLayer> noiseLayer)) noiseLayer.Add(layer);
    else noiseLayers.Add(biomeType, new List<NoiseLayer>{layer});
  }
  
  public float GenerateHeight(BiomeType biomeType, float x, float z){
    float finalValue = 0f;

    foreach (var layer in noiseLayers[biomeType]){
      float layerValue = layer.GetNoise(x, z);
      switch (layer.blendMode){
        case NoiseBlendMode.ADD:
          finalValue += layerValue;
          break;
        case NoiseBlendMode.MULTIPLY:
          finalValue *= layerValue;
          break;
        case NoiseBlendMode.MAX:
          finalValue = Mathf.Max(finalValue, layerValue);
          break;
        case NoiseBlendMode.MIN:
          finalValue = Mathf.Min(finalValue, layerValue);
          break;
        case NoiseBlendMode.SUBTRACT:
          finalValue -= layerValue;
          break;
      }
    }

    return finalValue;
  }
}