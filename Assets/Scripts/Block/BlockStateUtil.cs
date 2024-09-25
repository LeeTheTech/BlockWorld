public static class BlockStateUtil{
  private const byte TICK_MASK = 0x01; // 1 bit for water/lava ticking (true or false)
  private const byte ORIENTATION_MASK = 0x03; // 2 bits for orientation (0-3)
  private const int ORIENTATION_SHIFT = 1; // We need to shift orientation bits

  // Method to create default state data
  public static byte CreateDefaultStateData(){
    return CreateStateData(false, Direction.NORTH);
  }

  // Method to create state data from ticking and orientation values
  public static byte CreateStateData(bool isTicking, Direction direction){
    byte tickingValue = (byte)(isTicking ? 1 : 0); // Convert bool to byte (1 or 0)
    byte orientationValue = (byte)((byte)direction & ORIENTATION_MASK); // Ensure orientation is 2 bits

    // Combine ticking and orientation into one byte
    return (byte)((tickingValue & TICK_MASK) | (orientationValue << ORIENTATION_SHIFT));
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
    switch ((byte)((stateData >> ORIENTATION_SHIFT) & ORIENTATION_MASK)){
      case 0:
        return Direction.NORTH;
      case 1:
        return Direction.EAST;
      case 2:
        return Direction.SOUTH;
      case 3:
        return Direction.WEST;
      default:
        return Direction.NORTH; // Default fallback
    }
  }

  // Method to set the orientation value in state data
  public static byte SetOrientation(byte stateData, Direction direction){
    // Clear the orientation bits
    byte newStateData = (byte)(stateData & unchecked((byte)~(ORIENTATION_MASK << ORIENTATION_SHIFT)));
        
    // Set the new orientation bits based on the direction
    newStateData |= (byte)(((byte)direction & ORIENTATION_MASK) << ORIENTATION_SHIFT);
    return newStateData;
  }
}