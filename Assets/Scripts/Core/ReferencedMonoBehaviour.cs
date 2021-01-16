using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferencedMonoBehaviour : MonoBehaviour
{


  virtual public void Awake()
  {
    References references = GameObject.Find("References").GetComponent<References>() as References;
    references.Add(this.GetType().Name, this);
  }

}