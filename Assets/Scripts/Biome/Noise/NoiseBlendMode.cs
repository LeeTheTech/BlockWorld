public enum NoiseBlendMode {
  ADD,        // Adds the noise layer's value to the previous value
  MULTIPLY,   // Multiplies the noise layer's value with the previous value
  MAX,        // Takes the maximum value between the noise layer and the previous value
  MIN,        // Takes the minimum value between the noise layer and the previous value
  SUBTRACT    // Subtracts the noise layer's value from the previous value
}