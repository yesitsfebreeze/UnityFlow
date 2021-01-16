using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : ReferencedMonoBehaviour
{

  public Vector3 mousePosition;
  public Vector3 mouseWorldPosition;


  private PlayerControls playerControls;

  void OnEnable()
  {
    playerControls = new PlayerControls();
    playerControls.Enable();

    playerControls.CharacterControls.MousePos.performed += OnMousePos;
  }

  void OnDisable()
  {
    playerControls.Disable();
  }

  void OnMousePos(InputAction.CallbackContext context)
  {
    mousePosition = context.ReadValue<Vector2>();


    Ray ray = Camera.main.ScreenPointToRay(new Vector3(mousePosition.x, mousePosition.y, 0));
    RaycastHit hitData;

    if (Physics.Raycast(ray, out hitData, 1000))
    {
      mouseWorldPosition = hitData.point;
    }
  }

}

