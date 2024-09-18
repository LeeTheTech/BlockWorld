using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SavedDataUtil{
  private static readonly DirectoryInfo saveDirectory = new(Application.persistentDataPath + "/Worlds");
  
  public static List<WorldInfo> GetAllWorlds() {
    List<WorldInfo> worldList = new List<WorldInfo>();

    if (!saveDirectory.Exists) {
      Debug.LogWarning("Save directory does not exist.");
      return worldList;
    }

    // Loop through each directory inside the saveDirectory
    foreach (DirectoryInfo worldDir in saveDirectory.GetDirectories()) {
      FileInfo worldFileInfo = new FileInfo(worldDir.FullName + "/Info.json");
      if (worldFileInfo.Exists) {
        try {
          // Read and parse the WorldInfo from the Info.json file
          string json = File.ReadAllText(worldFileInfo.FullName);
          WorldInfo worldInfo = JsonUtility.FromJson<WorldInfo>(json);
          worldList.Add(worldInfo);
        } catch (System.Exception e) {
          Debug.LogError($"Failed to load world info from {worldFileInfo.FullName}: {e.Message}");
        }
      } else {
        Debug.LogWarning($"World info file not found for world directory: {worldDir.Name}");
      }
    }

    return worldList;
  }
}
