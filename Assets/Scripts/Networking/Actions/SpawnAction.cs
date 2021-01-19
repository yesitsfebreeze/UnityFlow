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

      print("--- ids");
      print(clientID);
      print(LocalClient.instance.localClientID);
      print("---");

      if (LocalClient.instance.localClientID == clientID)
      {
        Instantiate(settings.PLAYER_PREFAB, position, rotation);
      }
      else
      {
        print(clientID);

        print("spawn enemy");
        Instantiate(settings.ENEMY_PREFAB, position + new Vector3(3f, 0, 3f), rotation);
      }


    }


    public void ToServer(int clientID, string username)
    {

    }


    public void ToClient(int clientID, Vector3 position, Quaternion rotation)
    {
      Instantiate(settings.SERVER_PLAYER_PREFAB, position, rotation);

      using (Packet packet = new Packet(GetID()))
      {
        Debug.Log(clientID);

        packet.Write(clientID);
        packet.Write(position);
        packet.Write(rotation);

        GameServer.SendTCPData(packet, clientID);
        GameServer.SendTCPData(packet, 1);

        // GameServer.SendTCPDataToAllExcept(packet, new int[] { clientID });
      }
    }
  }
}

