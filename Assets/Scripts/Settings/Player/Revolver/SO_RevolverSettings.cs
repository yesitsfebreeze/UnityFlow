using UnityEngine;

[CreateAssetMenu(fileName = "RevolverSettings", menuName = "ScriptableObjects/RevolverSettings", order = 1)]
public class SO_RevolverSettings : ScriptableObject
{
  [Header("Revolver")]
  public int MaxAmmo = 3;
  public float RechargeTime = 0.7f;
  public float ShootTime = 0.2f;
  public float ChargeTime = 1.5f;
  public float ReloadTime = 1f;
  public float Slowdown = 0.2f;


  public GameObject Bullet;
  public GameObject SuperBullet;
}