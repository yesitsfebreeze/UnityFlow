using UnityEngine;

[CreateAssetMenu(fileName = "LauncherSettings", menuName = "ScriptableObjects/LauncherSettings", order = 1)]
public class LauncherSettingsSO : ScriptableObject
{
  [Header("Launcher")]
  public float RechargeTime = 1f;
  public float Slowdown = 0.2f;

  [Header("Nades")]
  public float ArcHeight = 6f;
  public float TimeToLand = 0.7f;
  public float MaxRange = 20f;
  public float IgniteDelay = 0.6f;
  public float[] LaunchTimes = new float[3];
  public float[] Cooldowns = new float[3];
}