using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
  [Header("Movement Settings")]

  [Range(1, 15)]
  public float rotationLerpSpeed = 9;
  [Range(0, 0.5f)]
  public float speed = 0.2f;

  [Range(0, 1)]
  public float airSpeedMultiplicator = 0.2f;

  [Range(0, 1)]
  public float launchingMultiplicator = 0.1f;

  [Header("Auto Assigned Components")]
  public GameObject Model;
  public Quaternion rotation;


  private PlayerControls playerControls;
  private Vector2 direction;
  private Vector2 mousePosition;
  private bool isGrounded;
  private bool isSlowed = false;
  private bool isMoveable = true;
  private float slowMultiplicator = 1;
  private Collider playerCollider;
  private float currentSpeed;
  private Rigidbody rb;

  public void Slow(float multiplicator)
  {
    isSlowed = true;
    slowMultiplicator = multiplicator;
  }

  public void ResetSlow()
  {
    isSlowed = false;
    slowMultiplicator = 1;
  }

  public void Moveable(bool state)
  {
    isMoveable = state;
  }


  void OnEnable()
  {
    playerControls = new PlayerControls();
    playerControls.Enable();

    playerControls.CharacterControls.Movement.performed += OnMovement;
    playerControls.CharacterControls.Movement.canceled += OnMovementCanceled;

    playerControls.CharacterControls.MousePos.performed += OnMousePos;

    rb = GetComponent<Rigidbody>();
  }


  void OnDisable()
  {
    playerControls.Disable();
  }

  // Start is called before the first frame update
  void Start()
  {
    Model = transform.Find("Model").gameObject;
    playerCollider = GetComponent<CapsuleCollider>() as Collider;
  }

  // Update is called once per frame
  void Update()
  {

    rotation = GetPlayerRotation();
    Model.transform.localRotation = Quaternion.Lerp(Model.transform.localRotation, rotation, rotationLerpSpeed * Time.deltaTime);


    if (!IsGrounded())
    {
      rb.AddForce(0, -15, 0);
    }

    if (direction.magnitude > 0 && isMoveable)
    {
      Move();
    }
  }

  bool IsGrounded()
  {
    return Physics.SphereCast(transform.position, playerCollider.bounds.size.x / 2, Vector3.down, out RaycastHit hit, playerCollider.bounds.size.y * 0.6f);
  }

  void OnMovement(InputAction.CallbackContext context)
  {
    direction = context.ReadValue<Vector2>();
  }

  void OnMovementCanceled(InputAction.CallbackContext context)
  {
    direction = context.ReadValue<Vector2>();
  }

  void OnMousePos(InputAction.CallbackContext context)
  {
    mousePosition = context.ReadValue<Vector2>();
  }


  void Move()
  {
    bool blockedFront = CastPlayerToWallRay(Vector3.forward);
    bool blockedBack = CastPlayerToWallRay(Vector3.back);
    bool blockedLeft = CastPlayerToWallRay(Vector3.left);
    bool blockedRight = CastPlayerToWallRay(Vector3.right);


    float x = speed * direction.normalized.x;
    if (x > 0 && blockedRight) x = 0;
    if (x < 0 && blockedLeft) x = 0;

    float y = speed * direction.normalized.y;
    if (y > 0 && blockedFront) y = 0;
    if (y < 0 && blockedBack) y = 0;

    if (!IsGrounded())
    {
      x = x * airSpeedMultiplicator;
      y = y * airSpeedMultiplicator;
    }

    if (isSlowed)
    {
      x = x * slowMultiplicator;
      y = y * slowMultiplicator;
    }

    rb.transform.position = new Vector3(rb.transform.position.x + x, rb.transform.position.y, rb.transform.position.z + y);
  }


  bool CastPlayerToWallRay(Vector3 dir)
  {
    // todo: ignore abilities aswell
    LayerMask playersMask = LayerMask.GetMask("Players");
    Vector3 colliderBounds = GetComponent<CapsuleCollider>().bounds.size * 0.9f;
    float checkDistance = 1.2f;
    float checkGrid = 10;
    float spacing;
    float length;

    if (dir == Vector3.forward || dir == Vector3.back)
    {
      length = colliderBounds.x * checkDistance;
      spacing = length / checkGrid;
      for (int i = 1; i < checkGrid; i++)
      {
        Vector3 pos = transform.position + (dir * colliderBounds.z / 2) - new Vector3(length / 2, 0, 0) + new Vector3(spacing * i, 0, 0);
        Debug.DrawRay(pos, dir * checkDistance / 2, Color.red);
        if (Physics.Raycast(pos, dir, checkDistance / 2, ~playersMask))
        {
          return true;
        }
      }
    }
    else
    {
      length = colliderBounds.z * checkDistance;
      spacing = length / checkGrid;
      for (int i = 1; i < checkGrid; i++)
      {
        Vector3 pos = transform.position + (dir * colliderBounds.z / 2) - new Vector3(0, 0, length / 2) + new Vector3(0, 0, spacing * i);
        Debug.DrawRay(pos, dir * checkDistance / 2, Color.red);
        if (Physics.Raycast(pos, dir, checkDistance / 2, ~playersMask))
        {
          return true;
        }
      }
    }

    return false;
  }

  Quaternion GetPlayerRotation()
  {
    Vector2 mousePercent = GetMousePercent();
    return Quaternion.LookRotation(new Vector3(mousePercent.x, 0, mousePercent.y), Vector3.up);
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
}
