using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Player : MonoBehaviour{
  const int playerLayer = 8;
  private Vector3 euler;
  private Quaternion cameraRotation;
  private Vector3 cameraPosition;
  private long lastPressedSpace;
  public State state;
  private bool running;
  private float wobble;
  private float wobbleIntensity;
  private PostProcessVolume waterPostProcessVolume;
  private bool isUnderWater;
  public Setup setup;

  private Direction playerDirection;

  public enum State{
    Survival = 0,
    Creative_Walking = 1,
    Creative_Flying = 2,
    Spectator = 3
  }

  [System.Serializable]
  public class Setup{
    public World world;
    public Camera mainCamera;
    public float walkSpeed, walkForce, fallSpeed, fallForce, jumpVelocity, runSpeed, runForce;
    public float fieldOfView = 60;
    public Rigidbody myRigidbody;
    public GameObject highlightPrefab;
  }

  void Start(){
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
    cameraRotation = Quaternion.Euler(euler);
    cameraPosition = transform.position + new Vector3(0, .5f, 0);
    
    PostProcessVolume[] postProcessVolumes = setup.mainCamera.GetComponents<PostProcessVolume>();
    waterPostProcessVolume = postProcessVolumes[1];
  }

  void Update(){
    setup.myRigidbody.isKinematic = (GameManager.instance.isInStartup || state == State.Spectator);
    
    if (Cursor.lockState == CursorLockMode.None) return;

    Vector2 movement = new Vector2();
    movement.x += Input.GetKey(KeyCode.D) ? 1 : 0;
    movement.x -= Input.GetKey(KeyCode.A) ? 1 : 0;
    movement.y += Input.GetKey(KeyCode.W) ? 1 : 0;
    movement.y -= Input.GetKey(KeyCode.S) ? 1 : 0;

    if (Input.GetKeyDown(KeyCode.LeftControl)){
      if (movement != Vector2.zero) running = true;
    }

    if (movement == Vector2.zero) running = false;
    float wobbleTargetIntensity = movement == Vector2.zero ? 0 : (running ? 2f : 1f);
    if (state > (State)1) wobbleTargetIntensity = 0;
    wobbleIntensity = Mathf.Lerp(wobbleIntensity, wobbleTargetIntensity, Time.deltaTime * 16f);

    if (state < State.Spectator){
      Movement(movement, running);
    }
    else{
      SpectatorMovement(movement, running);
    }
    
    CheckUnderWater();
    CameraUpdate();
    BlockPlacement();
    CheckPlayerDetails();
  }

  private long TimeStamp(){
    return System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
  }

  private void CheckPlayerDetails(){
    // Player Direction
    Vector3 forward = setup.mainCamera.transform.forward;
    forward.y = 0;
    forward.Normalize();
    playerDirection = GetFacingDirection(forward);
    setup.world.debugDirectionText.text = $"Direction : {playerDirection.ToString()}";
  }

  private void CheckUnderWater(){
    if (IsInWater()){
      if (!isUnderWater){
        waterPostProcessVolume.enabled = true;
        isUnderWater = true;
      }
    }
    else{
      if (isUnderWater){
        waterPostProcessVolume.enabled = false;
        isUnderWater = false;
      }
    }
  }

  private void CameraUpdate(){
    Camera cam = setup.mainCamera;
    euler.x -= Input.GetAxis("Mouse Y") * 2f;
    euler.y += Input.GetAxis("Mouse X") * 2f;
    euler.x = Mathf.Clamp(euler.x, -89.99f, 89.99f);
    cameraRotation = Quaternion.Euler(euler);
    Vector3 camTargetPosition = transform.position + new Vector3(0, .5f, 0);
    cameraPosition = Vector3.Lerp(
        cameraPosition,
        camTargetPosition,
        Time.deltaTime * 20f
    );

    wobble += Time.deltaTime * 8f * (running ? 1.3f : 1f);
    cam.transform.rotation = cameraRotation;
    cam.transform.position = cameraPosition;

    cam.transform.Rotate(Vector3.forward, Mathf.Sin(wobble) * 0.2f * wobbleIntensity);
    cam.transform.Rotate(Vector3.right, Mathf.Sin(wobble * 2f) * 0.3f * wobbleIntensity);
    cam.transform.Rotate(Vector3.up, -Mathf.Sin(wobble) * 0.2f * wobbleIntensity);
    cam.transform.position += (cam.transform.up * (Mathf.Sin(wobble * 2f) * 0.05f * wobbleIntensity));
    cam.transform.position += (cam.transform.right * (Mathf.Sin(wobble) * 0.05f * wobbleIntensity));

    float fov = setup.fieldOfView + (running ? 10 : 0);
    //if (movement == Vector2.zero) fov = Input.GetKey(KeyCode.Tab) ? 10 : fov;
    cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fov, Time.deltaTime * 8f);
    if (Input.GetKey(KeyCode.Tab)) cam.fieldOfView = 20;
    if (Input.GetKeyUp(KeyCode.Tab)) cam.fieldOfView = fov;
  }

  private void Movement(Vector2 movement, bool isRunning){
    float moveForce = isRunning ? setup.runForce : setup.walkForce;
    float moveSpeed = isRunning ? setup.runSpeed : setup.walkSpeed;
    
    Vector3 forward = setup.mainCamera.transform.forward;
    forward.y = 0;
    forward.Normalize();

    Vector3 right = setup.mainCamera.transform.right;
    right.y = 0;
    right.Normalize();

    Vector3 stillVelocity = setup.myRigidbody.velocity;
    stillVelocity.x = 0;
    stillVelocity.z = 0;
    setup.myRigidbody.velocity = Vector3.Lerp(setup.myRigidbody.velocity, stillVelocity, Time.deltaTime * 8f);

    setup.myRigidbody.AddForce(forward * (movement.y * (moveForce * Time.deltaTime)));
    setup.myRigidbody.AddForce(right * (movement.x * (moveForce * Time.deltaTime)));
    if (state < (State)2){
      setup.myRigidbody.AddForce(Vector3.down * (setup.fallForce * Time.deltaTime));
    }

    Vector3 velocityWalk = setup.myRigidbody.velocity;
    velocityWalk.y = 0;
    Vector3 velocityFall = setup.myRigidbody.velocity;
    velocityFall.x = 0;
    velocityFall.z = 0;
    if (velocityWalk.magnitude > moveSpeed){
      velocityWalk = velocityWalk.normalized * moveSpeed;
    }

    if (velocityFall.magnitude > setup.fallSpeed){
      velocityFall = velocityFall.normalized * setup.fallSpeed;
    }

    Vector3 targetVelocity = velocityWalk + velocityFall;

    if (Input.GetKeyDown(KeyCode.Space)){
      long timestamp = TimeStamp();
      if (timestamp < lastPressedSpace + 500){
        if (state == State.Creative_Walking){
          state = State.Creative_Flying;
        }
        else if (state == State.Creative_Flying){
          state = State.Creative_Walking;
        }

        lastPressedSpace = 0;
      }
      else{
        lastPressedSpace = TimeStamp();
        targetVelocity.y = setup.jumpVelocity;
      }
    }

    if (state > (State)1){
      targetVelocity.y = 0;
      if (Input.GetKey(KeyCode.Space)){
        targetVelocity.y += 8;
      }

      if (Input.GetKey(KeyCode.LeftShift)){
        targetVelocity.y -= 8;
      }
    }

    //myRigidbody.velocity = Vector3.Lerp(myRigidbody.velocity, targetVelocity, Time.deltaTime * 8f);
    setup.myRigidbody.velocity = targetVelocity;
  }

  private void BlockPlacement(){
    const int layerMask = ~(1 << playerLayer);
    if (Physics.Raycast(setup.mainCamera.transform.position, setup.mainCamera.transform.forward, out var hitInfo, 5, layerMask)){
      //Debug.Log(hitInfo.collider.gameObject.name);
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
        byte blockToReplace = setup.world.GetBlock(removeBlock.x, removeBlock.y, removeBlock.z);
        byte blockStateToReplace = setup.world.GetBlockState(removeBlock.x, removeBlock.y, removeBlock.z);
        if (setup.world.Modify(removeBlock.x, removeBlock.y, removeBlock.z, BlockTypes.AIR, blockStateToReplace)){
          AudioManager.instance.dig.Play(BlockTypes.digSound[blockToReplace], removeBlock);
          place = false;
        }
      }

      if (place){
        byte block = UI.instance.hotbar.GetCurrentHighlighted();
        if (setup.world.Modify(placeBlock.x, placeBlock.y, placeBlock.z, block, BlockStateUtil.SetOrientation(0, (byte)playerDirection))){
          AudioManager.instance.dig.Play(BlockTypes.digSound[block], removeBlock);
        }
      }

      setup.highlightPrefab.transform.position = removeBlock + new Vector3(.5f, .5f, .5f);
      setup.highlightPrefab.SetActive(true);
    }
    else{
      setup.highlightPrefab.SetActive(false);
    }
  }

  private void SpectatorMovement(Vector2 movement, bool isRunning){
    float moveSpeed = isRunning ? 16 : 8;

    Vector3 forward = setup.mainCamera.transform.forward;
    forward.y = 0;
    forward.Normalize();

    Vector3 right = setup.mainCamera.transform.right;
    right.y = 0;
    right.Normalize();

    float altitude = 0;

    if (Input.GetKey(KeyCode.Space)){
      altitude += 8;
    }

    if (Input.GetKey(KeyCode.LeftShift)){
      altitude -= 8;
    }

    Vector3 position = transform.position;
    position += forward * (movement.y * Time.deltaTime * moveSpeed);
    position += right * (movement.x * Time.deltaTime * moveSpeed);
    position += Vector3.up * (altitude * Time.deltaTime);
    transform.position = position;
  }
  
  private bool IsInWater(){
    Vector3 playerPosition = transform.position;
    Vector3Int blockPosition = new Vector3Int(
        Mathf.FloorToInt(playerPosition.x),
        Mathf.FloorToInt(playerPosition.y),
        Mathf.FloorToInt(playerPosition.z)
    );
    if (setup.world.IsBlockAvailable(blockPosition.x, blockPosition.y, blockPosition.z)){
      return setup.world.GetBlock(blockPosition.x, blockPosition.y, blockPosition.z) == BlockTypes.WATER;
    }
    return false;
  }

  private Direction GetFacingDirection(Vector3 forward){
    // Define cardinal directions as vectors
    Vector3 north = Vector3.forward; // Assuming forward is north
    Vector3 east = Vector3.right; // Assuming right is east
    Vector3 south = Vector3.back; // Assuming back is south
    Vector3 west = Vector3.left; // Assuming left is west

    // Calculate the angle between the forward vector and each cardinal direction
    float angleToNorth = Vector3.Angle(forward, north);
    float angleToEast = Vector3.Angle(forward, east);
    float angleToSouth = Vector3.Angle(forward, south);
    float angleToWest = Vector3.Angle(forward, west);

    // Find the minimum angle and return the corresponding direction
    float minAngle = Mathf.Min(angleToNorth, angleToEast, angleToSouth, angleToWest);

    if (minAngle == angleToNorth) return Direction.NORTH;
    if (minAngle == angleToEast) return Direction.EAST;
    if (minAngle == angleToSouth) return Direction.SOUTH;
    if (minAngle == angleToWest) return Direction.WEST;

    return Direction.NORTH;
  }
}