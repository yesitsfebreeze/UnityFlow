/// autogenerated Disconnect FlowAction, most likely needs manual adjustment...
using Client;
using Server;

namespace Flow.Actions {

  public class DisconnectFlowServerPackage : FlowPackage {
    public string message { get; set; }
  }

  public class DisconnectFlowClientAction : FlowAction {
    public void Handle(DisconnectFlowServerPackage package) {
      Logger.Log(package.message);
      if (package.clientId == FlowClient.id) {
        ClientPlayerManager.RemovePlayerPrefab();
      } else {
        ClientPlayerManager.RemoveEnemyPrefab(package.clientId);
      }
    }
  }

  public class DisconnectFlowServerAction : FlowAction {
    public void SendFrom(string clientId) {
      ServerPlayerManager.RemovePlayerPrefab(clientId);

      SendPackageFrom(clientId, new DisconnectFlowServerPackage() {
        message = $"Client ({clientId}) has disconnected.",
      }).SendExcept(SendMethod.ReliableUnordered, new string[] { clientId });
    }
  }
}
