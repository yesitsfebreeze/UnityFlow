using UnityEngine;
using UnityEngine.UI;


class IngameUI : ReferencedMonoBehaviour
{

  public SO_UISettings settings;
  public Transform cooldownWrapper;

  public void AddNadeCooldown(GameObject Nade, int slot, float cooldown)
  {
    GameObject UIELement = Instantiate(settings.Cooldown, Vector3.zero, Quaternion.identity) as GameObject;
    UIELement.transform.SetParent(cooldownWrapper);
    Cooldown UIScript = UIELement.GetComponent<Cooldown>() as Cooldown;
    UIScript.maxValue = cooldown;
    UIScript.slot = slot;
  }

  public void UpdateNadeCooldown(int slot, float cooldown, float reloadTime)
  {
    int count = cooldownWrapper.childCount;
    for (var i = 0; i < count; i++)
    {
      Transform UIELement = cooldownWrapper.GetChild(i);
      Cooldown UIScript = UIELement.GetComponent<Cooldown>() as Cooldown;

      if (UIScript.slot == slot)
      {
        UIScript.currentValue = cooldown;
        UIScript.reloadTime = reloadTime;
      };

    }
  }

}