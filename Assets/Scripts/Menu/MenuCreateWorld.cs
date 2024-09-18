using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class MenuCreateWorld : MonoBehaviour{
  [SerializeField] public MenuWorld worldMenu;
  [SerializeField] public TextMeshProUGUI nameInputField;
  [SerializeField] public TextMeshProUGUI seedInputField;
  private const int sceneToLoad = 1;

  public void OpenMenu(){
    this.worldMenu.gameObject.SetActive(false);
    this.gameObject.SetActive(true);
  }
    
  public void CloseMenu(){
    this.gameObject.SetActive(false);
    this.worldMenu.gameObject.SetActive(true);
  }

  public void CreateWorld(){
    if (nameInputField.text == string.Empty) return;

    string seedText = "";
    MatchCollection matches = Regex.Matches(seedInputField.text, @"\d+");
    foreach (object match in matches){
      seedText += match;
    }

    // If empty set to 0 for random seed
    if (seedText == string.Empty) seedText = "0";

    // Limit seed text to 10 digits
    if (seedText.Length > 10) seedText = seedText[..10];

    // Safely parse the seed value and check for overflow
    if (long.TryParse(seedText, out long seedLong) && seedLong <= int.MaxValue){
      int seed = (int)seedLong;
      WorldInfoStorage.worldInfo = new WorldInfo(nameInputField.text, seed);
      UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
    }
    else{
      WorldInfoStorage.worldInfo = new WorldInfo(nameInputField.text, int.MaxValue);
      UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
    }
  }
}
