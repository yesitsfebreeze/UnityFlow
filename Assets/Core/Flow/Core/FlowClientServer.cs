using LiteNetLib;
using Flow.Actions;
using System.Timers;
using System;
using System.Threading.Tasks;

namespace Flow
{

  /// <summary>
  /// Server side client
  /// </summary>
  public class FlowClientServer
  {

    public string id = "";
    public string endPoint = "";
    public bool isConnected = false;
    public NetPeer peer;

    private Timer disconnectTimer;
    private const float DISCONNECT_DELAY = 0.35f;

    /// <summary>
    /// Connect this client to the passed peer
    /// </summary>
    /// <param name="_peer"></param>
    public FlowClientServer Connect(NetPeer _peer, bool wasConnected = false)
    {
      if (isConnected) return this;
      peer = _peer;
      endPoint = $"{peer.EndPoint.Address}:{peer.EndPoint.Port}";
      id = Flow.CreateClientId(peer);
      isConnected = true;
      ConnectFlowServerAction action = FlowActions.GetActionByName("Connect") as ConnectFlowServerAction;

      if (wasConnected)
      {
        Logger.Debug($"Client ({id}) has reconnected");
        if (action != null) action.Send(id, "You reconnected succesfully.");
      }
      else
      {
        if (FlowServer.clients.ContainsKey(id)) FlowServer.clients.Remove(id);
        Logger.Debug($"Client ({id}) has connected");
        FlowServer.clients.Add(id, this);
        if (action != null) action.Send(id, "You connected succesfully.");
      }


      return this;
    }

    /// <summary>
    /// disconnect this client
    /// </summary>
    public bool Disconnect()
    {
      if (!isConnected) return true;
      isConnected = false;
      AsyncDisconnect();
      return false;
    }

    private async void AsyncDisconnect()
    {
      await Task.Delay((int)Math.Abs(DISCONNECT_DELAY * 1000));

      // peer.Disconnect();
      FlowServer.clients.Remove(id);
      Logger.Debug($"Client ({id}) has disconnected");
      DisconnectFlowServerAction action = FlowActions.GetActionByName("Disconnect") as DisconnectFlowServerAction;
      action.Send(id);
    }
  }
}