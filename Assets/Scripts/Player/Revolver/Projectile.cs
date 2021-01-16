using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Projectile : ReferenceAwareMonoBehaviour
{

  public SO_ProjectileSettings settings;

  private Vector3 startPosition;
  private Vector3 targetPosition;
  private bool fired = false;
  private Vector3 defaultDirection;
  private SphereCollider projectileCollider;
  private float initialGroundDistance;

  public void Fire(Vector3 to)
  {
    startPosition = transform.position;
    targetPosition = new Vector3(to.x, 0.5f, to.z);
    fired = true;
    defaultDirection = GetDefaultDirection();
    projectileCollider = GetComponent<SphereCollider>() as SphereCollider;
    initialGroundDistance = GetGroundDistance();
    transform.LookAt(new Vector3(targetPosition.x, transform.position.y, targetPosition.z), defaultDirection);

  }


  void Update()
  {
    if (!fired) return;
    LayerMask playersMask = LayerMask.GetMask("Players");
    Vector3 nextPos = transform.position + defaultDirection * settings.Speed * Time.deltaTime;
    transform.position = nextPos;
    float groundDistance = GetGroundDistance();
    float groundOffset = groundDistance - initialGroundDistance;
    transform.position = new Vector3(transform.position.x, transform.position.y - groundOffset, transform.position.z);
    float distance = Vector3.Distance(startPosition, transform.position);
    if (distance >= settings.Range) Hit();
  }


  Vector3 GetDefaultDirection()
  {
    Vector3 direction = (targetPosition - startPosition).normalized;
    return new Vector3(direction.x, 0, direction.z);
  }


  float GetGroundDistance()
  {
    if (Physics.SphereCast(transform.position, projectileCollider.radius * settings.ColliderSizeMultiplier, Vector3.down, out RaycastHit hit, 10f))
    {
      return hit.distance;
    }

    return 1f;
  }


  void OnTriggerEnter(Collider col)
  {
    if (col.gameObject.tag != "Player") Hit();
    if (col.gameObject.tag != "Enemy") return;
    Destroy(col.gameObject);
  }

  void Hit()
  {
    if (settings.Fx) Instantiate(settings.Fx, new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), Quaternion.identity);
    CameraShake();
    Destroy(this);
    Destroy(gameObject);
  }

  void CameraShake()
  {
    PlayerCameraShake shake = references.Get("PlayerCameraShake") as PlayerCameraShake;
    if (shake) shake.Shake(0.1f, 0.3f, 2f);
  }
}
