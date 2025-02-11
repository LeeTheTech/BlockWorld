﻿using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Linq;

public class ChunkManager : MonoBehaviour{
  public Chunk chunkPrefab;
  public ChunkDataManager chunkDataManager;

  private int renderDistance;
  private int maximumLoadQueueSize;

  public bool isInStartup;
  public bool isShuttingDown;

  private Queue<Chunk> chunkPool;
  private Queue<Vector2Int> modifiedRebuildQueue;
  private List<Vector2Int> modifyNeighborOrder;
  public Dictionary<Vector2Int, Chunk> chunkMap;

  //Camera info (multiple threads)
  private Vector3 cameraPosition;
  private Vector3 cameraForward;

  //ShouldRender Thread
  private Thread shouldRenderThread;
  private readonly System.Object shouldRenderLock = new();
  private bool shouldRenderWaitForUpdate;
  private volatile List<Vector2Int> loadQueue;
  private volatile Queue<Vector2Int> unloadQueue;
  private volatile List<Vector2Int> activeChunks;

  private readonly Vector2Int nFront = new(0, 1);
  private readonly Vector2Int nBack = new(0, -1);
  private readonly Vector2Int nLeft = new(-1, 0);
  private readonly Vector2Int nRight = new(1, 0);
  
  private readonly Vector2Int nFrontLeft = new(-1, 1);
  private readonly Vector2Int nFrontRight = new(1, 1);
  private readonly Vector2Int nBackLeft = new(-1, -1);
  private readonly Vector2Int nBackRight = new(1, -1);
  
  //ShouldTick Thread
  private Thread shouldTickThread;
  private readonly System.Object shouldTickLock = new();
  private Queue<Vector2Int> tickQueue;
  private Queue<Vector2Int> tickModifiedRebuildQueue;
  private List<Vector2Int> tickChunksToRebuild;
  private const int tickDistance = 3;
  private bool tickRan;

  public void Initialize(){
    renderDistance = GameManager.instance.gameSettings.RenderDistance;
    maximumLoadQueueSize = GameManager.instance.gameSettings.maximumLoadQueueSize;
    chunkDataManager = new ChunkDataManager();
    chunkMap = new Dictionary<Vector2Int, Chunk>();
    chunkPool = new Queue<Chunk>();
    activeChunks = new List<Vector2Int>();
    loadQueue = new List<Vector2Int>();
    tickQueue = new Queue<Vector2Int>();
    tickModifiedRebuildQueue = new Queue<Vector2Int>();
    tickChunksToRebuild = new List<Vector2Int>();
    unloadQueue = new Queue<Vector2Int>();
    modifiedRebuildQueue = new Queue<Vector2Int>();
    modifyNeighborOrder = new List<Vector2Int>();
    int chunkPoolSize = renderDistance * renderDistance * 4;
    for (int i = 0; i < chunkPoolSize; ++i){
      Chunk c = Instantiate(chunkPrefab, chunkPrefab.transform.parent, true);
      GameObject cObject = c.gameObject;
      cObject.SetActive(false);
      cObject.name = "Chunk " + i;
      chunkPool.Enqueue(c);
    }

    shouldRenderThread = new Thread(ShouldRenderThread){
        IsBackground = true
    };
    shouldRenderThread.Start();
    
    shouldTickThread = new Thread(ShouldTickThread){
        IsBackground = true
    };
    shouldTickThread.Start();
  }

  public void UpdateChunks(Camera mainCamera){
    if (isShuttingDown) return;
    UnityEngine.Profiling.Profiler.BeginSample("UPDATING CHUNKS");
    chunkDataManager.Update();

    Transform cameraTransform = mainCamera.transform;
    cameraPosition = cameraTransform.position;
    cameraForward = cameraTransform.forward;

    int loadQueueCount = 0;

    UnityEngine.Profiling.Profiler.BeginSample("LOCK SHOULD RENDER");

    //startup loads 8 chunks per frame
    for (int loop = 0; loop < (isInStartup ? 8 : 1); ++loop){
      lock (shouldRenderLock){
        loadQueueCount = loadQueue.Count;

        lock (shouldTickLock){
          UnityEngine.Profiling.Profiler.BeginSample("TICK CHUNK");
          // Tick chunk
          if (tickQueue.Count > 0 && tickModifiedRebuildQueue.Count == 0){
            tickRan = true;
            Vector2Int position = tickQueue.Dequeue();
            if (activeChunks.Contains(position)){ 
              chunkMap[position].Tick(chunkDataManager, tickChunksToRebuild);
              foreach (Vector2Int chunkPos in tickChunksToRebuild){
                QueueTickModify(chunkPos);
              }
              tickChunksToRebuild.Clear();
            }
          }
          // Process ticked chunks
          if (tickModifiedRebuildQueue.Count > 0 && tickRan == false){
            Vector2Int position = tickModifiedRebuildQueue.Dequeue();
            if (activeChunks.Contains(position)){
              chunkMap[position].Build(chunkDataManager);
            }
          }
          
          tickRan = false;
        }
        
        UnityEngine.Profiling.Profiler.EndSample();

        if (modifiedRebuildQueue.Count > 0){
          UnityEngine.Profiling.Profiler.BeginSample("MODIFY CHUNK");
          Vector2Int position = modifiedRebuildQueue.Dequeue();
          if (activeChunks.Contains(position)){
            chunkMap[position].Build(chunkDataManager);
          }

          UnityEngine.Profiling.Profiler.EndSample();
        }
        else{
          UnityEngine.Profiling.Profiler.BeginSample("UNLOADING");
          while (true){
            if (unloadQueue.Count == 0) break;

            Vector2Int position = unloadQueue.Dequeue();

            if (!activeChunks.Contains(position)) continue;

            Chunk chunk = chunkMap[position];
            chunk.Unload();
            chunkPool.Enqueue(chunk);

            activeChunks.Remove(position);
            chunkMap.Remove(position);
            chunkDataManager.UnloadChunk(position);
          }

          UnityEngine.Profiling.Profiler.EndSample();

          UnityEngine.Profiling.Profiler.BeginSample("BUILD CHUNK CHECK");

          bool buildChunk = false;
          Vector2Int chunkToBuild = Vector2Int.zero;
          for (int i = 0; i < loadQueue.Count; ++i){
            Vector2Int position = loadQueue[i];
            if (chunkDataManager.Load(position)){
              if (!buildChunk){
                buildChunk = true;
                chunkToBuild = position;
              }
            }
          }

          UnityEngine.Profiling.Profiler.EndSample();

          UnityEngine.Profiling.Profiler.BeginSample("BUILD CHUNK");

          if (buildChunk){
            loadQueue.Remove(chunkToBuild);
            if (!chunkMap.ContainsKey(chunkToBuild)){
              Chunk chunk = chunkPool.Dequeue();
              chunk.Initialize(chunkToBuild);
              chunkMap.Add(chunkToBuild, chunk);
              chunk.Build(chunkDataManager);
              activeChunks.Add(chunkToBuild);
            }
          }
          UnityEngine.Profiling.Profiler.EndSample();
        }
      }
    }

    shouldRenderWaitForUpdate = false;
    UnityEngine.Profiling.Profiler.EndSample();
    int activeChunksCount = activeChunks.Count;
    int rebuildQueueCount = modifiedRebuildQueue.Count;
    int chunksInMemoryCount = chunkDataManager.GetChunksInMemoryCount();
    World.activeWorld.debugText.text += $" / Chunks (Q:{loadQueueCount} R:{rebuildQueueCount} A:{activeChunksCount} M:{chunksInMemoryCount})";
    UnityEngine.Profiling.Profiler.EndSample();
  }

  public bool StartupFinished(){
    bool ready;
    lock (shouldRenderLock){
      ready = activeChunks.Count >= renderDistance * renderDistance;
    }
    return ready;
  }

  public Vector2Int[] GetActiveChunkPositions(){
    Vector2Int[] positions;
    lock (shouldRenderLock){
      positions = activeChunks.ToArray();
    }
    return positions;
  }

  private void ShouldTickThread(){
    List<Vector2Int> visiblePoints = new List<Vector2Int>();
    
    while (true){
      visiblePoints.Clear();
      Vector2Int cameraChunkPos = new Vector2Int((int)cameraPosition.x / 16, (int)cameraPosition.z / 16);
      Vector3 cameraPositionFloor = new Vector3(cameraPosition.x, 0, cameraPosition.z);
      for (int y = 0; y < tickDistance * 2; ++y){
        for (int x = 0; x < tickDistance * 2; ++x){
          Vector2Int c = cameraChunkPos - new Vector2Int(tickDistance, tickDistance) + new Vector2Int(x, y);
          Vector3 renderPosition = new Vector3(c.x * 16, 0, c.y * 16);
          Vector3 toChunk = renderPosition - cameraPositionFloor;

          if (toChunk.magnitude < (tickDistance * 16)){
            visiblePoints.Add(c);
          }
        }
      }
      List<Vector2Int> ordered = visiblePoints.OrderBy(vp => Vector2Int.Distance(cameraChunkPos, vp)).ToList();
      lock (shouldTickLock){
        foreach (Vector2Int chunkPos in ordered){
          if (!tickQueue.Contains(chunkPos)){
            tickQueue.Enqueue(chunkPos);
          }
        }
      }
      Thread.Sleep(8);
    }
  }

  private void ShouldRenderThread(){
    List<Vector2Int> visiblePoints = new List<Vector2Int>();
    List<Vector2Int> inRangePoints = new List<Vector2Int>();
    List<Vector2Int> copyOfActiveChunks = new List<Vector2Int>();
    Queue<Vector2Int> copyOfUnload = new Queue<Vector2Int>();
    Queue<Vector2Int> copyOfLoad = new Queue<Vector2Int>();
    while (true){
      visiblePoints.Clear();
      inRangePoints.Clear();
      copyOfActiveChunks.Clear();
      Vector2Int cameraChunkPos = new Vector2Int((int)cameraPosition.x / 16, (int)cameraPosition.z / 16);
      Vector3 cameraPositionFloor = new Vector3(cameraPosition.x, 0, cameraPosition.z);
      Vector3 cameraForwardFloor = cameraForward;
      cameraForwardFloor.y = 0;
      cameraForwardFloor.Normalize();
      for (int y = 0; y < renderDistance * 2; ++y){
        for (int x = 0; x < renderDistance * 2; ++x){
          Vector2Int c = cameraChunkPos - new Vector2Int(renderDistance, renderDistance) + new Vector2Int(x, y);
          Vector3 renderPosition = new Vector3(c.x * 16, 0, c.y * 16);

          Vector3 toChunk = renderPosition - cameraPositionFloor;

          bool inRange = toChunk.magnitude < (renderDistance * 16);
          bool inAngle = Vector3.Angle(toChunk, cameraForwardFloor) < 70;
          if (isInStartup){
            int startupMin = renderDistance / 2;
            int startupMax = startupMin + renderDistance;
            inAngle = true;
            inRange = x > startupMin && x <= startupMax && y > startupMin && y <= startupMax;
          }

          bool isClose = toChunk.magnitude < (16 * 3);
          if (inRange) inRangePoints.Add(c);
          if (((inAngle) && inRange) || isClose) visiblePoints.Add(c);
        }
      }

      List<Vector2Int> ordered = visiblePoints.OrderBy(vp => Vector2Int.Distance(cameraChunkPos, vp)).ToList<Vector2Int>();

      while (shouldRenderWaitForUpdate) Thread.Sleep(8);
      shouldRenderWaitForUpdate = true;
      
      lock (shouldRenderLock){
        for (int i = 0; i < activeChunks.Count; ++i){
          copyOfActiveChunks.Add(activeChunks[i]);
        }
      }

      for (int i = copyOfActiveChunks.Count - 1; i > -1; --i){
        Vector2Int position = copyOfActiveChunks[i];
        if (!inRangePoints.Contains(position)){
          copyOfUnload.Enqueue(position);
        }
      }

      for (int i = 0; i < ordered.Count; ++i){
        if (copyOfLoad.Count == maximumLoadQueueSize) break;
        Vector2Int position = ordered[i];
        if (!copyOfActiveChunks.Contains(position)){
          copyOfLoad.Enqueue(position);
        }
      }
      
      lock (shouldRenderLock){
        while (copyOfUnload.Count > 0){
          unloadQueue.Enqueue(copyOfUnload.Dequeue());
        }

        while (copyOfLoad.Count > 0){
          if (loadQueue.Count == maximumLoadQueueSize) break;
          Vector2Int position = copyOfLoad.Dequeue();
          if (!loadQueue.Contains(position)){
            loadQueue.Add(position);
          }
        }
      }
    }
  }

  public void RenderAllChunksAroundCamera(){
    Vector3 cameraPos = World.activeWorld.mainCamera.transform.position;
    Vector2Int cameraChunkPos = new Vector2Int((int)cameraPos.x / 16, (int)cameraPos.z / 16);
    lock (shouldRenderLock){
      for (int y = cameraChunkPos.y - renderDistance + 1; y < renderDistance - 1; ++y){
        for (int x = cameraChunkPos.x - renderDistance + 1; x < renderDistance - 1; ++x){
          Vector2Int pos = new Vector2Int(x, y);
          if (Vector2Int.Distance(pos, cameraChunkPos) < renderDistance){
            if (!loadQueue.Contains(pos)){
              loadQueue.Add(pos);
            }
          }
        }
      }
    }
  }

  public byte GetBlock(Vector2Int chunk, int x, int y, int z){
    if (!chunkMap.ContainsKey(chunk)) throw new System.Exception("Chunk is not available");
    return chunkDataManager.GetBlock(chunk, x, y, z);
  }
  
  public byte GetBlockState(Vector2Int chunk, int x, int y, int z){
    if (!chunkMap.ContainsKey(chunk)) throw new System.Exception("Chunk is not available");
    return chunkDataManager.GetBlockState(chunk, x, y, z);
  }
  
  public bool IsChunkAvailable(Vector2Int chunk){
    return chunkMap.ContainsKey(chunk);
  }

  private void QueueTickModify(Vector2Int chunk){
    if (!chunkMap.ContainsKey(chunk)) throw new System.Exception("Chunk is not available");
    
    // Front
    if (!tickModifiedRebuildQueue.Contains(chunk + nFront)){
      tickModifiedRebuildQueue.Enqueue(chunk + nFront);
    }
    if (!tickModifiedRebuildQueue.Contains(chunk + nFrontRight)){
      tickModifiedRebuildQueue.Enqueue(chunk + nFrontRight);
    }
    if (!tickModifiedRebuildQueue.Contains(chunk + nFrontLeft)){
      tickModifiedRebuildQueue.Enqueue(chunk + nFrontLeft);
    }
    
    // Right
    if (!tickModifiedRebuildQueue.Contains(chunk + nRight)){
      tickModifiedRebuildQueue.Enqueue(chunk + nRight);
    }
    
    // Left
    if (!tickModifiedRebuildQueue.Contains(chunk + nLeft)){
      tickModifiedRebuildQueue.Enqueue(chunk + nLeft);
    }
    
    // Back
    if (!tickModifiedRebuildQueue.Contains(chunk + nBack)){
      tickModifiedRebuildQueue.Enqueue(chunk + nBack);
    }
    if (!tickModifiedRebuildQueue.Contains(chunk + nBackLeft)){
      tickModifiedRebuildQueue.Enqueue(chunk + nBackLeft);
    }
    if (!tickModifiedRebuildQueue.Contains(chunk + nBackRight)){
      tickModifiedRebuildQueue.Enqueue(chunk + nBackRight);
    }
    
    // Target Chunk
    if (!tickModifiedRebuildQueue.Contains(chunk)){
      tickModifiedRebuildQueue.Enqueue(chunk);
    }
  }

  public bool Modify(Vector2Int chunk, int x, int y, int z, byte blockType, byte blockState){
    if (modifiedRebuildQueue.Count > 0) return false;
    if (!chunkMap.ContainsKey(chunk)) throw new System.Exception("Chunk is not available");
    chunkDataManager.Modify(chunk, x, y, z, blockType, blockState);
    bool f = z == 15;
    bool b = z == 0;
    bool l = x == 0;
    bool r = x == 15;
    if (blockType != BlockTypes.AIR) f = b = l = r = false;
    modifyNeighborOrder.Clear();
    modifyNeighborOrder.Add(chunk);
    if (f){
      modifyNeighborOrder.Insert(0, chunk + nFront);
    }
    else{
      modifyNeighborOrder.Add(chunk + nFront);
    }

    if (b){
      modifyNeighborOrder.Insert(0, chunk + nBack);
    }
    else{
      modifyNeighborOrder.Add(chunk + nBack);
    }

    if (l){
      modifyNeighborOrder.Insert(0, chunk + nLeft);
    }
    else{
      modifyNeighborOrder.Add(chunk + nLeft);
    }

    if (r){
      modifyNeighborOrder.Insert(0, chunk + nRight);
    }
    else{
      modifyNeighborOrder.Add(chunk + nRight);
    }

    for (int i = 0; i < modifyNeighborOrder.Count; ++i){
      modifiedRebuildQueue.Enqueue(modifyNeighborOrder[i]);
    }

    modifiedRebuildQueue.Enqueue(chunk + nFront + nLeft);
    modifiedRebuildQueue.Enqueue(chunk + nFront + nRight);
    modifiedRebuildQueue.Enqueue(chunk + nBack + nLeft);
    modifiedRebuildQueue.Enqueue(chunk + nBack + nRight);
    return true;
  }

  void OnDestroy(){
    shouldRenderThread.Abort();
    shouldTickThread.Abort();
    Thread.Sleep(30);
  }
  
  public void SaveAndShutDown(){
    isShuttingDown = true;
    shouldRenderThread.Abort();
    shouldTickThread.Abort();
    Thread.Sleep(30);
    foreach (Vector2Int chunkPos in chunkMap.Keys){
      chunkDataManager.UnloadChunk(chunkPos);
    }
  }

  private long TimeStamp(){
    return System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
  }
}