using UnityEngine;
using Flow;

namespace FlowActions
{
  public class SpawnFlow : FlowAction
  {

    override public void FromClient(int clientID, FlowPackage package)
    {

    }

    override public void FromServer(FlowPackage package)
    {
      int clientID = package.ReadInt();
      Vector3 position = package.ReadVector3();
      Quaternion rotation = package.ReadQuaternion();

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
      using (FlowPackage package = new FlowPackage(GetID()))
      {
        package.Write(clientID);
        package.Write(position);
        package.Write(rotation);

        Server.TCPSend(package, clientID);
        // Server.SendTCPAll(package);
      }


      // // create enemy for other local clients
      // Server.IterateClients((ServerClient client) =>
      // {
      //   // if (client.id != clientID)
      //   // {
      //   using (FlowPackage package = new FlowPackage(GetID()))
      //   {
      //     package.Write(clientID);
      //     package.Write(position);
      //     package.Write(rotation);

      //     Server.SendTCP(package, client.id);
      //   }
      //   // }
      // });

    }
  }
}


