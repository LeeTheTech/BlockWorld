using System.Collections.Generic;
using UnityEngine;

public static class ChunkExplosive{
  private const int explosionRadius = 10;

  public static void HandleExplosiveTick(ChunkDataManager chunkDataManager, Vector2Int chunkPos, List<Vector2Int> chunksToRebuild, byte block, byte state, int x, int y, int z, byte[,,] blocks, byte[,,] blockStates){
    chunkDataManager.Modify(chunkPos, x, y, z, BlockTypes.AIR, 0);

    for (int dx = -explosionRadius; dx <= explosionRadius; dx++){
      for (int dy = -explosionRadius; dy <= explosionRadius; dy++){
        for (int dz = -explosionRadius; dz <= explosionRadius; dz++){
          int newX = x + dx;
          int newY = y + dy;
          int newZ = z + dz;

          float distance = Mathf.Sqrt(dx * dx + dy * dy + dz * dz);
          if (distance <= explosionRadius){
            if (IsValidBlock(newX, newY, newZ, blocks)){
              chunkDataManager.Modify(chunkPos, newX, newY, newZ, BlockTypes.AIR, 0);
            }
            else{
              Vector2Int neighborChunkPos = new Vector2Int(chunkPos.x, chunkPos.y);
              int blockIndexX = newX % 16;
              int blockIndexZ = newZ % 16;

              if (blockIndexX < 0) blockIndexX += 16;
              if (blockIndexZ < 0) blockIndexZ += 16;
              
              switch (newX){
                case < 0:
                  neighborChunkPos.x -= 1;
                  break;
                case >= 16:
                  neighborChunkPos.x += 1;
                  break;
              }

              switch (newZ){
                case < 0:
                  neighborChunkPos.y -= 1;
                  break;
                case >= 16:
                  neighborChunkPos.y += 1;
                  break;
              }
              
              chunkDataManager.Modify(neighborChunkPos, blockIndexX, newY, blockIndexZ, BlockTypes.AIR, 0);
            }
          }
        }
      }
    }
  }

  private static bool IsValidBlock(int x, int y, int z, byte[,,] blocks){
    return x >= 0 && x < blocks.GetLength(0) && y >= 0 && y < blocks.GetLength(1) && z >= 0 && z < blocks.GetLength(2);
  }
}