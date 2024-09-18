[System.Serializable]
public class WorldInfo{
  public string name;
  public int seed;
  
  public WorldInfo(string name, int seed){
    this.name = name;
    this.seed = seed;
  }
}