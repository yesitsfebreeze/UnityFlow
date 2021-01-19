using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;


public static class BuildMultiplayer
{

  [MenuItem("File/BuildMultiplayer")]

  static void PerformMultyPlayerBuild()
  {

    int playerCount = 2;

    EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);

    string server = GetScenePath("Server");
    BuildPlayerOptions serverOptions = new BuildPlayerOptions();
    serverOptions.scenes = new[] { server };
    serverOptions.target = BuildTarget.StandaloneWindows64;
    serverOptions.locationPathName = "build/Win64/server/server.exe";
    serverOptions.options = BuildOptions.EnableHeadlessMode | BuildOptions.AutoRunPlayer;

    BuildPipeline.BuildPlayer(serverOptions);

    for (int i = 1; i <= playerCount; i++)
    {

      string client = GetScenePath("Client");
      BuildPlayerOptions clientOptions = new BuildPlayerOptions();
      clientOptions.scenes = new[] { client };
      clientOptions.target = BuildTarget.StandaloneWindows64;
      clientOptions.locationPathName = "build/Win64/client" + i.ToString() + "/client.exe";
      clientOptions.options = BuildOptions.AutoRunPlayer;

      BuildPipeline.BuildPlayer(clientOptions);
    }

  }

  static string GetProjectName()

  {

    string[] s = Application.dataPath.Split('/');

    return s[s.Length - 2];

  }

  static string GetScenePath(string name)

  {

    string[] scenes = new string[EditorBuildSettings.scenes.Length];

    for (int i = 0; i < scenes.Length; i++)
    {
      EditorBuildSettingsScene scene = EditorBuildSettings.scenes[i];
      string scenePath = scene.path;
      if (scenePath.EndsWith(name + ".unity"))
      {
        return scene.path;
      }
    }

    return null;
  }

}