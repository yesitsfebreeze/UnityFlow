using UnityEngine;

namespace Flow.Actions
{

  /// <summary>Package data definition for data sent by the client.<summary>
  public class ConnectFlowClientPackage
  {
    public int clientId { get; set; }
    public string messsage { get; set; }
  }

  /// <summary>Package data definition for data sent by the Server.<summary>
  public class ConnectFlowServerPackage
  {
    public int clientId { get; set; }
    public string messsage { get; set; }
  }

  /// <summary>Handles packages on the client.<summary>
  public class ConnectFlowClientAction : FlowAction
  {

    /// <summary>Subscribes server packages on the client.<summary>
    public override void SubscribePackage()
    {
      processor.SubscribeReusable<ConnectFlowServerPackage>(Handle);
    }

    /// <summary>Handles a server package on the client.<summary>
    private void Handle(ConnectFlowServerPackage package)
    {
      Debug.Log(package.messsage);
      FlowClient.id = package.clientId;
      Send(FlowClient.id, $"Client ({FlowClient.id}) was acknowledged");
    }

    /// <summary>Sends the client package to the server.<summary>
    public void Send(int _clientId, string _messsage)
    {
      SendPackage(new ConnectFlowClientPackage()
      {
        clientId = _clientId,
        messsage = _messsage,
      }).Send(SendMethod.ReliableOrdered);
    }
  }


  /// <summary>Handles packages on the server.<summary>
  public class ConnectFlowServerAction : FlowAction
  {

    /// <summary>Subscribes client packages on the Server.<summary>
    public override void SubscribePackage()
    {
      processor.SubscribeReusable<ConnectFlowClientPackage>(Handle);
    }

    /// <summary>Handles a client package on the server.<summary>
    private void Handle(ConnectFlowClientPackage package)
    {
      Debug.Log(package.messsage);

      SpawnFlowServerAction spawnAction = FlowActions.GetActionByName("Spawn") as SpawnFlowServerAction;
      // Todo: replace the vector and rotation with the values of an actual spawnpoint
      spawnAction.Send(package.clientId, new Vector3(0, 4f, 0), Quaternion.identity);
    }

    /// <summary>Sends the server package to the client(s).<summary>
    public void Send(int _clientId, string _messsage)
    {
      SendPackage(new ConnectFlowServerPackage()
      {
        clientId = _clientId,
        messsage = _messsage,
      }).Send(SendMethod.ReliableOrdered, _clientId);
    }
  }
}
