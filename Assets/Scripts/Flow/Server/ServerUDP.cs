using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Flow
{
  public class ServerUDP
  {

    private static Server server;
    private static FlowSettings settings;
    private static UdpClient listener;
    private static bool isStopped = true;

    public static bool StartListening()
    {
      isStopped = false;
      listener = new UdpClient(Server.settings.PORT);
      listener.BeginReceive(ReceiveCallback, null);
      return true;
    }

    public static bool StopListening()
    {
      isStopped = true;
      if (listener != null)
      {
        listener.Close();
        return true;
      }
      return false;
    }


    public static void SendData(IPEndPoint clientEndPoint, FlowPackage package)
    {
      try
      {
        if (clientEndPoint != null)
        {
          listener.BeginSend(package.ToArray(), package.Length(), clientEndPoint, null, null);
        }
      }
      catch (Exception ex)
      {
        Debug.Log($"Error sending data to {clientEndPoint} via UDP: {ex}");
      }
    }

    private static void ReceiveCallback(IAsyncResult result)
    {
      try
      {
        if (isStopped) return;

        IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
        byte[] _data = listener.EndReceive(result, ref endPoint);
        listener.BeginReceive(ReceiveCallback, null);

        if (_data.Length < 4) return;

        using (FlowPackage package = new FlowPackage(_data))
        {
          int clientID = package.ReadInt();
          if (clientID == 0) return;
          ServerClient client = Server.clients[clientID];

          if (!client.udp.isConnected)
          {
            client.udp.Connect(endPoint);
            return;
          }

          if (client.udp.endPoint.ToString() == endPoint.ToString())
          {
            // Ensures that the client is not being impersonated by another by sending a false clientID
            client.udp.HandleData(package);
          }
        }
      }
      catch (Exception ex)
      {
        Debug.Log($"Error receiving UDP data: {ex}");
      }
    }
  }
}