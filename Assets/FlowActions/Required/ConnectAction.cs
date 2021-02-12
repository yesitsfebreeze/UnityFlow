using UnityEngine;

namespace Flow.Actions {

  public class ConnectFlowClientPackage : FlowPackage {
  }

  public class ConnectFlowServerPackage : FlowPackage {
    public string messsage { get; set; }
  }

  public class ConnectFlowClientAction : FlowAction {

    public void Handle(ConnectFlowServerPackage package) {
      FlowClient.id = package.clientId;
      Send();
    }

    public void Send() {
      SendPackage(new ConnectFlowClientPackage() { }).Send(SendMethod.ReliableOrdered);
    }
  }


  public class ConnectFlowServerAction : FlowAction {

    public void Handle(ConnectFlowClientPackage package) {
      if (FlowServer.clients.TryGetValue(package.clientId, out FlowClientServer client)) {
        Debug.Log($"Client ({client.id}) was acknowledged");

        SpawnFlowServerAction spawnAction = FlowActions.GetActionByName("Spawn") as SpawnFlowServerAction;
        // Todo: replace the vector and rotation with the values of an actual spawnpoint
        spawnAction.SendFrom(client.id, new Vector3(0, 4f, 0), Quaternion.identity);
      }
    }

    public void SendFrom(string clientId, string _messsage) {
      SendPackageFrom(clientId, new ConnectFlowServerPackage() {
        messsage = _messsage,
      }).Send(SendMethod.ReliableOrdered, clientId);
    }
  }
}
