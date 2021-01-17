using UnityEngine;

[CreateAssetMenu(fileName = "DashDefinition", menuName = "ScriptableObjects/DashDefinition", order = 1)]
public class SO_DashDefinition : ScriptableObject
{
  public float Speed = 20f;
  public float Range = 20f;
  public string keyBind = "Space";
}
