using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class References : MonoBehaviour
{

  public SO_DefaultReferences defaultReferences;
  public static SO_DefaultReferences defaults;

  public static Dictionary<string, MonoBehaviour> references = new Dictionary<string, MonoBehaviour>();

  void Awake()
  {
    defaults = defaultReferences;
  }

  public void Add(string name, MonoBehaviour classRef)
  {
    if (!references.ContainsKey(name))
    {
      references.Add(name, classRef);
    }
    else
    {
      Debug.Log($"Reference added twice! ({name})");
    }
  }

  public MonoBehaviour Get(string name)
  {
    return StaticGet(name);
  }

  static public MonoBehaviour StaticGet(string name)
  {
    if (references.TryGetValue(name, out MonoBehaviour reference))
    {
      return reference;
    }

    return null;
  }

}
