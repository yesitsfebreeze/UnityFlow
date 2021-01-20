using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Flow
{
  public class FlowActionsRegistry : MonoBehaviour
  {
    public FlowSettings FlowSettings;
    public static FlowSettings settings;
    public bool isClient = false;
    public Component[] actions;
    public static FlowActionsRegistry instance;

    public static string NAME_SPACE = "Flow";

    void RegisterActions()
    {
      string subNamespace = "ServerSide";
      if (isClient) subNamespace = "ClientSide";

      foreach (string action in settings.actions)
      {
        string actionType = $"{NAME_SPACE}.{subNamespace}." + action;
        Type networkAction = Type.GetType(actionType);

        if (networkAction == null)
        {
          Debug.LogError($"Action of type {actionType} not found! Namespace must be '{NAME_SPACE}.{subNamespace}'.");
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

  class FlowActions
  {

    public enum actionIDs { }

    public static int ActionCount = 0;
    public static Dictionary<string, FlowAction> actions = new Dictionary<string, FlowAction>();
    public static Dictionary<int, FlowAction> actionsByID = new Dictionary<int, FlowAction>();

    public static void Register(FlowAction action)
    {
      string actionName = action.GetType().Name;
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
      FlowActions.Register(this);
    }

    public void Register(int actionID, Dictionary<string, FlowAction> actions)
    {
      id = actionID + 1;
      string actionName = this.GetType().Name;
      actions.Add(actionName, this);
    }

    // virtual public void FromClient(int clientID, FlowPackage package)
    // {
    //   Debug.Log("not implemented");
    // }

    // virtual public void FromServer(FlowPackage package)
    // {
    //   Debug.Log("not implemented");
    // }

    virtual public void In(FlowPackage package, int clientID) { }
    virtual public void In(FlowPackage package) { }

  }
}