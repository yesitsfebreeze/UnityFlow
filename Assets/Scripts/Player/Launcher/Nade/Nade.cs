using System;
using System.Collections;
using UnityEngine;


public class Nade : ReferenceAwareMonoBehaviour
{
  public LauncherSettingsSO launcherSettings;
  public UnityEngine.Object HitFx;

  private Vector3 target;
  private bool launchable = false;
  private Vector3 position;
  private float startTime;


  void Update()
  {
    if (!launchable) return;


    float percent = Math.Abs((startTime - Time.time) / launcherSettings.TimeToLand);

    transform.position = Parabola(position, target, launcherSettings.ArcHeight, percent);

    if (percent >= 1)
    {
      transform.position = target;
      StartCoroutine(Landed());
    }
  }

  public void launch(Vector3 to)
  {

    LayerMask playersMask = LayerMask.GetMask("Players");
    startTime = Time.time;
    position = transform.position;
    target = to;
    float distance = Vector3.Distance(position, target);
    if (distance > launcherSettings.MaxRange)
    {
      target = position + Vector3.ClampMagnitude(target - position, launcherSettings.MaxRange);
    }

    target.y = 100000f;

    RaycastHit[] hits = Physics.RaycastAll(target, Vector3.down, 100000.0F, ~playersMask);
    if (hits.Length > 0)
    {
      RaycastHit firstHit = hits[0];
      target.y = firstHit.point.y;
    }

    launchable = true;
  }

  Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
  {
    Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

    var mid = Vector3.Lerp(start, end, t);

    return new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
  }


  IEnumerator Landed()
  {
    yield return new WaitForSeconds(launcherSettings.IgniteDelay);

    GameObject hitFx = Instantiate(HitFx, new Vector3(target.x, target.y + 0.5f, target.z), Quaternion.identity) as GameObject;
    CameraShake();

    Destroy(this);
    Destroy(gameObject);
  }

  void CameraShake()
  {
    PlayerCameraShake shake = references.Get("PlayerCameraShake") as PlayerCameraShake;
    if (shake) shake.Shake(0.15f, 0.3f, 1.5f);

  }

}
