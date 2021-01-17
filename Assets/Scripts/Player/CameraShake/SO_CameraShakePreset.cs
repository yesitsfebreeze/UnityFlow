using UnityEngine;

[CreateAssetMenu(fileName = "CameraShakePreset", menuName = "ScriptableObjects/CameraShakePreset", order = 1)]
public class SO_CameraShakePreset : ScriptableObject
{
  public float duration;
  public float amount;
  public float decay;
}