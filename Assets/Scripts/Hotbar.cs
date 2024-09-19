using UnityEngine;
using UnityEngine.UI;

public class Hotbar : MonoBehaviour{
  [SerializeField] private int[] highlightPositions = new int[9];
  public RawImage elementPrefab;
  public int[] elements = new int[9];
  public RawImage[] elementGraphics = new RawImage[9];
  public int currentHighlighted;
  public RectTransform currentSelectedGraphic;
  
  private int[] hotbarBlocks = {
      BlockTypes.GLASS,
      BlockTypes.DIRT,
      BlockTypes.STONE,
      BlockTypes.LOG_OAK,
      BlockTypes.PLANKS_OAK,
      BlockTypes.COBBLESTONE,
      BlockTypes.WATER,
      BlockTypes.GLOWSTONE,
      BlockTypes.ICE
  };

  public void Initialize(){
    for (int i = 0; i < 9; ++i){
      RawImage elementGraphic = Instantiate(elementPrefab, elementPrefab.transform.parent, true);
      elementGraphic.rectTransform.localScale = Vector3.one;
      Vector2 anchoredPosition = elementGraphic.rectTransform.anchoredPosition;
      anchoredPosition.x = -80 + (20 * i);
      anchoredPosition.y = elementPrefab.rectTransform.anchoredPosition.y;
      elementGraphic.rectTransform.anchoredPosition = anchoredPosition;
      elementGraphics[i] = elementGraphic;
    }
    elementPrefab.gameObject.SetActive(false);
    Rebuild();
    SetHotbarElements(hotbarBlocks);
  }

  public void UpdateHotbar(){
    float scroll = Input.mouseScrollDelta.y;
    if (scroll != 0){
      int scrollDirection = (int)Mathf.Sign(scroll);
      currentHighlighted -= scrollDirection;
      if (currentHighlighted < 0) currentHighlighted += 9;
      if (currentHighlighted > 8) currentHighlighted -= 9;
      Rebuild();
    }

    const int alpha1 = (int)KeyCode.Alpha1;
    for (int i = 0; i < 9; ++i){
      if (Input.GetKeyDown((KeyCode)(alpha1 + i))){
        currentHighlighted = i;
        Rebuild();
      }
    }
  }
  
  // Method to set the block types for the hotbar
  public void SetHotbarElements(int[] blockTypes){
    for (int i = 0; i < elements.Length; i++){
      if (i < blockTypes.Length){
        elements[i] = blockTypes[i];
      } else {
        elements[i] = -1;  // Set to -1 if no block type is assigned (empty slot)
      }
    }
    Rebuild();
  }

  private void Rebuild(){
    Vector2 graphicAnchoredPosition = currentSelectedGraphic.anchoredPosition;
    graphicAnchoredPosition.x = highlightPositions[currentHighlighted];
    currentSelectedGraphic.anchoredPosition = graphicAnchoredPosition;
    TextureMapper textureMapper = GameManager.instance.textureMapper;
    for (int i = 0; i < 9; i++){
      RawImage rawImage = elementGraphics[i];
      int textureId = elements[i];
      if (textureId < 0){
        rawImage.enabled = false;
        continue;
      }

      rawImage.enabled = true;
      TextureMapper.TextureMap textureMap = textureMapper.map[(byte)textureId];
      TextureMapper.TextureMap.Face face = textureMap.front;
      Rect uvRect = new Rect(
          1.0f / 128 * face.bl.x * 16,
          1 - (1.0f / 128 * face.bl.y * 16),
          1.0f / 128 * 16,
          1.0f / 128 * 16
      );
      rawImage.color = textureMap.defaultColor;
      rawImage.uvRect = uvRect;
    }
  }

  public byte GetCurrentHighlighted(){
    return (byte)elements[currentHighlighted];
  }
}