﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class References : MonoBehaviour
{

  public SO_DefaultReferences defaultReferences;

  private Dictionary<string, MonoBehaviour> references = new Dictionary<string, MonoBehaviour>();

  public string[] registered;

  public void Add(string name, MonoBehaviour classRef)
  {
    references.Add(name, classRef);
    UpdateRegisteredList();
  }


  public MonoBehaviour Get(string name)
  {
    if (references.TryGetValue(name, out MonoBehaviour reference))
    {
      return reference;
    }

    return null;
  }

  void UpdateRegisteredList()
  {
    registered = new string[references.Keys.Count];
    int key = 0;
    foreach (var name in references.Keys)
    {
      registered[key] = name;
    }

  }


}
