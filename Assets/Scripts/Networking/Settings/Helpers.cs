


namespace Networking
{

  public static class Helpers
  {

    public static bool IsHeadlessMode()
    {
#if UNITY_EDITOR
      return true;
#else
      return UnityEngine.SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null;
#endif
    }

  }

}


