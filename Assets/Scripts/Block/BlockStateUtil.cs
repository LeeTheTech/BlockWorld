public static class BlockStateUtil{
  private const byte TICK_MASK = 0x01; // 1 bit for water/lava ticking (true or false)
  private const byte ORIENTATION_MASK = 0x03; // 2 bits for orientation (0-3)
  private const int ORIENTATION_SHIFT = 1; // Shift orientation bits by 1

  private const byte LIQUID_LEVEL_MASK = 0x07; // 3 bits for water level (0-7)
  private const int LIQUID_LEVEL_SHIFT = 3; // Shift water level bits by 3

  // Method to create default state data
  public static byte CreateDefaultStateData(){
    return CreateStateData(false, Direction.NORTH, 0);
  }

  // Method to create state data from ticking, orientation, and water level values
  public static byte CreateStateData(bool isTicking, Direction direction, int liquidLevel){
    byte tickingValue = (byte)(isTicking ? 1 : 0); // Convert bool to byte (1 or 0)
    byte orientationValue = (byte)((byte)direction & ORIENTATION_MASK); // Ensure orientation is 2 bits
    byte liquidLevelValue = (byte)(liquidLevel & LIQUID_LEVEL_MASK); // Ensure water level is 3 bits

    // Combine ticking, orientation, and liquid level into one byte
    return (byte)((tickingValue & TICK_MASK) | (orientationValue << ORIENTATION_SHIFT) | (liquidLevelValue << LIQUID_LEVEL_SHIFT));
  }

  // Method to extract the ticking value from state data
  public static bool IsTicking(byte stateData){
    return (byte)(stateData & TICK_MASK) == 1;
  }

  // Method to set the ticking value in state data
  public static byte SetTicking(byte stateData, bool isTicking){
    // Clear the ticking bit (set to 0)
    byte newStateData = (byte)(stateData & unchecked((byte)~TICK_MASK));
    // Set the ticking bit if isTicking is true
    if (isTicking){
      newStateData |= TICK_MASK; // Set ticking bit to 1
    }

    return newStateData;
  }

  // Method to extract the orientation value from state data
  public static Direction GetOrientation(byte stateData){
    return (Direction)((stateData >> ORIENTATION_SHIFT) & ORIENTATION_MASK);
  }

  // Method to set the orientation value in state data
  public static byte SetOrientation(byte stateData, Direction direction){
    // Clear the orientation bits
    byte newStateData = (byte)(stateData & unchecked((byte)~(ORIENTATION_MASK << ORIENTATION_SHIFT)));

    // Set the new orientation bits based on the direction
    newStateData |= (byte)(((byte)direction & ORIENTATION_MASK) << ORIENTATION_SHIFT);
    return newStateData;
  }

  // Method to extract the liquid level value from state data
  public static int GetLiquidLevel(byte stateData){
    return (stateData >> LIQUID_LEVEL_SHIFT) & LIQUID_LEVEL_MASK;
  }

  // Method to set the liquid level value in state data
  public static byte SetLiquidLevel(byte stateData, int waterLevel){
    // Clear the liquid level bits
    byte newStateData = (byte)(stateData & unchecked((byte)~(LIQUID_LEVEL_MASK << LIQUID_LEVEL_SHIFT)));

    // Set the new liquid level bits
    newStateData |= (byte)((waterLevel & LIQUID_LEVEL_MASK) << LIQUID_LEVEL_SHIFT);
    return newStateData;
  }
}