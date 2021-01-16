using UnityEngine;

[CreateAssetMenu(fileName = "RevolverSettings", menuName = "ScriptableObjects/RevolverSettings", order = 1)]
public class RevolverSettingSO : ScriptableObject
{
  [Header("Revolver")]
  public int MaxAmmo = 3;
  public float RechargeTime = 0.7f;
  public float ShootTime = 0.2f;
  public float ChargeTime = 1.5f;
  public float ReloadTime = 1f;
  public float Slowdown = 0.2f;

  [Header("Bullet")]
  public GameObject BulletPrefab;
  public float BulletSpeed = 20f;
  public float BulletRange = 20f;
  public GameObject BulletFx;

  [Header("Super Bullet")]
  public GameObject SuperBulletPrefab;
  public float SuperBulletSpeed = 30f;
  public float SuperBulletRange = 20f;
  public GameObject SuperBulletFx;

}