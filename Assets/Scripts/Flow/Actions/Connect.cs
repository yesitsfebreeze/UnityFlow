using UnityEngine;

namespace Flow
{
  namespace ClientSide
  {
    public class Connect : FlowAction
    {
      override public void In(FlowPackage package)
      {
        Debug.Log("in");
        int clientID = package.ReadInt();
        string msg = package.ReadString();
        LocalClient.instance.id = clientID;

        Debug.Log(msg);

        Out(LocalClient.instance.id, "febreeze");
      }

      public void Out(int clientID, string username)
      {
        using (FlowPackage package = new FlowPackage(GetID()))
        {
          package.Write(clientID);
          package.Write(username);

          LocalClient.TCPSend(package);
        }
      }

    }
  }

  namespace ServerSide
  {
    public class Connect : FlowAction
    {

      override public void In(FlowPackage package, int clientID) // To Server
      {
        int clientIDCheck = package.ReadInt();
        string username = package.ReadString();


        if (clientID != clientIDCheck)
        {
          Debug.Log($"Client {clientID} tried to connect as client {clientIDCheck}. That is wrong!");
          return;

        }

        Debug.Log($"Player ({clientID}) has connected");

        Spawn action = FlowActions.Get("Spawn") as Spawn;
        action.Out(clientID, Vector3.zero + new Vector3(0, 4f, 0), Quaternion.identity);
      }

      public void Out(int clientID, string msg)
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
}


