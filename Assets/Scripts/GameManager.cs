using UnityEngine;

public class GameManager : MonoBehaviour{
  public static GameManager instance{ get; private set; }
  public bool showLoadingScreen = true;
  public World world;
  public GameSettings gameSettings;
  public UI ui;
  private SaveDataManager saveDataManager;
  public TextureMapper textureMapper;
  public AudioManager audioManager;
  public bool isInStartup;
  public Texture2D textures;
  public Texture2D animatedTextures;
  public Camera screenshotCamera;
  public Texture2D latestScreenshot;

  private void Start(){
    instance = this;
    Initialize();
    BlockTypes.Initialize();
    textureMapper = new TextureMapper();

    if (AudioManager.instance == null){
      audioManager.Initialize();
    }

    audioManager = AudioManager.instance;

    CreateTextures();
    Structure.Initialize();
    InitializeWorld(WorldInfoStorage.worldInfo);
    ui.Initialize();
    
    Shader.SetGlobalFloat("_MinLightLevel", gameSettings.minimumLightLevel);
#if !UNITY_EDITOR
		showLoadingScreen = true;
#endif
    if (showLoadingScreen){
      isInStartup = true;
      world.chunkManager.isInStartup = true;
      ui.loadingScreen.gameObject.SetActive(true);
    }
    
    InvokeRepeating(nameof(TakeWorldPreviewScreenshot), 10f, 300f);
  }

  private void Update(){
    if (!audioManager.IsPlayingMusic()){
      if (isInStartup){
        audioManager.PlayNewPlaylist(audioManager.music.menu.clips);
      }
      else{
        audioManager.PlayNewPlaylist(audioManager.music.game.clips);
      }
    }
    else{
      if (!isInStartup){
        if (audioManager.musicPlaylist != audioManager.music.game.clips){
          audioManager.PlayNewPlaylist(audioManager.music.game.clips);
        }
      }
    }

    if (isInStartup){
      if (world.chunkManager.StartupFinished()){
        world.chunkManager.isInStartup = false;
        isInStartup = false;
        ui.loadingScreen.gameObject.SetActive(false);
        audioManager.PlayNewPlaylist(audioManager.music.game.clips);
        System.GC.Collect();
      }
    }

    ui.UpdateUI();
    TakeScreenshot();
    DebugStuff();
  }

  private void Initialize(){
    saveDataManager = new SaveDataManager();
  }

  public void InitializeWorld(WorldInfo worldInfo){
    worldInfo = saveDataManager.Initialize(worldInfo);
    world.Initialize(worldInfo);
  }

  private void CreateTextures(){
    Texture2D temp = new Texture2D(textures.width, textures.height, TextureFormat.ARGB32, 5, false);
    temp.SetPixels(textures.GetPixels());
    temp.filterMode = FilterMode.Point;
    temp.Apply();
    textures = temp;
    Shader.SetGlobalTexture("_BlockTextures", textures);
    
    Texture2D animatedTemp = new Texture2D(animatedTextures.width, animatedTextures.height, TextureFormat.ARGB32, 5, false);
    animatedTemp.SetPixels(animatedTextures.GetPixels());
    animatedTemp.filterMode = FilterMode.Point;
    animatedTemp.Apply();
    animatedTextures = animatedTemp;
    Shader.SetGlobalTexture("_AnimatedBlockTextures", animatedTextures);
  }

  private void DebugStuff(){
    //360 screenshot
    if (Input.GetKeyDown(KeyCode.F4)){
      RenderTexture cubemap = new RenderTexture(4096, 4096, 0, RenderTextureFormat.ARGB32);
      cubemap.dimension = UnityEngine.Rendering.TextureDimension.Cube;
      cubemap.Create();
      screenshotCamera.transform.position = world.mainCamera.transform.position;
      screenshotCamera.RenderToCubemap(cubemap);

      RenderTexture equirect = new RenderTexture(4096, 2048, 0, RenderTextureFormat.ARGB32);
      Texture2D texture = new Texture2D(4096, 2048, TextureFormat.ARGB32, false);
      cubemap.ConvertToEquirect(equirect, Camera.MonoOrStereoscopicEye.Mono);
      RenderTexture temp = RenderTexture.active;
      RenderTexture.active = equirect;
      texture.ReadPixels(new Rect(0, 0, equirect.width, equirect.height), 0, 0);
      RenderTexture.active = temp;
      texture.Apply();
      latestScreenshot = texture;
      System.IO.FileInfo file = new System.IO.FileInfo(Application.persistentDataPath + "/" + TimeStamp().ToString() + ".png");
      System.IO.File.WriteAllBytes(file.FullName, texture.EncodeToPNG());
    }
  }

  private void TakeScreenshot(){
    if (Input.GetKeyDown(KeyCode.F2)){
      // Define the path to the "Screenshots" folder
      string screenshotsFolder = Application.persistentDataPath + "/Screenshots/";

      // Check if the "Screenshots" folder exists, and create it if it doesn't
      if (!System.IO.Directory.Exists(screenshotsFolder)) {
        System.IO.Directory.CreateDirectory(screenshotsFolder);
      }

      // Create the file path for the screenshot
      System.IO.FileInfo file = new System.IO.FileInfo(screenshotsFolder + TimeStamp() + ".png");

      // Call your method to take and save the screenshot
      TakeScreenshot(file);
    }
  }

  private void TakeWorldPreviewScreenshot(){
    //TODO maybe make async
    System.IO.FileInfo file = new System.IO.FileInfo(Application.persistentDataPath + "/Worlds/" + world.info.name + "/WorldPreview.png");
    TakeScreenshot(file);
  }

  private void TakeScreenshot(System.IO.FileInfo file) {
    // Create a RenderTexture with 1920x1080 resolution
    RenderTexture renderTexture = new RenderTexture(1920, 1080, 24);
    
    // Assign the render texture to the camera's target texture
    screenshotCamera.targetTexture = renderTexture;

    // Position the screenshot camera to match the main camera if needed
    screenshotCamera.transform.position = world.mainCamera.transform.position;

    // Render the screenshot camera to the render texture
    screenshotCamera.Render();

    // Create a new Texture2D to hold the screenshot data
    Texture2D screenshot = new Texture2D(1920, 1080, TextureFormat.ARGB32, false);

    // Set the active render texture (so that we can read the pixels from it)
    RenderTexture.active = renderTexture;

    // Read the pixels from the render texture and store them in the Texture2D
    screenshot.ReadPixels(new Rect(0, 0, 1920, 1080), 0, 0);
    screenshot.Apply();

    // Save the Texture2D to a PNG file
    System.IO.File.WriteAllBytes(file.FullName, screenshot.EncodeToPNG());

    // Clean up: set the camera's target texture to null and reset the active render texture
    screenshotCamera.targetTexture = null;
    RenderTexture.active = null;

    // Destroy the render texture to free up memory
    Destroy(renderTexture);
  }

  private static long TimeStamp(){
    return System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
  }
}