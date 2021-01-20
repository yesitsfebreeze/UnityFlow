using UnityEngine;

[CreateAssetMenu(fileName = "FlowSettings", menuName = "ScriptableObjects/FlowSettings", order = 1)]

public class FlowSettings : ScriptableObject
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

  public string[] actions = new string[] {
    "Connect",
    "Disconnect",
    "Spawn",
  };

}

