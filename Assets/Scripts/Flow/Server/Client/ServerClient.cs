using UnityEngine;
using Flow.ServerSide;

namespace Flow
{

  public class ServerClient
  {
    public int id;
    public ServerClientTCP tcp;
    public ServerClientUDP udp;
    public bool isConnected = false;
    public bool wasConnected = false;

    public ServerClient(int clientId)
    {
      id = clientId;
      tcp = new ServerClientTCP(id);
      udp = new ServerClientUDP(id);
    }

    public bool IsConnected()
    {
      if (udp.isConnected && tcp.isConnected)
      {
        wasConnected = true;
        isConnected = true;
        return isConnected;
      }

      isConnected = false;
      return isConnected;
    }



    /// <summary>Disconnects the client and stops all network traffic.</summary>
    public void Disconnect()
    {
      Debug.Log($"Player {id} has disconnected.");
      tcp.Disconnect();
      udp.Disconnect();
      IsConnected();

      Disconnect action = FlowActions.Get("Disconnect") as Disconnect;
      action.Out(id, $"Player ({id}) has disconnected.");
    }
  }
}