using System;
using System.Collections;
using UnityEngine;


public class Nade : ReferenceAwareMonoBehaviour
{
  public SO_NadeSettings settings;
  public SO_NadeDefinition definition;

  private Vector3 target;
  private bool launchable = false;
  private Vector3 position;
  private float startTime;
  private bool hasHit = false;
  private Rigidbody rb;
  private bool hasLanded = false;


  void Start()
  {
    rb = GetComponent<Rigidbody>() as Rigidbody;
  }

  void Update()
  {

    if (!launchable)
    {
      rb.velocity = Vector3.down * 20f;
    }

    if (hasHit)
    {
      OnHit();
      return;
    }

    if (!launchable) return;

    rb.isKinematic = true;
    float percent = Mathf.Clamp(Math.Abs((startTime - Time.time) / settings.TimeToLand), 0, 1);
    transform.position = Parabola(position, target, settings.ArcHeight, percent);

    if (percent >= 1) OnHit();
  }


  void OnHit()
  {
    rb.velocity = Physics.gravity * definition.gravityMultiplier;
    hasHit = true;
    rb.isKinematic = false;
    if (!hasLanded)
    {
      StartCoroutine(Landed());
      hasLanded = true;
    }
  }

  public void launch(Vector3 to)
  {
    LayerMask playersMask = LayerMask.GetMask("Players");
    startTime = Time.time;
    position = transform.position;
    target = new Vector3(to.x, to.y * 0.01f, to.z);
    float distance = Vector3.Distance(position, target);
    if (distance > settings.MaxRange)
    {
      target = position + Vector3.ClampMagnitude(target - position, settings.MaxRange);
    }

    launchable = true;
  }

  void OnTriggerEnter(Collider collider)
  {
    if (collider.gameObject.tag == "Player") return;


    hasHit = true;
  }

  Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
  {
    Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

    var mid = Vector3.Lerp(start, end, t);

    return new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
  }


  IEnumerator Landed()
  {
    yield return new WaitForSeconds(settings.IgniteDelay);
    Vector3 pos = transform.position;

    if (definition.ExplodeFx) Instantiate(definition.ExplodeFx, new Vector3(pos.x, pos.y + 0.5f, pos.z), Quaternion.identity);


    CameraShake();
    Destroy(this);
    Destroy(gameObject);
  }

  void CameraShake()
  {
    CameraShake shake = references.Get("CameraShake") as CameraShake;
    if (settings.ShakePreset != null)
    {
      shake.Shake(settings.ShakePreset);
    }
    else
    {
      shake.Shake(settings.ShakeDuration, settings.ShakeAmount, settings.ShakeDecay);
    }
  }

}
