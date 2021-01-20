using UnityEngine;

namespace Networking
{

  public class DisconnectAction : NetworkAction
  {

    override public void FromClient(int clientID, Packet packet)
    {
      int _clientIDCheck = packet.ReadInt();
      string _username = packet.ReadString();


      if (clientID != _clientIDCheck)
      {
        Debug.Log($"Client {clientID} tried to connect as client {_clientIDCheck}. That is wrong!");
        return;

      }

      Debug.Log($"Player ({clientID}) has connected");

      SpawnAction action = Actions.Get("Spawn") as SpawnAction;
      action.ToClient(clientID, Vector3.zero + new Vector3(0, 4f, 0), Quaternion.identity);
    }

    public void ToServer(int clientID, string username)
    {
      using (Packet packet = new Packet(GetID()))
      {
        packet.Write(clientID);
        packet.Write(username);
        LocalClient.SendTCPData(packet);
      }
    }


    public void ToClient(int clientID, string msg)
    {

      print(clientID);
      print(msg);
      using (Packet packet = new Packet(GetID()))
      {
        packet.Write(clientID);
        packet.Write(msg);

        Server.SendTCP(packet, clientID);
      }
    }

    override public void FromServer(Packet packet)
    {
      int clientID = packet.ReadInt();
      string msg = packet.ReadString();
      LocalClient.instance.id = clientID;

      ToServer(LocalClient.instance.id, "febreeze");
    }
  }
}

