using System.Reflection;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Launcher : ReferenceAwareMonoBehaviour
{

  public float[] cooldowns = new float[3];
  public float rechargeCooldown;

  private PlayerControls playerControls;
  private IngameUI UI;
  public SO_LauncherSettings settings;
  public SO_NadeDefinition[] selectedNades;
  private IEnumerator launchRoutine;
  private bool launchRoutineStarted = false;
  private Player player;
  private bool isLaunchPressed = false;
  private int launchSlot;


  void OnEnable()
  {
    selectedNades = new SO_NadeDefinition[3];

    UI = references.Get("IngameUI") as IngameUI;

    playerControls = new PlayerControls();
    playerControls.Enable();

    playerControls.CharacterControls.Cancel.performed += ctx => OnCancelPress(ctx);
    playerControls.CharacterControls.Cancel.canceled += ctx => OnCancelRelease(ctx);

    playerControls.CharacterControls.LaunchGrenadeOne.performed += ctx => OnGrenadeLaunchPress(ctx, 0);
    playerControls.CharacterControls.LaunchGrenadeTwo.performed += ctx => OnGrenadeLaunchPress(ctx, 1);
    playerControls.CharacterControls.LaunchGrenadeThree.performed += ctx => OnGrenadeLaunchPress(ctx, 2);

    playerControls.CharacterControls.LaunchGrenadeOne.canceled += ctx => OnGrenadeLaunchRelease(ctx, 0);
    playerControls.CharacterControls.LaunchGrenadeTwo.canceled += ctx => OnGrenadeLaunchRelease(ctx, 1);
    playerControls.CharacterControls.LaunchGrenadeThree.canceled += ctx => OnGrenadeLaunchRelease(ctx, 2);
  }


  void OnDisable()
  {
    playerControls.Disable();
  }

  // Start is called before the first frame update
  void Start()
  {
    // todo: do that via UI
    SetSlot(0, 0);
    SetSlot(1, 1);
    SetSlot(2, 2);
  }

  // Update is called once per frame
  void Update()
  {

    if (isLaunchPressed)
    {
      if (player == null) player = references.Get("Player") as Player;
      if (player.isCasting) return;
      if (rechargeCooldown > 0) return;
      if (cooldowns[launchSlot] > 0) return;
      isLaunchPressed = false;

      launchRoutine = LaunchNade(launchSlot);
      StartCoroutine(launchRoutine);
    }


    if (rechargeCooldown > 0) rechargeCooldown -= Time.deltaTime;
    if (rechargeCooldown < 0)
    {
      player.SetCasting(false);
      rechargeCooldown = 0;
    }

    int slot = 0;
    foreach (float cd in cooldowns)
    {
      if (cooldowns[slot] > 0) cooldowns[slot] -= Time.deltaTime;
      if (cooldowns[slot] < 0) cooldowns[slot] = 0;

      if (cooldowns[slot] == 0 && rechargeCooldown > 0)
      {
        UI.UpdateNadeCooldown(slot, rechargeCooldown, settings.RechargeTime);
      }
      else
      {
        UI.UpdateNadeCooldown(slot, cooldowns[slot], settings.Cooldowns[slot]);
      }

      slot++;
    }
  }

  public void SetSlot(int slot, int nade)
  {
    SO_NadeDefinition definition = settings.Nades[nade];

    UI.AddNadeCooldown(definition, nade, settings.KeyBinds[slot]);
    selectedNades[slot] = definition;
  }

  public SO_NadeDefinition GetNade(int slot)
  {
    return selectedNades[slot];
  }

  void OnCancelPress(InputAction.CallbackContext context)
  {
    CancelLaunchRoutine();
  }

  void OnCancelRelease(InputAction.CallbackContext context) { }


  void OnGrenadeLaunchPress(InputAction.CallbackContext context, int slot)
  {
    isLaunchPressed = true;
    launchSlot = slot;
  }

  void OnGrenadeLaunchRelease(InputAction.CallbackContext context, int slot)
  {
    isLaunchPressed = false;
    launchSlot = -1;
  }

  void CancelLaunchRoutine()
  {
    if (!launchRoutineStarted) return;

    Movement movement = GetComponent<Movement>() as Movement;
    rechargeCooldown = settings.RechargeTime;
    StopCoroutine(launchRoutine);
    launchRoutineStarted = false;
    movement.ResetSlow();
  }

  IEnumerator LaunchNade(int slot)
  {
    if (player == null) player = references.Get("Player") as Player;
    player.SetCasting(true);

    launchRoutineStarted = true;
    Movement movement = GetComponent<Movement>() as Movement;

    movement.Slow(settings.Slowdown);
    player.unitUI.SetCastTime(settings.LaunchTimes[slot]);
    yield return new WaitForSeconds(settings.LaunchTimes[slot]);

    movement.ResetSlow();

    cooldowns[slot] = settings.Cooldowns[slot];
    rechargeCooldown = settings.RechargeTime;

    SO_NadeDefinition selectedNade = GetNade(slot);

    Vector3 from = transform.position;
    Vector3 to = player.mouseWorldPosition;

    GameObject nadePrefab = settings.NadePrefab;
    GameObject go = Instantiate(nadePrefab, from, Quaternion.identity) as GameObject;
    Nade nade = go.GetComponent<Nade>();

    nade.definition = selectedNade;
    nade.definition = selectedNade;
    nade.launch(to);
    player.SetCasting(false);
  }

}


