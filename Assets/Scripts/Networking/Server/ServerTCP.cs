using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Networking
{

  public static class ServerTCP
  {

    private static Server server;
    private static SO_NetworkSettings settings;
    private static TcpListener listener;
    private static bool isStopped = true;

    public static bool StartListening()
    {
      isStopped = false;
      listener = new TcpListener(IPAddress.Any, Server.settings.PORT);
      listener.Start();
      listener.BeginAcceptTcpClient(ConnectCallback, null);

      return true;
    }

    public static bool StopListening()
    {
      if (listener != null)
      {
        listener.Stop();
        isStopped = true;
        return true;
      }
      return false;
    }

    /// <summary>Handles new TCP connections.</summary>
    private static void ConnectCallback(IAsyncResult _result)
    {
      if (isStopped) return;

      TcpClient _client = listener.EndAcceptTcpClient(_result);
      listener.BeginAcceptTcpClient(ConnectCallback, null);
      Debug.Log($"Incoming connection from {_client.Client.RemoteEndPoint}...");

      for (int i = 1; i <= Server.settings.MAX_PLAYERS; i++)
      {
        if (Server.clients[i].tcp.socket == null)
        {
          Server.clients[i].tcp.Connect(_client);
          return;
        }
      }

      Debug.Log($"{_client.Client.RemoteEndPoint} failed to connect: Server full!");
    }
  }
}