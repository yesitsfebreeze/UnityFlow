using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Projectile : ReferenceAwareMonoBehaviour
{

  public RevolverSettingSO revolverSettings;
  public ProjectileSettingsSO projectileSettings;

  private Vector3 startPosition;
  private Object fx;
  private float speed;
  private float range;
  private Vector3 targetPosition;
  private bool fired = false;
  private bool changedHeight = false;
  private float targetY;

  public virtual void Start()
  {
    startPosition = transform.position;
  }

  protected void SetProperties(float speed, float range, Object fx)
  {
    this.speed = speed;
    this.range = range;
    this.fx = fx;
  }


  void Update()
  {
    if (!fired) return;
    LayerMask playersMask = LayerMask.GetMask("Players");
    Vector3 direction = (targetPosition - startPosition).normalized;
    direction = new Vector3(direction.x, 0, direction.z);
    transform.LookAt(targetPosition, direction);

    // check if colliding object has property to rotate
    // if so rotate the direction to the diretion of the object

    // Vector3 checkPosition = new Vector3(transform.position.x, transform.position.y - (projectileSettings.GroundDistance * 0.95f), transform.position.z);
    // if (Physics.Raycast(checkPosition, direction, out RaycastHit directionHit, projectileSettings.RayLength, ~playersMask))
    // {

    //   targetY = directionHit.collider.transform.position.y + projectileSettings.GroundDistance + directionHit.collider.bounds.max.y / 2;
    //   float heightDistance = targetY - checkPosition.y;

    //   if (heightDistance <= projectileSettings.MaxHeight)
    //   {
    //     changedHeight = true;
    //   }
    //   else
    //   {
    //     StartCoroutine(ResetHeight());
    //   }
    // }
    // else
    // {
    //   StartCoroutine(ResetHeight());
    // }


    // if (!changedHeight)
    // {
    //   if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit downHit, projectileSettings.MaxHeight, ~playersMask))
    //   {
    //     print("downhit");
    //     targetY = downHit.collider.transform.position.y + projectileSettings.GroundDistance + downHit.collider.bounds.max.y / 2;
    //   }
    //   else
    //   {
    //     targetY = transform.position.y;
    //   }
    // }



    // transform.position = new Vector3(transform.position.x, targetY, transform.position.z);
    // // Vector3.Lerp(transform.position, new Vector3(transform.position.x, targetY, transform.position.z), this.speed * Time.deltaTime);



    direction = (targetPosition - startPosition).normalized;
    direction = new Vector3(direction.x, 0, direction.z);
    Vector3 nextPos = transform.position + direction * this.speed * Time.deltaTime;
    transform.LookAt(nextPos, Vector3.forward);
    transform.position = nextPos;

    float distance = Vector3.Distance(startPosition, transform.position);
    if (distance >= this.range)
    {
      Hit(null);
    }
  }

  IEnumerator ResetHeight()
  {
    yield return new WaitForSeconds(this.speed * Time.deltaTime);
    targetY = transform.position.y;
    changedHeight = false;
  }

  public void Fire(Vector3 to)
  {
    targetPosition = new Vector3(to.x, 0.5f, to.z);
    fired = true;
  }

  void Hit(GameObject target)
  {
    Instantiate(this.fx, new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), Quaternion.identity);
    CameraShake();

    if (target)
    {
      // todo: implement logic
      // Destroy(target);
    }
    Destroy(this);
    Destroy(gameObject);
  }

  void OnTriggerEnter(Collider col)
  {
    // Hit(col.gameObject);

  }



  void CameraShake()
  {
    PlayerCameraShake shake = references.Get("PlayerCameraShake") as PlayerCameraShake;
    if (shake) shake.Shake(0.1f, 0.3f, 2f);
  }
}
