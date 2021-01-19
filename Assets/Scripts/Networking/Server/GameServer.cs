using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Networking
{

  public class GameServer : MonoBehaviour
  {
    public SO_NetworkSettings NetworkSettings;
    private static SO_NetworkSettings settings;
    public static Dictionary<int, GameServerClient> clients = new Dictionary<int, GameServerClient>();

    private static TcpListener tcpListener;
    private static UdpClient udpListener;

    public static bool IsHeadlessMode()
    {
#if UNITY_EDITOR
      return true;
#else
        return UnityEngine.SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null;
#endif
    }

    void Start()
    {
      Debug.Log("Starting Server....");

      settings = NetworkSettings;
      SetupUnity();
      StartServer();
    }

    void OnDestroy()
    {
      StopServer();
    }

    private static void SetupUnity()
    {
      Time.fixedDeltaTime = 1f / settings.TICK_RATE;
      QualitySettings.vSyncCount = 0;
      Application.targetFrameRate = settings.TICK_RATE;
    }


    /// <summary>Starts the server.</summary>
    public static void StartServer()
    {
      InitializeClients();

      tcpListener = new TcpListener(IPAddress.Any, settings.PORT);
      tcpListener.Start();
      tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);

      udpListener = new UdpClient(settings.PORT);
      udpListener.BeginReceive(UDPReceiveCallback, null);

      Debug.Log($"Server started on port {settings.PORT}.");
    }

    public static void StopServer()
    {
      if (tcpListener != null) tcpListener.Stop();
      if (udpListener != null) udpListener.Close();


      Debug.Log("Server stopped.");
    }

    /// <summary>Handles new TCP connections.</summary>
    private static void TCPConnectCallback(IAsyncResult _result)
    {
      TcpClient _client = tcpListener.EndAcceptTcpClient(_result);
      tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);
      Debug.Log($"Incoming connection from {_client.Client.RemoteEndPoint}...");

      for (int i = 1; i <= settings.MAX_PLAYERS; i++)
      {
        if (clients[i].tcp.socket == null)
        {
          clients[i].tcp.Connect(_client);
          return;
        }
      }

      Debug.Log($"{_client.Client.RemoteEndPoint} failed to connect: Server full!");
    }

    /// <summary>Receives incoming UDP data.</summary>
    private static void UDPReceiveCallback(IAsyncResult _result)
    {

      try
      {
        IPEndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
        byte[] _data = udpListener.EndReceive(_result, ref _clientEndPoint);
        udpListener.BeginReceive(UDPReceiveCallback, null);

        if (_data.Length < 4)
        {
          return;
        }

        using (Packet _packet = new Packet(_data))
        {
          int _clientId = _packet.ReadInt();

          if (_clientId == 0)
          {
            return;
          }

          if (clients[_clientId].udp.endPoint == null)
          {
            // If this is a new connection
            clients[_clientId].udp.Connect(_clientEndPoint);
            return;
          }

          if (clients[_clientId].udp.endPoint.ToString() == _clientEndPoint.ToString())
          {
            // Ensures that the client is not being impersonated by another by sending a false clientID
            clients[_clientId].udp.HandleData(_packet);
          }
        }
      }
      catch (Exception _ex)
      {
        Debug.Log($"Error receiving UDP data: {_ex}");
      }
    }

    /// <summary>Sends a packet to the specified endpoint via UDP.</summary>
    /// <param name="_clientEndPoint">The endpoint to send the packet to.</param>
    /// <param name="_packet">The packet to send.</param>
    public static void SendUDPData(IPEndPoint _clientEndPoint, Packet _packet)
    {
      try
      {
        if (_clientEndPoint != null)
        {
          udpListener.BeginSend(_packet.ToArray(), _packet.Length(), _clientEndPoint, null, null);
        }
      }
      catch (Exception _ex)
      {
        Debug.Log($"Error sending data to {_clientEndPoint} via UDP: {_ex}");
      }
    }

    /// <summary>Initializes all necessary server data.</summary>
    private static void InitializeClients()
    {
      for (int i = 1; i <= settings.MAX_PLAYERS; i++)
      {
        clients.Add(i, new GameServerClient(i, settings));
      }
      Debug.Log("Initialized Clients.");
    }

    #region SendData

    #region SendDataTCP

    // <summary>Sends a packet to a client via TCP.</summary>
    /// <param name="_toClient">The client to send the packet the packet to.</param>
    /// <param name="_packet">The packet to send to the client.</param>
    public static void SendTCPData(Packet _packet, int _toClient)
    {
      print(_toClient);
      print(_packet);
      _packet.WriteLength(); 
      int id = _packet.ReadInt();
      GameServer.clients[_toClient].tcp.SendData(_packet);
    }

    /// <summary>Sends a packet to all clients via TCP.</summary>
    /// <param name="_packet">The packet to send.</param>
    public static void SendTCPDataToAll(Packet _packet)
    {
      _packet.WriteLength();
      for (int i = 1; i <= settings.MAX_PLAYERS; i++)
      {
        SendTCPData(_packet, i);
      }
    }

    /// <summary>Sends a packet to all clients except one via TCP.</summary>
    /// <param name="_exceptClient">The client to NOT send the data to.</param>
    /// <param name="_packet">The packet to send.</param>
    public static void SendTCPDataToAllExcept(Packet _packet, int[] _exceptClients)
    {
      _packet.WriteLength();
      for (int i = 1; i <= settings.MAX_PLAYERS; i++)
      {
        if (!Array.Exists(_exceptClients, el => el == i))
        {
          Debug.Log($"sent to {i}");
          SendTCPData(_packet, i);

        }
      }
    }

    #endregion

    #region SendDataUDP
    /// <summary>Sends a packet to a client via UDP.</summary>
    /// <param name="_toClient">The client to send the packet the packet to.</param>
    /// <param name="_packet">The packet to send to the client.</param>
    public static void SendUDPData(Packet _packet, int _toClient)
    {
      _packet.WriteLength();
      GameServer.clients[_toClient].udp.SendData(_packet);
    }

    /// <summary>Sends a packet to all clients via UDP.</summary>
    /// <param name="_packet">The packet to send.</param>
    public static void SendUDPDataToAll(Packet _packet)
    {
      _packet.WriteLength();
      for (int i = 1; i <= settings.MAX_PLAYERS; i++)
      {
        GameServer.clients[i].udp.SendData(_packet);
      }
    }
    /// <summary>Sends a packet to all clients except one via UDP.</summary>
    /// <param name="_exceptClient">The client to NOT send the data to.</param>
    /// <param name="_packet">The packet to send.</param>
    public static void SendUDPDataToAllExcept(Packet _packet, int[] _exceptClients)
    {
      _packet.WriteLength();
      for (int i = 1; i <= settings.MAX_PLAYERS; i++)
      {
        if (!Array.Exists(_exceptClients, el => el == i))
        {
          GameServer.clients[i].udp.SendData(_packet);
        }
      }
    }

    #endregion

    #endregion
  }
}