using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Networking
{
  public class ServerUDP
  {

    private static Server server;
    private static SO_NetworkSettings settings;
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


    public static void SendData(IPEndPoint _clientEndPoint, Packet _packet)
    {
      try
      {
        if (_clientEndPoint != null)
        {
          listener.BeginSend(_packet.ToArray(), _packet.Length(), _clientEndPoint, null, null);
        }
      }
      catch (Exception _ex)
      {
        Debug.Log($"Error sending data to {_clientEndPoint} via UDP: {_ex}");
      }
    }

    private static void ReceiveCallback(IAsyncResult _result)
    {
      try
      {
        if (isStopped) return;

        IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
        byte[] _data = listener.EndReceive(_result, ref endPoint);
        listener.BeginReceive(ReceiveCallback, null);

        if (_data.Length < 4) return;

        using (Packet _packet = new Packet(_data))
        {
          int clientID = _packet.ReadInt();
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
            client.udp.HandleData(_packet);
          }
        }
      }
      catch (Exception _ex)
      {
        Debug.Log($"Error receiving UDP data: {_ex}");
      }
    }
  }
}