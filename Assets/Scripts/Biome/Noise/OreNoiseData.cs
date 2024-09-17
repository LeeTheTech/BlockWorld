public class OreNoiseData{
  public FastNoiseLite oreNoise;
  public int depth;
  public float noise;
  
  public OreNoiseData(FastNoiseLite oreNoise, int depth, float noise){
    this.oreNoise = oreNoise;
    this.depth = depth;
    this.noise = noise;
  }
}