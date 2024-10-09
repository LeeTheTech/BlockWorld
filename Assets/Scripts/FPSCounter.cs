using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour{
  public TextMeshProUGUI fpsDisplay; // Reference to a UI Text element to display FPS
  private float deltaTime = 0.0f;

  void Update(){
    // Calculate the time taken to render each frame
    deltaTime += (Time.deltaTime - deltaTime) * 0.1f;

    // Calculate FPS
    float fps = 1.0f / deltaTime;

    // Display the FPS
    if (fpsDisplay != null){
      fpsDisplay.text = "FPS: " + Mathf.Ceil(fps);
    }
  }
}