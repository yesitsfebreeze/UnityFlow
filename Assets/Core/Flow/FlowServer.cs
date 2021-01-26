using LiteNetLib;
using System.Collections.Generic;
using UnityEngine;
using Flow.Actions;

namespace Flow
{

  /// <summary>
  /// General server code for flow
  /// </summary>
  class FlowServer : MonoBehaviour, INetEventListener
  {
    public static bool isRunning = false;
    public FlowSettings FlowSettings;
    public static FlowSettings settings;
    public delegate bool IterateClientCallback(FlowClientServer client);
    public delegate void CreateClientCallback(FlowClientServer client);
    public static Dictionary<int, FlowClientServer> clients;
    private NetManager netManager;
    private FlowActions flowActions;


    /// <summary>
    /// create 
    /// </summary>
    void Awake()
    {
      Flow.isClient = false;
      Flow.isServer = true;

      clients = new Dictionary<int, FlowClientServer>();
      netManager = new NetManager(this) { AutoRecycle = true };
    }

    /// <summary>
    /// Sets up rudimentary stuff for the server
    /// </summary>
    void Setup()
    {
      settings = FlowSettings;
      FlowActions.settings = FlowSettings;
      flowActions = gameObject.AddComponent<FlowActions>();


      // setup unity to run on the defined tickrate
      Time.fixedDeltaTime = 1f / settings.TICK_RATE;
      QualitySettings.vSyncCount = 0;
      Application.targetFrameRate = settings.TICK_RATE;
    }

    /// <summary>
    /// unity method to start the server as soon as this behaviour is created
    /// </summary>
    void Start()
    {
      Setup();
      FlowActions.RegisterOnStartedCallback(StartServer);
    }

    /// <summary>
    /// actual server start method
    /// </summary>
    private void StartServer()
    {
      if (netManager.IsRunning) return;
      if (netManager.Start(settings.PORT))
      {
        Logger.Debug($"Server started listening on port {settings.PORT}");
        isRunning = true;
      }
      else
      {
        Logger.Debug("Server could not start!");
        return;
      }
    }


    /// <summary>
    /// Poll messages from the net manager
    /// </summary>
    void FixedUpdate()
    {
      // todo: check if fixed update has same time
      if (netManager.IsRunning) netManager.PollEvents();
    }

    /// <summary>
    /// Iterates over all connected clients (return false for early loop exit)
    /// </summary>
    public static void IterateConnectedClients(IterateClientCallback callback)
    {
      IterateClients((FlowClientServer client) =>
      {
        bool proceed = true;
        if (client.isConnected) proceed = callback(client);
        return proceed;
      });
    }

    /// <summary>
    /// Iterates over all clients (return false for early loop exit)
    /// </summary>
    public static void IterateClients(IterateClientCallback callback)
    {
      bool proceed = true;
      foreach (var id in clients.Keys)
      {
        if (!proceed) break;
        if (clients.ContainsKey(id))
        {
          proceed = callback(clients[id]);
        }
      }
    }

    /// <summary>
    /// creates a client in the next free slot
    /// </summary>
    /// <param name="callback"></param>
    public static void CreateClient(CreateClientCallback callback)
    {


    }

    /// <summary>
    /// Callback when connection is requested
    /// </summary>
    /// <param name="request"></param>
    void INetEventListener.OnConnectionRequest(ConnectionRequest request)
    {
      request.AcceptIfKey(settings.GAME_NAME);
    }

    /// <summary>
    /// Callback when peer has connected
    /// </summary>
    /// <param name="peer"></param>
    void INetEventListener.OnPeerConnected(NetPeer peer)
    {
      Logger.Debug($"Incomming connection from {peer.EndPoint.Address}:{peer.EndPoint.Port}");
      if (clients.ContainsKey(peer.Id) && clients.TryGetValue(peer.Id, out FlowClientServer existingClient))
      {

        if ($"{peer.EndPoint.Address}:{peer.EndPoint.Port}" == existingClient.endPoint)
        {
          existingClient.Connect(peer, true);
        }
        else
        {
          existingClient.Connect(peer, true);
        }

        return;
      }
      if (clients.Count >= settings.MAX_PLAYERS)
      {
        Logger.Log("Server is full");
        return;
      }
      FlowClientServer client = new FlowClientServer();
      client.Connect(peer);
    }


    /// <summary>
    /// Callback when peer has disconnected
    /// </summary>
    /// <param name="peer"></param>
    /// <param name="disconnectInfo"></param>
    void INetEventListener.OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
      Logger.Debug($"Connection to {peer.EndPoint.Address}:{peer.EndPoint.Port} closed. ({disconnectInfo.Reason.ToString()})");

      IterateConnectedClients((FlowClientServer client) =>
      {
        if (peer.Id == client.id) return client.Disconnect();
        return true;
      });
    }

    /// <summary>
    /// Callback when server receives packages
    /// </summary>
    /// <param name="peer"></param>
    /// <param name="reader"></param>
    /// <param name="deliveryMethod"></param>
    void INetEventListener.OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
    {
      FlowActions.processor.ReadAllPackets(reader, peer);
    }

    /// <summary>
    /// Callback when recieved an unconnected message
    /// </summary>
    /// <param name="remoteEndPoint"></param>
    /// <param name="reader"></param>
    /// <param name="messageType"></param>
    void INetEventListener.OnNetworkReceiveUnconnected(System.Net.IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType) { }

    /// <summary>
    /// Callback when latency has changed
    /// </summary>
    /// <param name="peer"></param>
    /// <param name="latency"></param>
    void INetEventListener.OnNetworkLatencyUpdate(NetPeer peer, int latency) { }

    /// <summary>
    /// Callback when a network error occured
    /// </summary>
    /// <param name="endPoint"></param>
    /// <param name="socketError"></param>
    void INetEventListener.OnNetworkError(System.Net.IPEndPoint endPoint, System.Net.Sockets.SocketError socketError) { }

  }

}


