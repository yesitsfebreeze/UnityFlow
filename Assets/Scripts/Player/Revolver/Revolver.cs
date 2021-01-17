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
  private Player player;


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

  void Start()
  {
    if (player == null) player = references.Get("Player") as Player;
    player.unitUI.SetBulletCount(ammo);
  }


  void OnShootPress(InputAction.CallbackContext ctx)
  {
    player.SetCasting(true);

    isShotDown = true;
  }

  void OnShootRelease(InputAction.CallbackContext ctx)
  {
    isShotDown = false;
  }

  void OnShootChargedPress(InputAction.CallbackContext ctx)
  {
    player.SetCasting(true);
    isSupershotDown = true;
  }

  void OnShootChargedRelease(InputAction.CallbackContext ctx)
  {
    if (player == null) player = references.Get("Player") as Player;

    StopCoroutine(chargeRoutine);
    isSupershotDown = false;
    isCharging = false;
    movement.Moveable(true);

    if (isRecharging || superAmmo == 0) return;

    StartCoroutine(ShootSuper());
  }


  IEnumerator Shoot()
  {
    if (player == null) player = references.Get("Player") as Player;

    isShooting = true;
    movement.Slow(settings.Slowdown);

    player.unitUI.SetCastTime(settings.ShootTime);
    yield return new WaitForSeconds(settings.ShootTime);

    GameObject bullet = Instantiate(settings.Bullet, transform.position, movement.rotation) as GameObject;
    Projectile projectile = bullet.GetComponent<Projectile>() as Projectile;

    projectile.Fire(player.mouseWorldPosition);
    player.SetCasting(true);
    ammo--;
    player.unitUI.SetBulletCount(ammo);
    yield return new WaitForSeconds(settings.ShootTime * 0.6f);
    movement.ResetSlow();
    StartCoroutine(Recharge());
    isShooting = false;
  }

  IEnumerator ShootSuper()
  {
    if (player == null) player = references.Get("Player") as Player;

    isShooting = true;

    player.unitUI.SetCastTime(settings.ShootTime);
    yield return new WaitForSeconds(settings.ShootTime);

    GameObject superBullet = Instantiate(settings.SuperBullet, transform.position, movement.rotation) as GameObject;
    Projectile projectile = superBullet.GetComponent<Projectile>() as Projectile;

    projectile.Fire(player.mouseWorldPosition);
    player.SetCasting(true);
    superAmmo = 0;
    player.unitUI.SetSuperBulletCount(superAmmo);
    player.unitUI.SetBulletCount(ammo);
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
    player.SetCasting(false);
    isRecharging = false;
  }

  // Update is called once per frame
  void Update()
  {

    if (isSupershotDown)
    {
      if (ammo > 0)
      {
        movement.Moveable(false);
      }
      else
      {
        movement.Moveable(true);
        isCharging = false;
        isSupershotDown = false;
        isShotDown = false;
      }
    }

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
    if (player == null) player = references.Get("Player") as Player;
    yield return new WaitForSeconds(settings.ChargeTime);
    if (ammo > 0)
    {
      ammo--;
      superAmmo++;
      player.unitUI.SetSuperBulletCount(superAmmo);
      isCharging = false;
    }
  }

  IEnumerator Reload()
  {
    if (player == null) player = references.Get("Player") as Player;
    isReloading = true;
    yield return new WaitForSeconds(settings.ReloadTime);
    ammo = settings.MaxAmmo;
    player.unitUI.SetBulletCount(ammo);
    isReloading = false;
  }
}
