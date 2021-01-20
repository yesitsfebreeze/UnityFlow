using UnityEngine;
using Flow;

namespace Flow
{
  public class ConnectFlowAction : FlowAction
  {

    override public void FromClient(int clientID, FlowPackage package)
    {
      int clientIDCheck = package.ReadInt();
      string username = package.ReadString();


      if (clientID != clientIDCheck)
      {
        Debug.Log($"Client {clientID} tried to connect as client {clientIDCheck}. That is wrong!");
        return;

      }

      Debug.Log($"Player ({clientID}) has connected");

      SpawnFlowAction action = FlowActions.Get("Spawn") as SpawnFlowAction;
      action.ToClient(clientID, Vector3.zero + new Vector3(0, 4f, 0), Quaternion.identity);
    }

    public void ToServer(int clientID, string username)
    {
      using (FlowPackage package = new FlowPackage(GetID()))
      {
        package.Write(clientID);
        package.Write(username);
        LocalClient.TCPSend(package);
      }
    }

    override public void FromServer(FlowPackage package)
    {
      int clientID = package.ReadInt();
      string msg = package.ReadString();
      LocalClient.instance.id = clientID;

      Debug.Log(msg);

      ToServer(LocalClient.instance.id, "febreeze");
    }

    public void ToClient(int clientID, string msg)
    {
      using (FlowPackage package = new FlowPackage(GetID()))
      {
        package.Write(clientID);
        package.Write(msg);

        Server.TCPSend(package, clientID);
      }
    }
  }
}


