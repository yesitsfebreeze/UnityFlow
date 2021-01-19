using UnityEngine;

[CreateAssetMenu(fileName = "NetworkSettings", menuName = "ScriptableObjects/NetworkSettings", order = 1)]

public class SO_NetworkSettings : ScriptableObject
{

  public bool DEVELOPMENT_MODE = true;

  public string IP = "127.0.0.1";
  public int MAX_PLAYERS = 50;
  public int PORT = 26950;
  public int TICK_RATE = 64;
  public int DATA_BUFFER_SIZE = 4096;
  public GameObject PLAYER_PREFAB;
  public GameObject ENEMY_PREFAB;
  public GameObject SERVER_PLAYER_PREFAB;

}

