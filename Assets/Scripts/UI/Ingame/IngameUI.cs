using UnityEngine;
using UnityEngine.UI;


public class IngameUI : ReferencedMonoBehaviour
{

  public SO_UISettings settings;
  public Transform cooldownWrapper;

  public void AddNadeCooldown(SO_NadeDefinition nadeDefinition, int slot, string key)
  {
    GameObject UIELement = Instantiate(settings.NadeCooldown, Vector3.zero, Quaternion.identity) as GameObject;
    UIELement.transform.SetParent(cooldownWrapper);
    NadeCooldown UIScript = UIELement.GetComponent<NadeCooldown>() as NadeCooldown;
    UIScript.nadeDefinition = nadeDefinition;
    UIScript.key = key;
    UIScript.slot = slot;
  }

  public void AddDashCooldown(SO_DashDefinition dashDefinition)
  {
    GameObject UIELement = Instantiate(settings.DashCooldown, Vector3.zero, Quaternion.identity) as GameObject;
    UIELement.transform.SetParent(cooldownWrapper);
    DashCooldown UIScript = UIELement.GetComponent<DashCooldown>() as DashCooldown;
    UIScript.dashDefinition = dashDefinition;
  }

  public void UpdateNadeCooldown(int slot, float value, float maxValue)
  {
    int count = cooldownWrapper.childCount;
    for (var i = 0; i < count; i++)
    {
      Transform UIELement = cooldownWrapper.GetChild(i);
      NadeCooldown UIScript = UIELement.GetComponent<NadeCooldown>() as NadeCooldown;
      if (UIScript)
      {
        if (UIScript.slot == slot)
        {
          UIScript.currentValue = value;
          UIScript.maxValue = maxValue;
        };
      }
    }
  }


}