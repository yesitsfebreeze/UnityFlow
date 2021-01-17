using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NadeDefinition", menuName = "ScriptableObjects/NadeDefinition", order = 1)]
public class SO_NadeDefinition : ScriptableObject
{

  public GameObject ExplodeFx;

  public float gravityMultiplier = 3f;

  public Sprite icon;
}