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


    #region DataSending

    #region DataSending TCP
    public static void SendTCP(Package package, int clientID)
    {
      Send(Protocol.TCP, package, clientID);
    }
    public static void SendTCPAll(Package package)
    {
      SendAll(Protocol.TCP, package);
    }
    public static void SendTCPAllExcept(Package package, int[] exceptClientIDs)
    {
      SendAllExcept(Protocol.TCP, package, exceptClientIDs);
    }
    #endregion

    #region DataSending UDP
    public static void SendUDP(Package package, int clientID)
    {
      Send(Protocol.UDP, package, clientID);
    }
    public static void SendUDPAll(Package package)
    {
      SendAll(Protocol.UDP, package);
    }
    public static void SendUDPAllExcept(Package package, int[] exceptClientIDs)
    {
      SendAllExcept(Protocol.UDP, package, exceptClientIDs);
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
      print("test");
      IterateClients((ServerClient client) =>
      {
        print(client.id);
        SendData(protocol, package, client);
      });
    }

    public static void SendAllExcept(Protocol protocol, Package package, int[] exceptClientIDs)
    {
      // except ids
      IterateClients((ServerClient client) =>
      {
        SendData(protocol, package, client);
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

