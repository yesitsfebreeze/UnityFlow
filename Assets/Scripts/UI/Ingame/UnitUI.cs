using UnityEngine;
using UnityEngine.UI;


public class UnitUI : ReferenceAwareMonoBehaviour
{

  public Vector3 Offset = new Vector3(0, 4f, 2.5f);
  public float LerpSpeed = 7f;
  public GameObject HealthBar;
  public GameObject CastBar;
  public GameObject Bullets;

  private Canvas canvas;
  private PlayerCamera playerCamera;
  private Vector3 playerPos;
  private RectTransform rect;
  private HealthState health;
  private float castTime;
  private float currentCastTime;
  private int bulletCount;
  private int superBulletCount;

  public void SetHealth(HealthState health)
  {
    this.health = health;
  }

  public void SetCastTime(float castTime)
  {

    this.castTime = castTime;
    currentCastTime = castTime;
  }

  public void SetBulletCount(int bulletCount)
  {
    this.bulletCount = bulletCount;
  }

  public void SetSuperBulletCount(int superBulletCount)
  {
    this.superBulletCount = superBulletCount;
  }

  void Start()
  {
    rect = GetComponent<RectTransform>() as RectTransform;
    canvas = GetComponent<Canvas>() as Canvas;
    playerCamera = references.Get("PlayerCamera") as PlayerCamera;
    canvas.worldCamera = Camera.main;

    transform.localPosition = Offset;
  }

  void Update()
  {
    LookAtCamera();

    UpdateBullets();
    UpdateSuperBullets();
    UpdateHealth();
    UpdateCastTime();
  }



  void UpdateBullets()
  {
    int childCount = Bullets.transform.childCount;
    for (int i = 0; i < childCount; i++)
    {
      Transform child = Bullets.transform.GetChild(i);
      GameObject childGO = child.gameObject;
      if (i >= bulletCount)
      {
        childGO.SetActive(false);
      }
      else
      {
        childGO.SetActive(true);
      }
    }

  }

  void UpdateSuperBullets()
  {
    int childCount = Bullets.transform.childCount;
    for (int i = 0; i < childCount; i++)
    {
      Transform child = Bullets.transform.GetChild(i);
      GameObject childGO = child.gameObject;
      Image image = childGO.GetComponent<Image>() as Image;

      if (i <= superBulletCount - 1)
      {
        image.color = Color.black;
      }
      else
      {
        image.color = Color.white;
      }
    }
  }

  void UpdateHealth()
  {
    float percent = health.CurrentHealth / health.MaxHealth;
    RectTransform healthRect = HealthBar.GetComponent<RectTransform>() as RectTransform;
    Vector2 targetHealth = new Vector2(rect.sizeDelta.x * percent, healthRect.sizeDelta.y);
    healthRect.sizeDelta = Vector2.Lerp(healthRect.sizeDelta, targetHealth, LerpSpeed * Time.deltaTime);
  }

  void UpdateCastTime()
  {
    if (currentCastTime > 0) currentCastTime -= Time.deltaTime;
    if (currentCastTime < 0) currentCastTime = 0;

    RectTransform castRect = CastBar.GetComponent<RectTransform>() as RectTransform;

    if (castTime > 0)
    {
      float percent = 1 - currentCastTime / castTime;
      Vector2 targetCast = new Vector2(rect.sizeDelta.x * percent, castRect.sizeDelta.y);
      castRect.sizeDelta = targetCast;
    }

    if (currentCastTime == 0)
    {
      castTime = 0;
      castRect.sizeDelta = new Vector2(0, castRect.sizeDelta.y);
    }
  }

  void LookAtCamera()
  {
    transform.rotation = playerCamera.GetCameraAngle();
  }

}