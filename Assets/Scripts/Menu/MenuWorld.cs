using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuWorld : MonoBehaviour{
  [SerializeField] public MainMenu mainMenu;
  [SerializeField] public GameObject savedWorldPrefab;
  [SerializeField] public GridLayoutGroup savedWorldGrid;
  private const int sceneToLoad = 1;
  private List<WorldInfo> worldInfoList;

  public void OpenMenu(){
    LoadData();
    this.mainMenu.CloseMenu();
    this.gameObject.SetActive(true);
  }

  public void CloseMenu(){
    this.gameObject.SetActive(false);
    this.mainMenu.OpenMenu();
  }

  private void LoadData(){
    worldInfoList = SavedDataUtil.GetAllWorlds();
    if (worldInfoList.Count < 1) return;
    foreach (Transform child in savedWorldGrid.transform) Destroy(child.gameObject);
    foreach (WorldInfo worldInfo in worldInfoList){
      CreateSaveWorldObject(worldInfo.name, worldInfo.seed);
    }
    AdjustGridHeight();
  }

  private void CreateSaveWorldObject(string worldName, int seed){
    GameObject savedWorldPanel = Instantiate(savedWorldPrefab, savedWorldGrid.transform);
    // Button
    Button btn = savedWorldPanel.GetComponent<Button>();
    btn.onClick.AddListener(() => LoadSavedWorld(worldName, seed));
    // Text
    TextMeshProUGUI txt = savedWorldPanel.GetComponentInChildren<TextMeshProUGUI>();
    txt.text = worldName;
  }

  private static void LoadSavedWorld(string worldName, int seed){
    WorldInfoStorage.worldInfo = new WorldInfo(worldName, seed);
    UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
  }

  private void AdjustGridHeight(){
    // Get the GridLayoutGroup's RectTransform
    RectTransform gridRectTransform = savedWorldGrid.GetComponent<RectTransform>();

    // Get the cell size, spacing, and padding from the GridLayoutGroup
    Vector2 cellSize = savedWorldGrid.cellSize;
    Vector2 spacing = savedWorldGrid.spacing;
    RectOffset padding = savedWorldGrid.padding;
    
    // Get the number of columns set in the GridLayoutGroup's constraint
    int columns = savedWorldGrid.constraintCount;

    // Calculate the number of rows needed to display the current count of items
    int rows = Mathf.CeilToInt((float)worldInfoList.Count * 2 / columns);

    // Calculate the new height based on the number of rows, cell size, spacing, and padding
    float newHeight = rows * cellSize.y + (rows - 1) * spacing.y + padding.top + padding.bottom;

    // Adjust the grid's height
    gridRectTransform.sizeDelta = new Vector2(gridRectTransform.sizeDelta.x, newHeight);
  }
}