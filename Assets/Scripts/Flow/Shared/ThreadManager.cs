using System;
using System.Collections.Generic;
using UnityEngine;

namespace Flow
{
  public class ThreadManager : MonoBehaviour
  {
    private static readonly List<Action> executeOnMainThread = new List<Action>();
    private static readonly List<Action> executeCopiedOnMainThread = new List<Action>();
    private static bool actionToExecuteOnMainThread = false;

    public bool isClient = false;

    private void Update()
    {
      if (isClient)
      {
        UpdateMain();
      }
    }

    private void FixedUpdate()
    {
      if (!isClient)
      {
        UpdateMain();
      }
    }

    /// <summary>Sets an action to be executed on the main thread.</summary>
    /// <param name="action">The action to be executed on the main thread.</param>
    public static void ExecuteOnMainThread(Action action)
    {
      if (action == null)
      {
        Debug.Log("No action to execute on main thread!");
        return;
      }

      lock (executeOnMainThread)
      {
        executeOnMainThread.Add(action);
        actionToExecuteOnMainThread = true;
      }
    }

    /// <summary>Executes all code meant to run on the main thread. NOTE: Call this ONLY from the main thread.</summary>
    public static void UpdateMain()
    {
      if (actionToExecuteOnMainThread)
      {
        executeCopiedOnMainThread.Clear();
        lock (executeOnMainThread)
        {
          executeCopiedOnMainThread.AddRange(executeOnMainThread);
          executeOnMainThread.Clear();
          actionToExecuteOnMainThread = false;
        }

        for (int i = 0; i < executeCopiedOnMainThread.Count; i++)
        {
          executeCopiedOnMainThread[i]();
        }
      }
    }
  }
}
