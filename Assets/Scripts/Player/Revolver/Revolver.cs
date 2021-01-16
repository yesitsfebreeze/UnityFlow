using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Revolver : ReferenceAwareMonoBehaviour
{

  public int ammo;
  public int superAmmo;
  public RevolverSettingSO revolverSettings;

  private bool isReloading = false;
  private bool isRecharging = false;
  private bool isCharging = false;
  private bool isShotDown = false;
  private bool isSupershotDown = false;
  private bool isShooting = false;
  private IEnumerator chargeRoutine;
  private PlayerControls playerControls;
  private Movement movement;


  override public void Awake()
  {
    base.Awake();
    ammo = revolverSettings.MaxAmmo;
    superAmmo = 0;
    movement = GetComponent<Movement>() as Movement;
  }

  void OnEnable()
  {
    playerControls = new PlayerControls();
    playerControls.Enable();

    playerControls.CharacterControls.Shoot.performed += ctx => OnShootPress(ctx);
    playerControls.CharacterControls.Shoot.canceled += ctx => OnShootRelease(ctx);

    playerControls.CharacterControls.ShootCharged.performed += ctx => OnShootChargedPress(ctx);
    playerControls.CharacterControls.ShootCharged.canceled += ctx => OnShootChargedRelease(ctx);
  }


  void OnShootPress(InputAction.CallbackContext ctx)
  {
    isShotDown = true;
  }

  void OnShootRelease(InputAction.CallbackContext ctx)
  {
    isShotDown = false;
  }

  void OnShootChargedPress(InputAction.CallbackContext ctx)
  {
    isSupershotDown = true;
    movement.Moveable(false);
  }

  void OnShootChargedRelease(InputAction.CallbackContext ctx)
  {
    StopCoroutine(chargeRoutine);
    isSupershotDown = false;
    isCharging = false;
    movement.Moveable(true);

    if (isRecharging || superAmmo == 0) return;

    StartCoroutine(ShootSuper());
  }


  IEnumerator Shoot()
  {
    isShooting = true;
    movement.Slow(revolverSettings.Slowdown);
    yield return new WaitForSeconds(revolverSettings.ShootTime);

    GameObject bullet = Instantiate(revolverSettings.BulletPrefab, transform.position, movement.rotation) as GameObject;
    Bullet Bullet = bullet.GetComponent<Bullet>() as Bullet;

    Player player = references.Get("Player") as Player;
    if (!player) yield break;

    Bullet.Fire(player.mouseWorldPosition);
    ammo--;
    yield return new WaitForSeconds(revolverSettings.ShootTime * 0.6f);
    movement.ResetSlow();
    StartCoroutine(Recharge());
    isShooting = false;
  }

  IEnumerator ShootSuper()
  {
    isShooting = true;
    yield return new WaitForSeconds(revolverSettings.ShootTime);

    GameObject superBullet = Instantiate(revolverSettings.SuperBulletPrefab, transform.position, movement.rotation) as GameObject;
    SuperBullet SuperBullet = superBullet.GetComponent<SuperBullet>() as SuperBullet;


    Player player = references.Get("Player") as Player;
    if (!player) yield break;


    SuperBullet.Fire(player.mouseWorldPosition);
    superAmmo = 0;
    isCharging = false;
    yield return new WaitForSeconds(revolverSettings.ShootTime * 0.6f);
    movement.ResetSlow();
    StartCoroutine(Recharge());
    isShooting = false;
  }

  IEnumerator Recharge()
  {
    isRecharging = true;
    yield return new WaitForSeconds(revolverSettings.RechargeTime);
    isRecharging = false;
  }

  // Update is called once per frame
  void Update()
  {

    bool shooting = isSupershotDown || isShotDown;

    int totalAmmo = ammo + superAmmo;
    if (totalAmmo == 0 && !isReloading && !isCharging && !shooting)
    {
      StartCoroutine(Reload());
    }

    if (isSupershotDown && !isCharging && !isShooting)
    {
      chargeRoutine = ChargeUp();
      StartCoroutine(chargeRoutine);

      if (ammo == 0 && superAmmo > 0)
      {
        StartCoroutine(ShootSuper());
      }
    }

    if (isSupershotDown) return;
    if (isRecharging) return;
    if (isShooting) return;
    if (!isShotDown) return;

    if (ammo == 0)
    {
      // todo: play click sound
    }
    else
    {
      StartCoroutine(Shoot());
    }

  }

  IEnumerator ChargeUp()
  {
    isCharging = true;
    yield return new WaitForSeconds(revolverSettings.ChargeTime);
    if (ammo > 0)
    {
      ammo--;
      superAmmo++;
    }
    isCharging = false;
  }

  IEnumerator Reload()
  {
    isReloading = true;
    yield return new WaitForSeconds(revolverSettings.ReloadTime);
    ammo = revolverSettings.MaxAmmo;
    isReloading = false;
  }
}
