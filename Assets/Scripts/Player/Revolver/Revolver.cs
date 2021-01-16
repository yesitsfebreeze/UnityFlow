using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Revolver : ReferenceAwareMonoBehaviour
{

  public int ammo;
  public int superAmmo;
  public SO_RevolverSettings settings;

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
    ammo = settings.MaxAmmo;
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
    movement.Slow(settings.Slowdown);
    yield return new WaitForSeconds(settings.ShootTime);

    GameObject bullet = Instantiate(settings.Bullet, transform.position, movement.rotation) as GameObject;
    Projectile projectile = bullet.GetComponent<Projectile>() as Projectile;

    Player player = references.Get("Player") as Player;
    if (!player) yield break;

    projectile.Fire(player.mouseWorldPosition);
    ammo--;
    yield return new WaitForSeconds(settings.ShootTime * 0.6f);
    movement.ResetSlow();
    StartCoroutine(Recharge());
    isShooting = false;
  }

  IEnumerator ShootSuper()
  {
    isShooting = true;
    yield return new WaitForSeconds(settings.ShootTime);

    GameObject superBullet = Instantiate(settings.SuperBullet, transform.position, movement.rotation) as GameObject;
    Projectile projectile = superBullet.GetComponent<Projectile>() as Projectile;


    Player player = references.Get("Player") as Player;
    if (!player) yield break;


    projectile.Fire(player.mouseWorldPosition);
    superAmmo = 0;
    isCharging = false;
    yield return new WaitForSeconds(settings.ShootTime * 0.6f);
    movement.ResetSlow();
    StartCoroutine(Recharge());
    isShooting = false;
  }

  IEnumerator Recharge()
  {
    isRecharging = true;
    yield return new WaitForSeconds(settings.RechargeTime);
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
    yield return new WaitForSeconds(settings.ChargeTime);
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
    yield return new WaitForSeconds(settings.ReloadTime);
    ammo = settings.MaxAmmo;
    isReloading = false;
  }
}
