using UnityEngine;

namespace Flow
{
  namespace ClientSide
  {
    public class Spawn : FlowAction
    {

      override public void In(FlowPackage package)
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

      public void Out(int clientID, string username)
      {

      }

    }
  }

  namespace ServerSide
  {
    public class Spawn : FlowAction
    {

      override public void In(FlowPackage package, int clientID)
      {

      }

      public void Out(int clientID, Vector3 position, Quaternion rotation)
      {

        // create at server
        Instantiate(settings.SERVER_PLAYER_PREFAB, position, rotation);

        using (FlowPackage package = new FlowPackage(GetID()))
        {
          package.Write(clientID);
          package.Write(position);
          package.Write(rotation);

          // create player for passed client
          Server.TCPSend(package, clientID);

          // create enemy for other clients
          Server.TCPSendAllExcept(package, new int[] { clientID });
        }

        // create enemies for passed clientID
        Server.IterateClients((ServerClient client) =>
        {
          if (client.id != clientID)
          {
            using (FlowPackage package = new FlowPackage(GetID()))
            {
              package.Write(client.id);
              package.Write(position);
              package.Write(rotation);

              Server.TCPSend(package, clientID);
            }
          }
        });

      }
    }
  }
}


