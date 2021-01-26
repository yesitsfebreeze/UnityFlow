using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using LiteNetLib.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace Flow.Actions
{

  /// <summary>
  /// Used to setup all actions and their needed counterparts
  /// </summary>
  public class FlowActions : MonoBehaviour
  {
    public static List<string> AvailableActions;
    public static bool isClient = false;
    public static int ActionCount = 0;
    public static Dictionary<string, Component> actionComponents = new Dictionary<string, Component>();
    public static Dictionary<string, FlowAction> actions = new Dictionary<string, FlowAction>();
    public static Dictionary<int, FlowAction> actionsByID = new Dictionary<int, FlowAction>();
    public static FlowSettings settings;
    public static NetDataWriter writer;
    public static NetPacketProcessor processor;
    public static FlowActions instance;
    public static string ACTION_NAME_SPACE = "Flow.Actions";
    public static UnityEvent OnStartedEvent;
    public static bool HasListeners;

    private HashSet<string> actionNames = new HashSet<string>();

    private void Awake()
    {
      if (instance == null)
      {
        OnStartedEvent = new UnityEvent();
        instance = this;
      }
      else if (instance != this)
      {
        Logger.Debug("Instance already exists, destroying object!");
        Destroy(this);
      }
    }

    void FixedUpdate()
    {
      FireStartedCallbacks();
    }

    void Start()
    {
      writer = new NetDataWriter();
      processor = new NetPacketProcessor();

      RegisterProcessorExtensions();
      CollectActions();
      RegisterActions();
      FireStartedCallbacks();
    }

    /// <summary>
    /// Fires the callback when listeners are present
    /// </summary>
    private static void FireStartedCallbacks()
    {
      if (!HasListeners) return;
      OnStartedEvent.Invoke();
      OnStartedEvent.RemoveAllListeners();
      HasListeners = false;
    }

    /// <summary>
    /// Callback that fires when all actions are registered.
    /// </summary>
    /// <param name="call"></param>
    static public void RegisterOnStartedCallback(UnityAction call)
    {
      OnStartedEvent.AddListener(call);
      HasListeners = true;
    }

    void OnDestroy()
    {
      foreach (Component actionComponent in actionComponents.Values)
      {
        Destroy(actionComponent);
      }

      Destroy(this);
    }

    /// <summary>
    /// Collects all actions in the current assembly
    /// </summary>
    private void CollectActions()
    {
      Assembly asm = Assembly.GetExecutingAssembly();
      Type[] types = asm.GetTypes();
      foreach (var type in types)
      {
        if (type.Namespace == "Flow.Actions")
        {
          bool isServerAction = type.Name.EndsWith("FlowServerAction");
          bool isClientAction = type.Name.EndsWith("FlowClientAction");
          if (isServerAction || isClientAction)
          {
            actionNames.Add(CleanName(type.Name, true));
          }

        }
      }
    }

    /// <summary>
    /// Registers all connected actions
    /// </summary>
    private void RegisterActions()
    {

      foreach (string action in actionNames)
      {
        string actionName = CleanName(action);
        string actionType = $"{ACTION_NAME_SPACE}.{actionName}";

        Type flowAction = Type.GetType(actionType);

        if (flowAction == null)
        {
          throw new Exception($"Action of type {actionType} not found!");
        }

        else
        {
          FlowAction actionComponent = gameObject.AddComponent(flowAction) as FlowAction;
          actionComponents.Add(actionName, actionComponent);
          RegisterAction(actionComponent);
        }
      }
    }

    /// <summary>
    /// Registers additional value types for the packages
    /// Types can be defined in "FlowPackageExtensions.cs" 
    ///    /// </summary>
    private void RegisterProcessorExtensions()
    {
      processor.RegisterNestedType((w, v) => w.Put(v), reader => reader.GetVector2());
      processor.RegisterNestedType((w, v) => w.Put(v), reader => reader.GetVector3());
      processor.RegisterNestedType((w, v) => w.Put(v), reader => reader.GetQuaternion());
    }

    /// <summary>
    /// Registers a single action component
    /// </summary>
    /// <param name="action"></param>
    private void RegisterAction(FlowAction action)
    {
      string actionName = FlowActions.CleanName(action.GetType().Name, true);
      if (actions.TryGetValue(actionName, out FlowAction flowAction)) return;

      ActionCount++;

      actions.Add(actionName, action);
      actionsByID.Add(ActionCount, action);
      action.id = ActionCount;
      action.settings = settings;
      action.writer = writer;
      action.processor = processor;
      action.isClient = isClient;
      action.SubscribePackage();
    }

    /// <summary>
    /// Returns a cleaned action name.
    /// if strip is true it will omit the postfix
    /// </summary>
    /// <param name="name"></param>
    /// <param name="strip"></param>
    /// <returns></returns>
    public static string CleanName(string name, bool strip = false)
    {

      string stripped = Regex.Replace(name, @"FlowClientAction$", "");
      stripped = Regex.Replace(stripped, @"FlowServerAction$", "");
      if (strip) return stripped;
      if (isClient) return stripped + "FlowClientAction";
      return stripped + "FlowServerAction";
    }

    /// <summary>
    /// Returns an action by its name
    /// </summary>
    /// <param name="actionName"></param>
    /// <returns></returns>
    static public FlowAction GetActionByName(string actionName)
    {
      actionName = FlowActions.CleanName(actionName, true);
      return actions[actionName];
    }

    /// <summary>
    /// Returns an action by its id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    static public FlowAction GetActionById(int id)
    {
      return actionsByID[id];
    }
  }



}