using UnityEngine;

public static class LiquidUtil {

  public static void HandleLiquidTick(ChunkDataManager chunkDataManager, Vector2Int position, byte block, byte state, int x, int y, int z, byte[,,] blocks, byte[,,] blockStates){
    blockStates[x, y, z] = BlockStateUtil.SetTicking(state, false);
    if (BlockStateUtil.GetLiquidLevel(state) >= 7) return;
            
    if (y > 0 && blocks[x, y - 1, z] == BlockTypes.AIR){
      blocks[x, y - 1, z] = block;
      blockStates[x, y - 1, z] = BlockStateUtil.SetTicking(state, true);
    }
    else{
      TrySpreadLiquid(blocks, blockStates, chunkDataManager, position, x, y, z, block);
    }
  }
  
  private static void TrySpreadLiquid(byte[,,] blocks, byte[,,] blockStates, ChunkDataManager chunkDataManager, Vector2Int chunkPosition, int x, int y, int z, byte liquidType){
    // Spread water inside the chunk first
    SpreadLiquidWithinChunk(blocks, blockStates, x, y, z, liquidType);

    // Now check if the water is at the chunk edges
    if (x == 0 || x == 15 || z == 0 || z == 15){
      TrySpreadToNeighborChunk(chunkDataManager, chunkPosition, x, y, z, liquidType, BlockStateUtil.GetLiquidLevel(blockStates[x, y, z]));
    }
  }

  private static void SpreadLiquidWithinChunk(byte[,,] blocks, byte[,,] blockStates, int x, int y, int z, byte liquidType){
    // Check adjacent blocks within the chunk
    byte blockState = blockStates[x, y, z];
    int liquidLevel = BlockStateUtil.GetLiquidLevel(blockState);
    if (liquidLevel == 7){
      blockStates[x, y, z] = BlockStateUtil.SetTicking(blockState, false);
      return;
    }
        
    if (z > 0 && blocks[x, y, z - 1] == BlockTypes.AIR) {
      blocks[x, y, z - 1] = liquidType;
      blockStates[x, y, z - 1] = BlockStateUtil.CreateStateData(true, Direction.NORTH, liquidLevel + 1);
    }

    if (z < 15 && blocks[x, y, z + 1] == BlockTypes.AIR) {
      blocks[x, y, z + 1] = liquidType;
      blockStates[x, y, z + 1] = BlockStateUtil.CreateStateData(true, Direction.NORTH, liquidLevel + 1);
    }

    if (x > 0 && blocks[x - 1, y, z] == BlockTypes.AIR) {
      blocks[x - 1, y, z] = liquidType;
      blockStates[x - 1, y, z] = BlockStateUtil.CreateStateData(true, Direction.NORTH, liquidLevel + 1);
    }

    if (x < 15 && blocks[x + 1, y, z] == BlockTypes.AIR) {
      blocks[x + 1, y, z] = liquidType;
      blockStates[x + 1, y, z] = BlockStateUtil.CreateStateData(true, Direction.NORTH, liquidLevel + 1);
    }
  }

  private static void TrySpreadToNeighborChunk(ChunkDataManager chunkDataManager, Vector2Int chunkPosition, int x, int y, int z, byte liquidType, int liquidLevel){
    switch (x){
      // Determine the neighbor chunk position based on the edge
      case 0:{
        Vector2Int neighborPos = chunkPosition + Vector2Int.left;
        SpreadLiquidToNeighbor(chunkDataManager, neighborPos, 15, y, z, liquidType, liquidLevel); // Rightmost block of the neighbor chunk
        break;
      }
      case 15:{
        Vector2Int neighborPos = chunkPosition + Vector2Int.right;
        SpreadLiquidToNeighbor(chunkDataManager, neighborPos, 0, y, z, liquidType, liquidLevel); // Leftmost block of the neighbor chunk
        break;
      }
    }
    switch (z){
      case 0:{
        Vector2Int neighborPos = chunkPosition + Vector2Int.down;
        SpreadLiquidToNeighbor(chunkDataManager, neighborPos, x, y, 15, liquidType, liquidLevel); // Topmost block of the neighbor chunk
        break;
      }
      case 15:{
        Vector2Int neighborPos = chunkPosition + Vector2Int.up;
        SpreadLiquidToNeighbor(chunkDataManager, neighborPos, x, y, 0, liquidType, liquidLevel); // Bottommost block of the neighbor chunk
        break;
      }
    }
  }

  private static void SpreadLiquidToNeighbor(ChunkDataManager chunkDataManager, Vector2Int neighborPos, int neighborX, int y, int neighborZ, byte liquidType, int liquidLevel){
    ChunkData neighborChunk = chunkDataManager.data[neighborPos];
    byte[,,] neighborBlocks = neighborChunk.GetBlocks();
    byte[,,] neighborBlockStates = neighborChunk.GetBlockStates();

    if (neighborBlocks[neighborX, y, neighborZ] == BlockTypes.AIR){
      neighborBlocks[neighborX, y, neighborZ] = liquidType;
      neighborBlockStates[neighborX, y, neighborZ] = BlockStateUtil.CreateStateData(true, Direction.NORTH, Mathf.Min(liquidLevel + 1, 7));
    }
  }
}
