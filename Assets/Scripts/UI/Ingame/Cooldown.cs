using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cooldown : MonoBehaviour
{

  public float maxValue = 1f;
  public float currentValue = 0f;
  public float reloadTime = 0f;
  public string key = "None";
  public float slot;
  public GameObject overlayGO;
  public GameObject cooldownTextGO;
  public GameObject keybindTextGO;

  private RectTransform overlay;
  private Text cooldownText;
  private Text keybindText;
  private Vector2 overlaySize;

  void Start()
  {
    cooldownText = cooldownTextGO.gameObject.GetComponent<Text>() as Text;
    keybindText = keybindTextGO.gameObject.GetComponent<Text>() as Text;
    overlay = overlayGO.gameObject.GetComponent<RectTransform>() as RectTransform;
    overlaySize = overlay.sizeDelta;
    overlay.sizeDelta = new Vector2(overlaySize.x, 0);
  }

  void Update()
  {
    float percent = currentValue / maxValue;
    float value = maxValue * percent;

    keybindText.text = key;

    cooldownText.enabled = value != 0;
    if (!cooldownText.enabled) return;
    cooldownText.text = "" + (Mathf.Round(value * 10) / 10);
    overlay.sizeDelta = new Vector2(overlaySize.x, overlaySize.y * percent);
  }
}
