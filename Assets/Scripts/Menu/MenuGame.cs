using UnityEngine;

public class GameMenu : MonoBehaviour{

  public void OpenMenu(){
    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;
    this.gameObject.SetActive(true);
  }

  public void CloseMenu(){
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
    this.gameObject.SetActive(false);
  }

  public void Disconnect(){
    World.activeWorld.chunkManager.SaveAndShutDown();
    UnityEngine.SceneManagement.SceneManager.LoadScene(0);
  }
}