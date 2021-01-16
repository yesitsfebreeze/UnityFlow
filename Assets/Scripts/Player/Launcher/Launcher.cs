using System.Reflection;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Launcher : MonoBehaviour
{

  public float[] cooldowns = new float[3];
  public float reloadCooldown;

  private PlayerControls playerControls;
  public NadeTypesSO nadeTypes;
  public LauncherSettingsSO launcherSettings;
  private Object[] allNades;
  public GameObject[] selectedNades;
  private IEnumerator launchRoutine;
  private bool launchRoutineStarted = false;


  void Awake()
  {
    int nadeKey = 0;
    FieldInfo[] nadeFields = nadeTypes.GetType().GetFields();
    allNades = new Object[nadeFields.Length];
    foreach (var nade in nadeFields)
    {
      allNades[nadeKey] = nade.GetValue(nadeTypes) as Object;
      nadeKey++;
    }
  }

  void OnEnable()
  {
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

  }

  // Update is called once per frame
  void Update()
  {
    int slot = 0;
    foreach (float cd in cooldowns)
    {
      if (cooldowns[slot] > 0) cooldowns[slot] -= Time.deltaTime;
      if (cooldowns[slot] < 0) cooldowns[slot] = 0;
      slot++;
    }

    if (reloadCooldown > 0) reloadCooldown -= Time.deltaTime;
    if (reloadCooldown < 0) reloadCooldown = 0;

  }

  public void setSlot(int slot, int nade)
  {
    // selectedNades[slot] = nade;
  }

  public Object getNade(int slot)
  {
    return allNades[slot];
    // turn that on when nade slection is functional
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
    reloadCooldown = launcherSettings.RechargeTime;
    StopCoroutine(launchRoutine);
    launchRoutineStarted = false;
    movement.ResetSlow();
  }

  IEnumerator LaunchNade(int slot)
  {
    launchRoutineStarted = true;
    reloadCooldown = launcherSettings.RechargeTime;
    Movement movement = GetComponent<Movement>() as Movement;

    movement.Slow(launcherSettings.Slowdown);
    yield return new WaitForSeconds(launcherSettings.LaunchTimes[slot]);
    movement.ResetSlow();

    cooldowns[slot] = launcherSettings.Cooldowns[slot];
    reloadCooldown = launcherSettings.RechargeTime;

    Player player = GetComponent<Player>() as Player;
    Object selectedNade = getNade(slot);

    Vector3 from = transform.position;
    Vector3 to = player.mouseWorldPosition;
    GameObject go = Instantiate(selectedNade, from, Quaternion.identity) as GameObject;
    go.transform.parent = gameObject.transform;
    Nade nade = go.GetComponent<Nade>();
    nade.launch(to);
  }

}


