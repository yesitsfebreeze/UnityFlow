using UnityEngine;
using UnityEngine.Events;
using System;
using static System.Linq.Enumerable;

namespace Flow
{

  public static class Logger
  {

    public static UnityAction<string> OnMessage;

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
          UnityEngine.Debug.Log(messages[i] + " ");
          if (OnMessage != null) OnMessage.Invoke(messages[i] + " ");
        }
      }
    }

    public static void Debug(params object[] messages)
    {
      foreach (var i in Range(0, messages.Length))
      {
        if (IsHeadlessMode())
        {
          Console.WriteLine(messages[i] + " ");
        }
        else
        {
          UnityEngine.Debug.Log(messages[i] + " ");
        }
      }
    }

    private static bool IsHeadlessMode()
    {
      return Application.isBatchMode;
    }
  }
}