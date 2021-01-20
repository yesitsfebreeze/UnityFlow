using System;
using System.Collections.Generic;
using UnityEngine;

namespace Networking
{

  public class Server : MonoBehaviour
  {
    public SO_NetworkSettings NetworkSettings;

    public static Dictionary<int, ServerClient> clients = new Dictionary<int, ServerClient>();
    public delegate void ClientIteration(ServerClient client);
    public static SO_NetworkSettings settings;

    public enum Protocol
    {
      TCP,
      UDP,
    }


    /// <summary>Starts the server and listends for connections.</summary>
    void Start()
    {
      settings = NetworkSettings;
      gameObject.AddComponent<ThreadManager>();
      NetworkActions actions = gameObject.AddComponent<NetworkActions>();
      actions.NetworkSettings = settings;

      // setup unity to run efficently on headless mode
      Time.fixedDeltaTime = 1f / settings.TICK_RATE;
      QualitySettings.vSyncCount = 0;
      Application.targetFrameRate = settings.TICK_RATE;

      // creates all clients
      // they are not connected yet
      for (int i = 1; i <= settings.MAX_PLAYERS; i++)
      {
        clients.Add(i, new ServerClient(i));
      }

      ServerTCP.StartListening();
      ServerUDP.StartListening();

      Debug.Log($"Server started on port {settings.PORT}.");
    }

    /// <summary>Stops listeners once the GameObject is destroyed.</summary>
    void OnDestroy()
    {
      bool stoppedTCP = ServerTCP.StopListening();
      bool stoppedUDP = ServerUDP.StopListening();

      if (stoppedTCP && stoppedUDP)
      {
        Debug.Log($"Server on port {settings.PORT} as stopped.");
      }
    }


    public static void IterateClients(ClientIteration Callback)
    {
      foreach (KeyValuePair<int, ServerClient> clientI in clients)
      {
        ServerClient client = clientI.Value;
        if (client.IsConnected()) Callback(client);
      }
    }

    public static bool IsHeadlessMode()
    {
#if UNITY_EDITOR
      return true;
#else
      return UnityEngine.SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null;
#endif
    }

    public static bool IntInArray(Array list, int value)
    {
      bool contains = false;
      foreach (int n in list) // go over every number in the list
      {
        if (n == value) // check if it matches
        {
          contains = true;
          break; // no need to check any further
        }
      }
      return contains;
    }


    #region DataSending

    #region DataSending TCP
    public static void TCPSend(Package package, int clientID)
    {
      Send(Protocol.TCP, package, clientID);
    }
    public static void TCPSendAll(Package package)
    {
      SendAll(Protocol.TCP, package);
    }
    public static void TCPSendAllExcept(Package package, int[] exceptClientIDs)
    {
      SendAllExcept(Protocol.TCP, package, exceptClientIDs);
    }
    public static void TCPSendSpecific(Package package, int[] specificClientIDs)
    {
      SendAllExcept(Protocol.TCP, package, specificClientIDs);
    }
    #endregion

    #region DataSending UDP
    public static void UDPSend(Package package, int clientID)
    {
      Send(Protocol.UDP, package, clientID);
    }
    public static void UDPSendAll(Package package)
    {
      SendAll(Protocol.UDP, package);
    }
    public static void UDPSendAllExcept(Package package, int[] exceptClientIDs)
    {
      SendAllExcept(Protocol.UDP, package, exceptClientIDs);
    }
    public static void UDPSendSpecific(Package package, int[] specificClientIDs)
    {
      SendAllExcept(Protocol.UDP, package, specificClientIDs);
    }
    #endregion

    public static void Send(Protocol protocol, Package package, int clientID)
    {
      package.WriteLength();
      ServerClient client = Server.clients[clientID];
      SendData(protocol, package, client);
    }

    public static void SendAll(Protocol protocol, Package package)
    {
      IterateClients((ServerClient client) =>
      {
        SendData(protocol, package, client);
      });
    }

    public static void SendAllExcept(Protocol protocol, Package package, int[] exceptClientIDs)
    {
      // except ids
      IterateClients((ServerClient client) =>
      {
        if (!IntInArray(exceptClientIDs, client.id))
        {
          SendData(protocol, package, client);
        }
      });
    }

    public static void SendSpecific(Protocol protocol, Package package, int[] specificClientIDs)
    {
      // specific ids
      IterateClients((ServerClient client) =>
      {
        if (IntInArray(specificClientIDs, client.id))
        {
          SendData(protocol, package, client);
        }
      });
    }

    private static void SendData(Protocol protocol, Package package, ServerClient client)
    {
      if (protocol == Protocol.TCP) client.tcp.SendData(package);
      if (protocol == Protocol.UDP) client.udp.SendData(package);
    }


    #endregion
  }
}

