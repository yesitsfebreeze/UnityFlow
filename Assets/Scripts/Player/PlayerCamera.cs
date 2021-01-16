using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : ReferencedMonoBehaviour
{

  static float camAngleMin = 10;
  static float camAngleMax = 65;

  static float camDistanceMin = 15;
  static float camDistanceMax = 26;


  [Header("Auto Assigned Components")]
  public GameObject model;
  public GameObject boomAnchor;
  public GameObject arm;
  public Camera cam;


  [Header("Camera Settings")]

  [Range(10, 70)]
  public float camAngle = 70;
  [Range(15, 26)]
  public float camDistance = 26;
  [Range(0.01f, 100f)]
  public float camLerpSpeed = 11f;
  [Range(0.01f, 100f)]
  public float camZoomLerpSpeed = 5f;
  [Range(0.05f, 1f)]
  public float cameraZoomStep = 0.1f;
  [Range(0, 30)]
  public float camOffsetMaxLeftRight = 27;
  [Range(0, 30)]
  public float camOffsetMaxFront = 11;
  [Range(0, 30)]
  public float camOffsetMaxBack = 16;


  private PlayerControls playerControls;
  private Vector2 mousePosition;
  private float scrollWheelValue;
  private Vector3 cameraArmPos;
  private Quaternion cameraRot;
  private float lastCamAngle;
  private float lastCamDistance;


  public Quaternion GetCameraAngle()
  {
    return Quaternion.LookRotation(boomAnchor.transform.position - arm.transform.position);
  }

  void OnEnable()
  {
    playerControls = new PlayerControls();
    playerControls.Enable();

    playerControls.CharacterControls.MousePos.performed += OnMousePos;
    playerControls.CharacterControls.Scrollwheel.performed += OnScrollWheel;
    playerControls.CharacterControls.Scrollwheel.canceled += OnScrollWheelCancel;
  }

  void Start()
  {
    boomAnchor = gameObject.transform.Find("CameraBoomAnchor").gameObject;
    arm = boomAnchor.transform.Find("CameraBoomArm").gameObject;
    cam = boomAnchor.GetComponentInChildren<Camera>();
  }

  void Update()

  {
    SetCameraPosition();

    arm.transform.localPosition = Vector3.Lerp(arm.transform.localPosition, cameraArmPos, camZoomLerpSpeed * Time.deltaTime);
    cam.transform.localRotation = Quaternion.Lerp(cam.transform.localRotation, cameraRot, camZoomLerpSpeed * Time.deltaTime);
    boomAnchor.transform.localPosition = Vector3.Lerp(boomAnchor.transform.localPosition, GetboomAnchorOffset(), camLerpSpeed * Time.deltaTime);

    OnZoom();
  }

  void OnMousePos(InputAction.CallbackContext context)
  {
    mousePosition = context.ReadValue<Vector2>();
  }

  void OnScrollWheel(InputAction.CallbackContext context)
  {
    scrollWheelValue = context.ReadValue<float>();
  }

  void OnScrollWheelCancel(InputAction.CallbackContext context)
  {
    scrollWheelValue = context.ReadValue<float>();
  }

  void OnZoom()
  {

    float distanceRange = camDistanceMax - camDistanceMin;
    float angleRange = camAngleMax - camAngleMin;

    if (scrollWheelValue > 0)
    {
      if (camAngle > camAngleMin)
      {
        camDistance -= distanceRange * cameraZoomStep;
        camAngle -= angleRange * cameraZoomStep;
      }
    }
    else if (scrollWheelValue < 0)
    {
      if (camAngle < camAngleMax)
      {
        camDistance += distanceRange * cameraZoomStep;
        camAngle += angleRange * cameraZoomStep;
      }
    }

  }


  Vector3 GetboomAnchorOffset()
  {
    Vector2 mousePercent = GetMousePercent();

    Vector3 targetOffset = new Vector3();
    targetOffset.x += camOffsetMaxLeftRight * mousePercent.x;
    if (mousePercent.y > 0)
    {
      targetOffset.z += camOffsetMaxFront * mousePercent.y;
    }
    else
    {

      targetOffset.z += camOffsetMaxBack * mousePercent.y;
    }

    return targetOffset;
  }

  void SetCameraPosition()
  {
    if (lastCamAngle == camAngle && lastCamDistance == camDistance) return;

    cameraArmPos = GetVectorFromAngleAndDistance(new Vector3(camAngle, 0, 0), -camDistance);

    cameraRot = new Quaternion();
    cameraRot.eulerAngles = new Vector3(camAngle, 0, 0);

    lastCamAngle = camAngle;
    lastCamDistance = camDistance;
  }

  Vector2 GetMousePercent()
  {
    float x = (float)(mousePosition.x / Screen.width - 0.5) * 2;
    float y = (float)(mousePosition.y / Screen.height - 0.5) * 2;

    if (x > 1) x = 1;
    if (x < -1) x = -1;

    if (y > 1) y = 1;
    if (y < -1) y = -1;

    return new Vector2(x, y);
  }

  Vector3 GetVectorFromAngleAndDistance(Vector3 angle, float distance)
  {
    return Quaternion.Euler(angle.x, angle.y, angle.z) * (Vector3.forward * distance);
  }
}
