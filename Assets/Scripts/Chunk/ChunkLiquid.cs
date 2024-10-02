using System.Collections.Generic;
using UnityEngine;

public static class ChunkLiquid {

  public static void HandleLiquidTick(ChunkDataManager chunkDataManager, Vector2Int chunkPos, List<Vector2Int> chunksToRebuild, byte liquidType, byte state, int x, int y, int z, byte[,,] blocks, byte[,,] blockStates){
    chunkDataManager.Modify(chunkPos, x, y, z, liquidType, BlockStateUtil.SetTicking(state, false));
            
    if (y > 0 && blocks[x, y - 1, z] == BlockTypes.AIR){
      chunkDataManager.Modify(chunkPos, x, y - 1, z, liquidType, BlockStateUtil.CreateStateData(true, Direction.NORTH, 0));
    }
    else{
      TrySpreadLiquid(chunkDataManager, chunkPos, chunksToRebuild, blocks, blockStates, x, y, z, liquidType);
    }
  }
  
  private static void TrySpreadLiquid(ChunkDataManager chunkDataManager, Vector2Int chunkPos, List<Vector2Int> chunksToRebuild, byte[,,] blocks, byte[,,] blockStates, int x, int y, int z, byte liquidType){
    // Spread water inside the chunk first
    SpreadLiquidWithinChunk(chunkDataManager, chunkPos, blocks, blockStates, x, y, z, liquidType);

    // Now check if the water is at the chunk edges
    if (x == 0 || x == 15 || z == 0 || z == 15){
      TrySpreadToNeighborChunk(chunkDataManager, chunkPos, chunksToRebuild, x, y, z, liquidType, BlockStateUtil.GetLiquidLevel(blockStates[x, y, z]));
    }
  }

  private static void SpreadLiquidWithinChunk(ChunkDataManager chunkDataManager, Vector2Int chunkPos, byte[,,] blocks, byte[,,] blockStates, int x, int y, int z, byte liquidType){
    // Check adjacent blocks within the chunk
    byte blockState = blockStates[x, y, z];
    int liquidLevel = BlockStateUtil.GetLiquidLevel(blockState);
    if (liquidLevel >= 7){
      chunkDataManager.Modify(chunkPos, x, y, z, liquidType, BlockStateUtil.SetTicking(blockState, false));
      return;
    }
        
    if (z > 0 && blocks[x, y, z - 1] == BlockTypes.AIR){
      chunkDataManager.Modify(chunkPos, x, y, z - 1, liquidType, BlockStateUtil.CreateStateData(true, Direction.NORTH, Mathf.Min(liquidLevel + 1, 7)));
    }

    if (z < 15 && blocks[x, y, z + 1] == BlockTypes.AIR){
      chunkDataManager.Modify(chunkPos, x, y, z + 1, liquidType, BlockStateUtil.CreateStateData(true, Direction.NORTH, Mathf.Min(liquidLevel + 1, 7)));
    }

    if (x > 0 && blocks[x - 1, y, z] == BlockTypes.AIR){
      chunkDataManager.Modify(chunkPos,x - 1, y, z, liquidType, BlockStateUtil.CreateStateData(true, Direction.NORTH, Mathf.Min(liquidLevel + 1, 7)));
    }

    if (x < 15 && blocks[x + 1, y, z] == BlockTypes.AIR){
      chunkDataManager.Modify(chunkPos, x + 1, y, z, liquidType, BlockStateUtil.CreateStateData(true, Direction.NORTH, Mathf.Min(liquidLevel + 1, 7)));
    }
  }

  private static void TrySpreadToNeighborChunk(ChunkDataManager chunkDataManager, Vector2Int chunkPosition, List<Vector2Int> chunksToRebuild, int x, int y, int z, byte liquidType, int liquidLevel){
    switch (x){
      // Determine the neighbor chunk position based on the edge
      case 0:{
        Vector2Int neighborPos = chunkPosition + Vector2Int.left;
        SpreadLiquidToNeighbor(chunkDataManager, neighborPos, chunksToRebuild, 15, y, z, liquidType, liquidLevel); // Rightmost block of the neighbor chunk
        break;
      }
      case 15:{
        Vector2Int neighborPos = chunkPosition + Vector2Int.right;
        SpreadLiquidToNeighbor(chunkDataManager, neighborPos, chunksToRebuild, 0, y, z, liquidType, liquidLevel); // Leftmost block of the neighbor chunk
        break;
      }
    }
    switch (z){
      case 0:{
        Vector2Int neighborPos = chunkPosition + Vector2Int.down;
        SpreadLiquidToNeighbor(chunkDataManager, neighborPos, chunksToRebuild, x, y, 15, liquidType, liquidLevel); // Topmost block of the neighbor chunk
        break;
      }
      case 15:{
        Vector2Int neighborPos = chunkPosition + Vector2Int.up;
        SpreadLiquidToNeighbor(chunkDataManager, neighborPos, chunksToRebuild, x, y, 0, liquidType, liquidLevel); // Bottommost block of the neighbor chunk
        break;
      }
    }
  }

  private static void SpreadLiquidToNeighbor(ChunkDataManager chunkDataManager, Vector2Int neighborPos, List<Vector2Int> chunksToRebuild, int neighborX, int y, int neighborZ, byte liquidType, int liquidLevel){
    ChunkData neighborChunk = chunkDataManager.data[neighborPos];
    byte[,,] neighborBlocks = neighborChunk.GetBlocks();
    if (neighborBlocks[neighborX, y, neighborZ] == BlockTypes.AIR){
      if (!chunksToRebuild.Contains(neighborChunk.position)) chunksToRebuild.Add(new Vector2Int(neighborChunk.position.x, neighborChunk.position.y));
      chunkDataManager.Modify(neighborChunk.position, neighborX, y, neighborZ, liquidType, BlockStateUtil.CreateStateData(true, Direction.NORTH, Mathf.Min(liquidLevel + 1, 7)));
    }
  }
}
