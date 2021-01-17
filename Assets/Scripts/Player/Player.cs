using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : ReferencedReferenceAwareMonoBehaviour
{

  public Vector3 mousePosition;
  public Vector3 mouseWorldPosition;
  public UnitUI unitUI;
  public bool isCasting = false;

  private PlayerControls playerControls;
  private GameObject unitUIGO;
  private HealthState health;


  public void SetCasting(bool state)
  {
    isCasting = state;
  }

  void OnEnable()
  {
    playerControls = new PlayerControls();
    playerControls.Enable();

    playerControls.CharacterControls.MousePos.performed += OnMousePos;

    AddUnitUI();

    health = GetComponent<HealthState>() as HealthState;
    unitUI.SetHealth(health);
  }

  void AddUnitUI()
  {
    unitUIGO = Instantiate(references.defaultReferences.UnitUI, transform.position, Quaternion.identity) as GameObject;
    unitUIGO.transform.SetParent(transform);
    unitUI = unitUIGO.GetComponent<UnitUI>() as UnitUI;
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

