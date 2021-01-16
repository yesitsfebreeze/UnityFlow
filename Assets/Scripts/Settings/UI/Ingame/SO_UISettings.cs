using UnityEngine;

[CreateAssetMenu(fileName = "UISettings", menuName = "ScriptableObjects/UISettings", order = 1)]
public class SO_UISettings : ScriptableObject
{
  [Header("Launcher")]
  public GameObject Cooldown;
}