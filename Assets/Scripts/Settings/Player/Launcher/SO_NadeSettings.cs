using UnityEngine;

[CreateAssetMenu(fileName = "NadeSettings", menuName = "ScriptableObjects/NadeSettings", order = 1)]
public class SO_NadeSettings : ScriptableObject
{
  public float ArcHeight = 6f;
  public float TimeToLand = 0.7f;
  public float MaxRange = 20f;
  public float IgniteDelay = 0.6f;

}