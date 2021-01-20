using UnityEngine;
using Networking;

namespace NetworkingActions
{
  public class NA_Disconnect : NetworkAction
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

      NA_Spawn action = Actions.Get("Spawn") as NA_Spawn;
      action.ToClient(clientID, Vector3.zero + new Vector3(0, 4f, 0), Quaternion.identity);
    }

    public void ToServer(int clientID, string username)
    {
      using (Package package = new Package(GetID()))
      {
        package.Write(clientID);
        package.Write(username);
        LocalClient.TCPSend(package);
      }
    }


    public void ToClient(int clientID, string msg)
    {

      print(clientID);
      print(msg);
      using (Package package = new Package(GetID()))
      {
        package.Write(clientID);
        package.Write(msg);

        Server.TCPSend(package, clientID);
      }
    }

    override public void FromServer(Package package)
    {
      int clientID = package.ReadInt();
      string msg = package.ReadString();
      LocalClient.instance.id = clientID;

      ToServer(LocalClient.instance.id, "febreeze");
    }
  }
}


