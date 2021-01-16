using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Bullet : Projectile
{

  public override void Start()
  {
    base.Start();
    this.SetProperties(
      revolverSettings.BulletSpeed,
      revolverSettings.BulletRange,
      revolverSettings.BulletFx
    );
  }


}
