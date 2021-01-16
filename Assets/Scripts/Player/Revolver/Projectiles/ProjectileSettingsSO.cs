using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileSettings", menuName = "ScriptableObjects/ProjectileSettings", order = 1)]
public class ProjectileSettingsSO : ScriptableObject
{
  [Header("Projectile")]
  public float GroundDistance = 2f;
  public float RayLength = 2f;
  public float MaxHeight = 2f;
}
