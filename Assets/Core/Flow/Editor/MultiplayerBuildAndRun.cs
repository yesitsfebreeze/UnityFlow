using UnityEngine;
using UnityEditor;

/// <summary>
/// builds the server & client 
/// </summary>
public static class BuildMultiplayer {

  [MenuItem("Flow/Build (1 Player) %F1")]
  static void PerformMultyPlayerBuildOne() {
    StartBuild(1);
  }

  [MenuItem("Flow/Build (2 Players) %F2")]
  static void PerformMultyPlayerBuildTwo() {
    StartBuild(2);
  }

  [MenuItem("Flow/Build (3 Players) %F3")]
  static void PerformMultyPlayerBuildThree() {
    StartBuild(3);
  }

  [MenuItem("Flow/Build (4 Players) %F4")]
  static void PerformMultyPlayerBuildFour() {
    StartBuild(4);
  }

  [MenuItem("Flow/Build (Server Only) %F12")]
  static void PerformMultyPlayerBuildServer() {
    StartBuild(0);
  }


  private static void StartBuild(int playerCount) {

    FlowSettings settings = ScriptableObject.CreateInstance<FlowSettings>();
    BuildTarget target = BuildTarget.StandaloneWindows64;
    EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, target);

    string server = GetScenePath(settings.TEST_SCENE_SERVER);
    BuildPlayerOptions serverOptions = new BuildPlayerOptions();
    serverOptions.scenes = new[] { server };
    serverOptions.target = target;
    serverOptions.locationPathName = "Build/server/server.exe";
    serverOptions.options = BuildOptions.EnableHeadlessMode | BuildOptions.AutoRunPlayer;

    BuildPipeline.BuildPlayer(serverOptions);

    for (int i = 1; i <= playerCount; i++) {
      string client = GetScenePath(settings.TEST_SCENE_CLIENT);
      BuildPlayerOptions clientOptions = new BuildPlayerOptions();
      clientOptions.scenes = new[] { client };
      clientOptions.target = target;
      clientOptions.locationPathName = "Build/clients/client" + i.ToString() + "/client.exe";
      clientOptions.options = BuildOptions.AutoRunPlayer;

      BuildPipeline.BuildPlayer(clientOptions);
    }
  }

  static string GetProjectName() {

    string[] s = Application.dataPath.Split('/');

    return s[s.Length - 2];

  }

  static string GetScenePath(string name) {

    string[] scenes = new string[EditorBuildSettings.scenes.Length];

    for (int i = 0; i < scenes.Length; i++) {
      EditorBuildSettingsScene scene = EditorBuildSettings.scenes[i];
      string scenePath = scene.path;
      if (scenePath.EndsWith(name + ".unity")) {
        return scene.path;
      }
    }

    return null;
  }

}