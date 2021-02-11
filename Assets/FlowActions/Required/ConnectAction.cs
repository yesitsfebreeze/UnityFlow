using UnityEngine;

namespace Flow.Actions {

  public class ConnectFlowClientPackage : FlowPackage {
    public string messsage { get; set; }
  }

  public class ConnectFlowServerPackage : FlowPackage {
    public string messsage { get; set; }
  }

  public class ConnectFlowClientAction : FlowAction {

    public void Handle(ConnectFlowServerPackage package) {
      Debug.Log(package.messsage);
      FlowClient.id = package.clientId;
      Send(FlowClient.id, $"Client ({FlowClient.id}) was acknowledged");
    }

    public void Send(string clientId, string _messsage) {
      SendPackage(new ConnectFlowClientPackage() {
        clientId = clientId,
        messsage = _messsage,
      }).Send(SendMethod.ReliableOrdered);
    }
  }


  public class ConnectFlowServerAction : FlowAction {

    public void Handle(ConnectFlowClientPackage package) {
      Debug.Log(package.messsage);

      SpawnFlowServerAction spawnAction = FlowActions.GetActionByName("Spawn") as SpawnFlowServerAction;
      // Todo: replace the vector and rotation with the values of an actual spawnpoint
      spawnAction.SendFrom(package.clientId, new Vector3(0, 4f, 0), Quaternion.identity);
    }

    public void SendFrom(string clientId, string _messsage) {
      SendPackage(new ConnectFlowServerPackage() {
        messsage = _messsage,
      }).Send(SendMethod.ReliableOrdered, clientId);
    }
  }
}
