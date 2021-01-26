using LiteNetLib;
using UnityEngine;
using System.Collections;
using Flow.Actions;

namespace Flow
{

  public class FlowClient : MonoBehaviour, INetEventListener
  {

    public static int id = -1;
    public static bool isConnected = false;
    public FlowSettings FlowSettings;
    public static FlowSettings settings;
    public static NetPeer peer;
    private FlowActions flowActions;
    private NetManager netManager;

    private bool IsEnabled = false;
    private const float DISCONNECT_DELAY = 0.35f;

    /// <summary>
    /// create need instances
    /// </summary>
    void Awake()
    {
      Flow.isClient = true;
      Flow.isServer = false;
      netManager = new NetManager(this) { AutoRecycle = true, IPv6Enabled = IPv6Mode.Disabled };
    }

    /// <summary>
    /// Connects the client when this behavriour is initialized
    /// </summary>
    void Start()
    {
      Setup();
      FlowActions.RegisterOnStartedCallback(Connect);
    }

    /// <summary>
    /// setup necessary stuff for the client
    /// </summary>
    void Setup()
    {
      settings = FlowSettings;
      FlowActions.settings = settings;
      FlowActions.isClient = true;
      flowActions = gameObject.AddComponent<FlowActions>();
      IsEnabled = true;
    }

    /// <summary>
    /// Connects the client to the server
    /// </summary>
    private void Connect()
    {
      if (!IsEnabled) return;

      if (netManager.IsRunning) netManager.Stop();

      if (netManager.Start())
      {
        netManager.Connect(settings.IP, settings.PORT, settings.GAME_NAME);
        isConnected = true;
        Logger.Log("Trying to connecting to server...");
      }
      else
      {
        isConnected = false;
        Logger.Log("Could not connect to server...");
      }
    }

    /// <summary>
    /// Disconnects the client from the server
    /// </summary>
    public void Disconnect()
    {
      isConnected = false;
      netManager.Stop();
      this.enabled = false;
    }

    /// <summary>
    /// Connects the client from the server when behaviour is enabled
    /// </summary>
    void OnEnable()
    {
      Connect();
    }

    /// <summary>
    /// Disconnects the client from the server when behaviour is disabled
    /// </summary>
    void OnDisable()
    {
      if (isConnected) Disconnect();
    }

    /// <summary>
    /// Destory the client
    /// </summary>
    void OnDestroy()
    {
      Destroy(flowActions);
      Disconnect();
    }

    /// <summary>
    /// Stops the netmanager after a short delay
    /// </summary>
    /// <returns></returns>
    IEnumerator StopNetManager()
    {
      yield return new WaitForSeconds(DISCONNECT_DELAY);
      netManager.Stop();
    }

    /// <summary>
    /// Poll packages from the server
    /// </summary>
    void Update()
    {
      if (netManager.IsRunning) netManager.PollEvents();
    }

    /// <summary>
    /// Callback when connection is requested (not needed on client)
    /// </summary>
    /// <param name="request"></param>
    void INetEventListener.OnConnectionRequest(ConnectionRequest request) { }

    /// <summary>
    /// Callback when the client peer has connected to the server
    /// </summary>
    /// <param name="ServerPeer"></param>
    void INetEventListener.OnPeerConnected(NetPeer ServerPeer)
    {
      peer = ServerPeer;
      Logger.Debug($"Connection established to {peer.EndPoint.Address}:{peer.EndPoint.Port}");
    }

    /// <summary>
    /// callback when the client peer has disconnected form the server
    /// </summary>
    /// <param name="peer"></param>
    /// <param name="disconnectInfo"></param>
    void INetEventListener.OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {

      // todo: implement
    }

    /// <summary>
    /// Callback when the client receives packages
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
    void INetEventListener.OnNetworkReceiveUnconnected(System.Net.IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
    {
      Logger.Debug($"NetworkReceiveUnconnected: {reader.UserDataSize}");
    }

    // <summary>
    /// Callback when client latency has changed
    /// </summary>
    /// <param name="peer"></param>
    /// <param name="latency"></param>
    void INetEventListener.OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
      Logger.Debug($"latency update {latency}");
    }

    /// <summary>
    /// Callback when a network error occured
    /// </summary>
    /// <param name="endPoint"></param>
    /// <param name="socketError"></param>
    void INetEventListener.OnNetworkError(System.Net.IPEndPoint endPoint, System.Net.Sockets.SocketError socketError)
    {
      Logger.Debug($"NetworkError: {socketError}");
    }
  }
}
