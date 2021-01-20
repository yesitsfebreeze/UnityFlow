using UnityEngine;

namespace Networking
{

  public class SpawnAction : NetworkAction
  {

    override public void FromClient(int clientID, Packet packet)
    {

    }

    override public void FromServer(Packet packet)
    {
      int clientID = packet.ReadInt();
      Vector3 position = packet.ReadVector3();
      Quaternion rotation = packet.ReadQuaternion();

      if (LocalClient.instance.id == clientID)
      {
        Instantiate(settings.PLAYER_PREFAB, position, rotation);
      }
      else
      {
        print("spawn enemy");
        Instantiate(settings.ENEMY_PREFAB, position + new Vector3(3f, 0, 3f), rotation);
      }

    }


    public void ToServer(int clientID, string username)
    {

    }


    public void ToClient(int clientID, Vector3 position, Quaternion rotation)
    {

      // create at server
      Instantiate(settings.SERVER_PLAYER_PREFAB, position, rotation);

      // create player for local client
      using (Packet packet = new Packet(GetID()))
      {
        packet.Write(clientID);
        packet.Write(position);
        packet.Write(rotation);

        Server.SendTCPAll(packet);
      }


      // // create enemy for other local clients
      // Server.IterateClients((ServerClient client) =>
      // {
      //   // if (client.id != clientID)
      //   // {
      //   using (Packet packet = new Packet(GetID()))
      //   {
      //     packet.Write(clientID);
      //     packet.Write(position);
      //     packet.Write(rotation);

      //     Server.SendTCP(packet, client.id);
      //   }
      //   // }
      // });

    }
  }
}

