using System;

[Serializable]
public class Biome{
  public BiomeType biomeType;
  public BlockLayers blockLayers;
  
  public Biome(BiomeType biomeType, BlockLayers blockLayers){
    this.biomeType = biomeType;
    this.blockLayers = blockLayers;
  }
}

[Serializable]
public struct BlockLayers{
  public byte layer1;
  public byte layer2;
  public byte layer3;
  
  public BlockLayers(byte layer1, byte layer2, byte layer3){
    this.layer1 = layer1;
    this.layer2 = layer2;
    this.layer3 = layer3;
  }
}