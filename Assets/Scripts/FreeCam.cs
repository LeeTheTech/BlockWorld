using UnityEngine;

public class FreeCam : MonoBehaviour{
  public Vector3 euler;
  public World world;
  public GameObject highlightPrefab;

  void Start(){
    Cursor.lockState = CursorLockMode.Locked;
  }

  // Update is called once per frame
  void Update(){
    if (GameManager.instance.isInStartup) return;
    if (Input.GetKeyDown(KeyCode.F1)){
      Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
    }

    euler.x -= Input.GetAxis("Mouse Y") * 2f;
    euler.y += Input.GetAxis("Mouse X") * 2f;
    euler.x = Mathf.Clamp(euler.x, -89, 89);
    transform.rotation = Quaternion.Euler(euler);

    Vector2 movement = new Vector2();
    movement.x += Input.GetKey(KeyCode.D) ? 1 : 0;
    movement.x -= Input.GetKey(KeyCode.A) ? 1 : 0;
    movement.y += Input.GetKey(KeyCode.W) ? 1 : 0;
    movement.y -= Input.GetKey(KeyCode.S) ? 1 : 0;

    float speed = Input.GetKey(KeyCode.LeftShift) ? 4 : 1;

    Transform camTransform = transform;
    Vector3 position = camTransform.position;
    position += camTransform.forward * (movement.y * speed);
    position += camTransform.right * (movement.x * speed);
    camTransform.position = position;


    if (Physics.Raycast(transform.position, transform.forward, out var hitInfo)){
      //Debug.Log(hitInfo.point);
      Vector3 inCube = hitInfo.point - (hitInfo.normal * 0.5f);
      Vector3Int removeBlock = new Vector3Int(
          Mathf.FloorToInt(inCube.x),
          Mathf.FloorToInt(inCube.y),
          Mathf.FloorToInt(inCube.z)
      );
      Vector3 fromCube = hitInfo.point + (hitInfo.normal * 0.5f);
      Vector3Int placeBlock = new Vector3Int(
          Mathf.FloorToInt(fromCube.x),
          Mathf.FloorToInt(fromCube.y),
          Mathf.FloorToInt(fromCube.z)
      );
      bool remove = false;
      bool place = false;
      remove |= Input.GetKeyDown(KeyCode.Mouse0);
      remove |= (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.Mouse0));

      place |= Input.GetKeyDown(KeyCode.Mouse1);
      place |= (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.Mouse1));

      if (remove){
        world.Modify(removeBlock.x, removeBlock.y, removeBlock.z, BlockTypes.AIR, BlockStateUtil.SetOrientation(0, 0));
      }

      if (place){
        world.Modify(placeBlock.x, placeBlock.y, placeBlock.z, UI.instance.hotbar.GetCurrentHighlighted(), BlockStateUtil.SetOrientation(0, 0));
      }

      highlightPrefab.transform.position = removeBlock + new Vector3(.5f, .5f, .5f);
      highlightPrefab.SetActive(true);
    }
    else{
      highlightPrefab.SetActive(false);
    }
  }
}