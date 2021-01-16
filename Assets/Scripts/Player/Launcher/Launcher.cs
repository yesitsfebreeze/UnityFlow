using System.Reflection;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Launcher : ReferenceAwareMonoBehaviour
{

  public float[] cooldowns = new float[3];
  public float reloadCooldown;

  private PlayerControls playerControls;
  private IngameUI UI;
  public SO_LauncherSettings settings;
  public GameObject[] selectedNades;
  private IEnumerator launchRoutine;
  private bool launchRoutineStarted = false;
  private Player player;


  void OnEnable()
  {
    selectedNades = new GameObject[3];

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

    if (reloadCooldown > 0) reloadCooldown -= Time.deltaTime;
    if (reloadCooldown < 0) reloadCooldown = 0;

    int slot = 0;
    foreach (float cd in cooldowns)
    {
      if (cooldowns[slot] > 0) cooldowns[slot] -= Time.deltaTime;
      if (cooldowns[slot] < 0) cooldowns[slot] = 0;
      UI.UpdateNadeCooldown(slot, cooldowns[slot], reloadCooldown);

      slot++;
    }
  }

  public void SetSlot(int slot, int nade)
  {
    GameObject selectedNade = settings.Nades[nade];
    UI.AddNadeCooldown(selectedNade, nade, settings.Cooldowns[slot], settings.KeyBinds[slot]);
    selectedNades[slot] = selectedNade;
  }

  public Object GetNade(int slot)
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
    if (reloadCooldown > 0) return;
    if (cooldowns[slot] > 0) return;

    launchRoutine = LaunchNade(slot);
    StartCoroutine(launchRoutine);
  }

  void OnGrenadeLaunchRelease(InputAction.CallbackContext context, int slot)
  {

  }

  void CancelLaunchRoutine()
  {
    if (!launchRoutineStarted) return;

    Movement movement = GetComponent<Movement>() as Movement;
    reloadCooldown = settings.RechargeTime;
    StopCoroutine(launchRoutine);
    launchRoutineStarted = false;
    movement.ResetSlow();
  }

  IEnumerator LaunchNade(int slot)
  {
    if (player == null) player = references.Get("Player") as Player;

    launchRoutineStarted = true;
    reloadCooldown = settings.RechargeTime;
    Movement movement = GetComponent<Movement>() as Movement;

    movement.Slow(settings.Slowdown);
    player.unitUI.SetCastTime(settings.LaunchTimes[slot]);
    yield return new WaitForSeconds(settings.LaunchTimes[slot]);
    movement.ResetSlow();

    cooldowns[slot] = settings.Cooldowns[slot];
    reloadCooldown = settings.RechargeTime;

    Object selectedNade = GetNade(slot);

    Vector3 from = transform.position;
    Vector3 to = player.mouseWorldPosition;
    GameObject go = Instantiate(selectedNade, from, Quaternion.identity) as GameObject;
    go.transform.parent = gameObject.transform;
    Nade nade = go.GetComponent<Nade>();
    nade.launch(to);
  }

}


