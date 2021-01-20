using UnityEngine;

namespace Networking
{

  public class ConnectAction : NetworkAction
  {

    override public void FromClient(int clientID, Package package)
    {
      int clientIDCheck = package.ReadInt();
      string username = package.ReadString();


      if (clientID != clientIDCheck)
      {
        Debug.Log($"Client {clientID} tried to connect as client {clientIDCheck}. That is wrong!");
        return;

      }

      Debug.Log($"Player ({clientID}) has connected");

      SpawnAction action = Actions.Get("Spawn") as SpawnAction;
      action.ToClient(clientID, Vector3.zero + new Vector3(0, 4f, 0), Quaternion.identity);
    }

    public void ToServer(int clientID, string username)
    {
      using (Package package = new Package(GetID()))
      {
        package.Write(clientID);
        package.Write(username);
        LocalClient.SendTCPData(package);
      }
    }


    public void ToClient(int clientID, string msg)
    {
      using (Package package = new Package(GetID()))
      {
        package.Write(clientID);
        package.Write(msg);

        Server.SendTCP(package, clientID);
      }
    }

    override public void FromServer(Package package)
    {

      Debug.Log("package received");

      int clientID = package.ReadInt();
      string msg = package.ReadString();
      LocalClient.instance.id = clientID;

      Debug.Log(msg);

      ToServer(LocalClient.instance.id, "febreeze");
    }
  }
}

