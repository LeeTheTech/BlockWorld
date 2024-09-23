public static class BlockStateUtil{
  public const byte WATER_LEVEL_MASK = 0x07; // 3 bits for water level (0-7)
  public const byte ORIENTATION_MASK = 0x03; // 2 bits for orientation (0-3)

  public static byte SetWaterLevel(byte state, byte waterLevel){
    return (byte)((state & ~WATER_LEVEL_MASK) | (waterLevel & WATER_LEVEL_MASK));
  }

  public static byte SetOrientation(byte state, byte orientation){
    return (byte)((state & ~ORIENTATION_MASK) | (orientation & ORIENTATION_MASK));
  }

  public static byte GetWaterLevel(byte state){
    return (byte)(state & WATER_LEVEL_MASK);
  }

  public static byte GetOrientation(byte state){
    return (byte)(state & ORIENTATION_MASK);
  }
}