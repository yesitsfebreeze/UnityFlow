using UnityEngine;
using System.Collections;

public class PlayerCameraShake : ReferencedMonoBehaviour
{
  public float shakeDuration = 0f;
  public float shakeAmount = 0.7f;
  public float shakeDecline = 1.0f;

  Vector3 originalPos;

  private bool started = false;

  public void Shake(float duration, float amount, float decline)
  {
    shakeDuration = duration;
    shakeAmount = amount;
    shakeDecline = decline;

    started = true;
    originalPos = transform.localPosition;
  }

  void Update()
  {
    if (!started) return;

    if (shakeDuration > 0)
    {
      transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;

      shakeDuration -= Time.deltaTime * shakeDecline;
    }
    else
    {
      shakeDuration = 0f;
      transform.localPosition = originalPos;
    }
  }
}