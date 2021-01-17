using UnityEngine;
using System.Reflection;
using System.Collections;

public class CameraShake : ReferencedMonoBehaviour
{
  private float shakeDuration;
  private float shakeAmount;
  private float shakeDecay;

  Vector3 originalPos;

  private bool started = false;

  public void Shake(float duration, float amount, float decay)
  {
    shakeDuration = duration;
    shakeAmount = amount;
    shakeDecay = decay;

    started = true;
    originalPos = transform.localPosition;
  }

  public void Shake(SO_CameraShakePreset preset)
  {
    Shake(preset.duration, preset.amount, preset.decay);
  }


  void Update()
  {
    if (!started) return;

    if (shakeDuration > 0)
    {
      Vector3 target = originalPos + Random.insideUnitSphere * shakeAmount;
      transform.localPosition = Vector3.Lerp(transform.localPosition, target, 20f * Time.deltaTime);

      shakeDuration -= Time.deltaTime * shakeDecay;
    }
    else
    {
      shakeDuration = 0f;
      transform.localPosition = originalPos;
    }
  }

  // presets

  public void PresetShortSubtle()
  {
    Shake(0.2f, 0.3f, 1f);
  }

  public void PresetShortIntense()
  {
    Shake(0.2f, 0.75f, 2f);
  }

  public void PresetMediumSubtle()
  {
    Shake(0.6f, 0.3f, 1f);
  }

  public void PresetMediumIntense()
  {
    Shake(0.6f, 0.75f, 2f);
  }

  public void PresetLongSubtle()
  {
    Shake(1.2f, 0.5f, 2f);
  }

  public void PresetLongIntense()
  {
    Shake(1.2f, 0.5f, 2f);
  }
}