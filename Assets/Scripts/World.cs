﻿using UnityEngine;

public class World : MonoBehaviour{
  public static World activeWorld;
  public WorldInfo info;
  public Camera mainCamera;
  public ChunkManager chunkManager;
  private bool initialized;
  public TMPro.TextMeshProUGUI debugText;
  public TMPro.TextMeshProUGUI debugDirectionText;

  public void Initialize(WorldInfo worldInfo){
    this.info = worldInfo;
    activeWorld = this;
    chunkManager.Initialize();
    BiomeManager.Initialize(worldInfo.seed);
    System.GC.Collect();
    initialized = true;
  }

  void LateUpdate(){
    if (!initialized) return;
    debugText.text = "Seed: " + info.seed;
    //update chunks if no modifications have happened this frame
    //only rebuild 1 chunk per frame to avoid framedrops
    chunkManager.UpdateChunks(mainCamera);
  }

  public bool Modify(int x, int y, int z, byte blockType, byte blockState){
    if (!initialized) return false;
    if (y < 0 || y > 255){
      Debug.LogWarning("This is outside build limit");
      return false;
    }

    int chunkX = Mathf.FloorToInt(x / 16f);
    int chunkY = Mathf.FloorToInt(z / 16f);
    int relativeX = x - (chunkX * 16);
    int relativeZ = z - (chunkY * 16);

    return chunkManager.Modify(new Vector2Int(chunkX, chunkY), relativeX, y, relativeZ, blockType, blockState);
  }

  public bool IsBlockAvailable(int x, int y, int z){
    if (!initialized) return false;
    if (y < 0 || y > 255){
      return false;
    }

    int chunkX = Mathf.FloorToInt(x / 16f);
    int chunkY = Mathf.FloorToInt(z / 16f);
    return chunkManager.IsChunkAvailable(new Vector2Int(chunkX, chunkY));
  }

  public byte GetBlock(int x, int y, int z){
    if (!initialized) return 255;
    if (y < 0 || y > 255){
      Debug.LogWarning("This is outside build limit");
      return 255;
    }

    int chunkX = Mathf.FloorToInt(x / 16f);
    int chunkY = Mathf.FloorToInt(z / 16f);
    int relativeX = x - (chunkX * 16);
    int relativeZ = z - (chunkY * 16);
    return chunkManager.GetBlock(new Vector2Int(chunkX, chunkY), relativeX, y, relativeZ);
  }
  
  public byte GetBlockState(int x, int y, int z){
    if (!initialized) return 255;
    if (y < 0 || y > 255){
      Debug.LogWarning("This is outside build limit");
      return 255;
    }

    int chunkX = Mathf.FloorToInt(x / 16f);
    int chunkY = Mathf.FloorToInt(z / 16f);
    int relativeX = x - (chunkX * 16);
    int relativeZ = z - (chunkY * 16);
    return chunkManager.GetBlockState(new Vector2Int(chunkX, chunkY), relativeX, y, relativeZ);
  }
}