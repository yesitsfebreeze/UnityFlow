using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cooldown : MonoBehaviour
{


  public float maxValue = 1f;
  public float currentValue = 0f;
  public float reloadTime = 0f;
  public float slot;
  public GameObject overlayGO;
  public GameObject textGO;

  private RectTransform overlay;
  private Text text;
  private Vector2 overlaySize;

  void Start()
  {
    text = textGO.gameObject.GetComponent<Text>() as Text;
    overlay = overlayGO.gameObject.GetComponent<RectTransform>() as RectTransform;
    overlaySize = overlay.sizeDelta;
    overlay.sizeDelta = new Vector2(overlaySize.x, 0);
  }

  void Update()
  {
    float percent = currentValue / maxValue;
    float value = maxValue * percent;

    text.enabled = value != 0;
    if (!text.enabled) return;
    text.text = "" + (Mathf.Round(value * 10) / 10);
    overlay.sizeDelta = new Vector2(overlaySize.x, overlaySize.y * percent);
  }
}
