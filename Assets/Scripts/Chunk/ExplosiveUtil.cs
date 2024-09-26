using UnityEngine;

public static class ExplosiveUtil{
  private const int explosionRadius = 5;

  public static void HandleExplosiveTick(ChunkDataManager chunkDataManager, Vector2Int position, byte block, byte state, int x, int y, int z, byte[,,] blocks, byte[,,] blockStates){
    // Set the block at the explosion center to air
    blocks[x, y, z] = BlockTypes.AIR;
    blockStates[x, y, z] = 0;

    // Iterate over the explosion radius
    for (int dx = -explosionRadius; dx <= explosionRadius; dx++){
      for (int dy = -explosionRadius; dy <= explosionRadius; dy++){
        for (int dz = -explosionRadius; dz <= explosionRadius; dz++){
          int newX = x + dx;
          int newY = y + dy;
          int newZ = z + dz;

          // Check if the block is within the current chunk
          if (IsValidBlock(newX, newY, newZ, blocks)){
            blocks[newX, newY, newZ] = BlockTypes.AIR;
            blockStates[newX, newY, newZ] = 0;
          }
        }
      }
    }
  }

  private static bool IsValidBlock(int x, int y, int z, byte[,,] blocks){
    return x >= 0 && x < blocks.GetLength(0) && y >= 0 && y < blocks.GetLength(1) && z >= 0 && z < blocks.GetLength(2);
  }
}