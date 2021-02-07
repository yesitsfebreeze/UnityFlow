using UnityEngine;
using System.Collections.Generic;

namespace Server
{
  public class ServerPlayerManager : MonoBehaviour
  {

    public int playerCount = 0;
    private int prevPlayerCount = 0;

    public static Dictionary<string, GameObject> playerPrefabs = new Dictionary<string, GameObject>();

    public static void AddPlayerPrefab(GameObject _playerPrefab, string _clientId)
    {
      playerPrefabs.Add(_clientId, _playerPrefab);
    }

    public static void RemovePlayerPrefab(string _clientId)
    {
      if (playerPrefabs.TryGetValue(_clientId, out GameObject prefab)) Destroy(prefab);
      playerPrefabs.Remove(_clientId);
    }

    void Update()
    {
      prevPlayerCount = playerCount;
      playerCount = playerPrefabs.Values.Count;
      if (prevPlayerCount != playerCount)
      {
        Flow.Logger.Log($"current player count on server ({playerCount})");
      }
    }
  }

}