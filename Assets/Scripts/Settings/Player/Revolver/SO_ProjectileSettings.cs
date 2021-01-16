using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileSettings", menuName = "ScriptableObjects/ProjectileSettings", order = 1)]
public class SO_ProjectileSettings : ScriptableObject
{
  [Header("Projectile")]
  public float ColliderSizeMultiplier = 1.5f;
  public float Speed = 20f;
  public float Range = 20f;
  public GameObject Fx;
}
