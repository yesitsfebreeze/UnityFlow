using UnityEngine;
using Client;
using Server;


namespace Flow.Actions {
  public class SpawnFlowServerPackage : FlowPackage {
    public Vector3 position { get; set; }
    public Quaternion rotation { get; set; }
  }

  public class SpawnFlowClientAction : FlowAction {
    private void Handle(SpawnFlowServerPackage package) {
      if (package.clientId == FlowClient.id) {
        GameObject prefab = Instantiate(References.defaults.PLAYER_PREFAB, package.position, package.rotation); ;
        ClientPlayerManager.AddPlayerPrefab(prefab);
      } else {
        GameObject prefab = Instantiate(References.defaults.ENEMY_PREFAB, package.position, package.rotation);
        ClientPlayerManager.AddEnemyPrefab(prefab, package.clientId);
      }
    }
  }


  public class SpawnFlowServerAction : FlowAction {
    public void SendFrom(string clientId, Vector3 _position, Quaternion _rotation) {
      GameObject prefab = Instantiate(References.defaults.SERVER_PLAYER_PREFAB, _position, _rotation);
      ServerPlayerManager.AddPlayerPrefab(prefab, clientId);

      // spawn new client onn all existing clients
      SendPackage(new SpawnFlowServerPackage() {
        position = _position,
        rotation = _rotation
      }).SendAll(SendMethod.ReliableOrdered);

      // spawn already existing clients on the newly connected client
      FlowServer.IterateConnectedClients((FlowClientServer client) => {
        if (client.id != clientId) {
          SendPackage(new SpawnFlowServerPackage() {
            position = _position,
            rotation = _rotation
          }).Send(SendMethod.ReliableOrdered, clientId);
        }
        return true;
      });

    }
  }
}
