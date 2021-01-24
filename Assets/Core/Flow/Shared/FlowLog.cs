using UnityEngine;
using System;
using static System.Linq.Enumerable;

namespace Flow
{
  public static class Logger
  {
    public static void Log(params object[] messages)
    {
      foreach (var i in Range(0, messages.Length))
      {
        if (IsHeadlessMode())
        {
          Console.WriteLine(messages[i] + " ");
        }
        else
        {
          Debug.Log(messages[i] + " ");
        }
      }
    }

    private static bool IsHeadlessMode()
    {
      return Application.isBatchMode;
    }
  }
}