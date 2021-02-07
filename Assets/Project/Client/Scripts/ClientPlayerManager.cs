using UnityEngine;
using System.Collections.Generic;

namespace Client
{
  public class ClientPlayerManager : MonoBehaviour
  {

    public static GameObject playerPrefab;
    public static Dictionary<string, GameObject> enemyPrefabs = new Dictionary<string, GameObject>();

    public static void AddPlayerPrefab(GameObject _playerPrefab)
    {
      playerPrefab = _playerPrefab;
    }

    public static void RemovePlayerPrefab()
    {
      Destroy(playerPrefab);
    }

    public static void AddEnemyPrefab(GameObject _enemyPrefab, string _clientId)
    {
      enemyPrefabs.Add(_clientId, _enemyPrefab);
    }

    public static void RemoveEnemyPrefab(string _clientId)
    {
      if (enemyPrefabs.TryGetValue(_clientId, out GameObject prefab)) Destroy(prefab);
      enemyPrefabs.Remove(_clientId);
    }
  }

}