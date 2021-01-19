using UnityEngine;
using System.Collections.Generic;


namespace Networking
{
  public class NetworkActions : MonoBehaviour
  {
    public SO_NetworkSettings NetworkSettings;
    public static SO_NetworkSettings settings;
    public Component[] actions;
    public static NetworkActions instance;

    void RegisterActions()
    {
      gameObject.AddComponent<ConnectAction>();
      gameObject.AddComponent<SpawnAction>();
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
      settings = NetworkSettings;
      RegisterActions();
      AddSettings();
    }

    private void AddSettings()
    {
      actions = GetComponents<NetworkAction>();

      foreach (NetworkAction action in actions)
      {
        action.SetSettings(settings);
      }
    }
  }

  class Actions
  {

    public enum actionIDs { }

    public static int ActionCount = 0;
    public static Dictionary<string, NetworkAction> actions = new Dictionary<string, NetworkAction>();
    public static Dictionary<int, NetworkAction> actionsByID = new Dictionary<int, NetworkAction>();

    public static void Register(NetworkAction action)
    {
      if (actions.TryGetValue(action.GetType().Name.Replace("Action", ""), out NetworkAction nwAction)) return;

      action.Register(ActionCount, actions);
      actionsByID.Add(ActionCount, action);

      ActionCount++;
    }

    static public NetworkAction Get(string actionName)
    {
      return actions[actionName];
    }

    static public NetworkAction GetByID(int id)
    {
      return actionsByID[id - 1];
    }
  }



  public class NetworkAction : MonoBehaviour
  {
    public int id;
    public static SO_NetworkSettings settings;

    public delegate void FromClientHandler(int clientID, Packet packet);
    public delegate void FromServerHandler(Packet packet);


    public int GetID()
    {
      return id;
    }

    public void SetSettings(SO_NetworkSettings NetworkSettings)
    {
      settings = NetworkSettings;
    }

    void OnEnable()
    {
      Actions.Register(this);
    }

    public void Register(int actionID, Dictionary<string, NetworkAction> actions)
    {
      id = actionID + 1;
      actions.Add(this.GetType().Name.Replace("Action", ""), this);
    }

    virtual public void FromClient(int clientID, Packet packet)
    {
      Debug.Log("not implemented");
    }

    virtual public void FromServer(Packet packet)
    {
      Debug.Log("not implemented");
    }

  }
}