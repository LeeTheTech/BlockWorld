using System.IO;
using UnityEditor;
using UnityEngine;

public class MenuItems : MonoBehaviour{
  [MenuItem("BlockWorld/Open Save Folder")]
  static void OpenSaveFolder(){
    EditorUtility.RevealInFinder(Application.persistentDataPath + "/Worlds");
  }

  [MenuItem("BlockWorld/Open Build Folder")]
  static void OpenBuildFolder(){
    EditorUtility.RevealInFinder(new DirectoryInfo(Application.dataPath).Parent.FullName + "/Build");
  }

  [MenuItem("BlockWorld/Run Latest Build")]
  static void RunLatestBuild(){
    string file = new DirectoryInfo(Application.dataPath).Parent.FullName + "/Build/Block-World.exe";
    System.Diagnostics.Process process = new System.Diagnostics.Process();
    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
    startInfo.FileName = file;
    process.StartInfo = startInfo;
    process.Start();
  }
}