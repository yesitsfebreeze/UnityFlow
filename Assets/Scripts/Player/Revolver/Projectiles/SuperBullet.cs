using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SuperBullet : Projectile
{

  public override void Start()
  {
    base.Start();
    this.SetProperties(
      revolverSettings.SuperBulletSpeed,
      revolverSettings.SuperBulletRange,
      revolverSettings.SuperBulletFx
    );
  }


}
