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

    public int id = -999;
    public string endPoint = "";
    public bool isConnected = false;
    public NetPeer peer;

    private Timer disconnectTimer = new Timer();
    private const float DISCONNECT_DELAY = 0.35f;

    /// <summary>
    /// Connect this client to the passed peer
    /// </summary>
    /// <param name="_peer"></param>
    public FlowClientServer Connect(NetPeer _peer, bool wasConnected = false)
    {
      if (isConnected) return this;
      peer = _peer;
      peer.Tag = this;
      id = peer.Id;
      isConnected = true;
      endPoint = $"{peer.EndPoint.Address}:{peer.EndPoint.Port}";
      ConnectFlowServerAction action = FlowActions.GetActionByName("Connect") as ConnectFlowServerAction;

      if (wasConnected)
      {
        Logger.Log($"Client ({id}) has reconnected");
        action.Send(id, "You reconnected succesfully.");
      }
      else
      {
        if (FlowServer.clients.ContainsKey(id)) FlowServer.clients.Remove(id);
        Logger.Log($"Client ({id}) has connected");
        FlowServer.clients.Add(id, this);
        action.Send(id, "You connected succesfully.");
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