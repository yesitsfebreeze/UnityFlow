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
    public static void SendTCP(Packet packet, int clientID)
    {
      Send(Protocol.TCP, packet, clientID);
    }
    public static void SendTCPAll(Packet packet)
    {
      SendAll(Protocol.TCP, packet);
    }
    public static void SendTCPAllExcept(Packet packet, int[] exceptClientIDs)
    {
      SendAllExcept(Protocol.TCP, packet, exceptClientIDs);
    }
    #endregion

    #region DataSending UDP
    public static void SendUDP(Packet packet, int clientID)
    {
      Send(Protocol.UDP, packet, clientID);
    }
    public static void SendUDPAll(Packet packet)
    {
      SendAll(Protocol.UDP, packet);
    }
    public static void SendUDPAllExcept(Packet packet, int[] exceptClientIDs)
    {
      SendAllExcept(Protocol.UDP, packet, exceptClientIDs);
    }
    #endregion

    public static void Send(Protocol protocol, Packet packet, int clientID)
    {
      packet.WriteLength();
      ServerClient client = Server.clients[clientID];
      SendData(protocol, packet, client);
    }

    public static void SendAll(Protocol protocol, Packet packet)
    {
      print("test");
      IterateClients((ServerClient client) =>
      {
        print(client.id);
        SendData(protocol, packet, client);
      });
    }

    public static void SendAllExcept(Protocol protocol, Packet packet, int[] exceptClientIDs)
    {
      // except ids
      IterateClients((ServerClient client) =>
      {
        SendData(protocol, packet, client);
      });
    }

    private static void SendData(Protocol protocol, Packet packet, ServerClient client)
    {
      if (protocol == Protocol.TCP) client.tcp.SendData(packet);
      if (protocol == Protocol.UDP) client.udp.SendData(packet);
    }


    #endregion
  }
}

