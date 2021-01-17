using UnityEngine;

[CreateAssetMenu(fileName = "LauncherSettings", menuName = "ScriptableObjects/LauncherSettings", order = 1)]
public class SO_LauncherSettings : ScriptableObject
{
  [Header("Launcher")]
  public float RechargeTime = 1f;
  public float Slowdown = 0.2f;
  public GameObject NadePrefab;

  public string[] KeyBinds = new string[3];
  public float[] LaunchTimes = new float[3];
  public float[] Cooldowns = new float[3];
  public SO_NadeDefinition[] Nades = new SO_NadeDefinition[9];
}