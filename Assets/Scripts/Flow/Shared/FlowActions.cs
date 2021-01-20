using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FlowActions;

namespace Flow
{
  public class FlowActions : MonoBehaviour
  {
    public FlowSettings FlowSettings;
    public static FlowSettings settings;
    public Component[] actions;
    public static FlowActions instance;

    void RegisterActions()
    {
      foreach (string action in settings.actions)
      {
        string actionName = Regex.Replace(action, @"Flow$", "") + "Flow";
        string actionType = "FlowActions." + actionName;
        Type networkAction = Type.GetType(actionType);
        if (networkAction == null)
        {
          Debug.LogError($"Action of type {actionType} not found! Check the Namespace and the 'NA_' prefix");
        }
        else
        {
          gameObject.AddComponent(networkAction);
        }

      }
    }

    private void Awake()
    {
      if (instance == null)
      {
        instance = this;
      }
      else if (instance != this)
      {
        Debug.Log("Instance already exists, destroying object!");
        Destroy(this);
      }
    }

    void Start()
    {
      settings = FlowSettings;
      RegisterActions();
      AddSettings();
    }

    private void AddSettings()
    {
      actions = GetComponents<FlowAction>();

      foreach (FlowAction action in actions)
      {
        action.SetSettings(settings);
      }
    }
  }

  class Actions
  {

    public enum actionIDs { }

    public static int ActionCount = 0;
    public static Dictionary<string, FlowAction> actions = new Dictionary<string, FlowAction>();
    public static Dictionary<int, FlowAction> actionsByID = new Dictionary<int, FlowAction>();

    public static void Register(FlowAction action)
    {
      string actionName = Regex.Replace(action.GetType().Name, @"Flow$", "") + "Flow";
      if (actions.TryGetValue(actionName, out FlowAction flowAction)) return;

      action.Register(ActionCount, actions);
      actionsByID.Add(ActionCount, action);

      ActionCount++;
    }

    static public FlowAction Get(string actionName)
    {
      return actions[actionName];
    }

    static public FlowAction GetByID(int id)
    {
      return actionsByID[id - 1];
    }
  }



  public class FlowAction : MonoBehaviour
  {
    public int id;
    public static FlowSettings settings;

    public delegate void FromClientHandler(int clientID, FlowPackage package);
    public delegate void FromServerHandler(FlowPackage package);


    public int GetID()
    {
      return id;
    }

    public void SetSettings(FlowSettings NetworkSettings)
    {
      settings = NetworkSettings;
    }

    void OnEnable()
    {
      Actions.Register(this);
    }

    public void Register(int actionID, Dictionary<string, FlowAction> actions)
    {
      id = actionID + 1;
      string actionName = Regex.Replace(this.GetType().Name, @"Flow$", "") + "Flow";
      actions.Add(actionName, this);
    }

    virtual public void FromClient(int clientID, FlowPackage package)
    {
      Debug.Log("not implemented");
    }

    virtual public void FromServer(FlowPackage package)
    {
      Debug.Log("not implemented");
    }

  }
}