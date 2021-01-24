using LiteNetLib;
using Flow.Shared;
using Flow.Actions;
using System.Collections.Generic;
using System.Timers;
using System;

namespace Flow.Server
{

  /// <summary>
  /// Server side client
  /// </summary>
  public class FlowClientServer
  {

    public int id = -1;
    public bool isConnected = false;
    public NetPeer peer;

    private Timer disconnectTimer = new Timer();
    private const float DISCONNECT_DELAY = 0.35f;

    /// <summary>
    /// Connect this client to the passed peer
    /// </summary>
    /// <param name="_peer"></param>
    public void Connect(NetPeer _peer)
    {
      if (isConnected) return;
      peer = _peer;
      peer.Tag = this;
      isConnected = true;
      Logger.Log($"Client ({id}) has connected");
      ConnectFlowServerAction action = FlowActions.GetActionByName("Connect") as ConnectFlowServerAction;
      action.Send(id, "You connected succesfully.");
    }

    /// <summary>
    /// disconnect this client
    /// </summary>
    public bool Disconnect()
    {
      if (!isConnected) return true;
      isConnected = false;
      StartAsyncDisconnectPeer();
      return false;
    }

    private void StartAsyncDisconnectPeer()
    {
      disconnectTimer.Interval = DISCONNECT_DELAY * 1000;
      disconnectTimer.Elapsed += (Object source, System.Timers.ElapsedEventArgs e) =>
      {
        peer.Disconnect();
        FlowServer.clients.Remove(id);
        Logger.Log($"Client ({id}) has disconnected");
        DisconnectFlowServerAction action = FlowActions.GetActionByName("Disconnect") as DisconnectFlowServerAction;
        action.Send(id);
        disconnectTimer.Enabled = false;
      };
      disconnectTimer.Enabled = true;
    }
  }
}