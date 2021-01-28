using UnityEngine;

[CreateAssetMenu(fileName = "FlowSettings", menuName = "Flow/Settings", order = 1)]

public class FlowSettings : ScriptableObject
{

  public bool DEVELOPMENT_MODE = true;

  public string GAME_NAME = "ARENA";
  public string TEST_SCENE_SERVER = "Assets/Project/Server/Scenes/Server";
  public string TEST_SCENE_CLIENT = "Assets/Project/Client/Scenes/Client";
  public string IP = "127.0.0.1";
  public int MAX_PLAYERS = 50;
  public int PORT = 26950;
  public int TICK_RATE = 128;
  public float ConnectedCheckTime = 5f; // sec
  public int ReconenctAttempts = 5;
  public float ReconenctTime = 3f; // sec
}

