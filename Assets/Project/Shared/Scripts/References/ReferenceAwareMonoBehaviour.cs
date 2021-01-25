using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferenceAwareMonoBehaviour : MonoBehaviour
{

  protected References references;

  virtual public void Awake()
  {
    references = GameObject.Find("References").GetComponent<References>() as References;
  }

}