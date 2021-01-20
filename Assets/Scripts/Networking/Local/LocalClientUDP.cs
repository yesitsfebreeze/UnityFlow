using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

namespace Networking
{
  public class LocalClientUDP
  {
    public bool isConnected = false;
    public UdpClient socket;
    private IPEndPoint endPoint;
    private LocalClient client;


    public LocalClientUDP()
    {
      client = LocalClient.instance;
      endPoint = new IPEndPoint(IPAddress.Parse(client.ip), client.port);
    }

    public void Connect(int port)
    {
      isConnected = true;
      socket = new UdpClient(port);

      socket.Connect(endPoint);
      socket.BeginReceive(ReceiveCallback, null);

      using (Packet _packet = new Packet())
      {
        SendData(_packet);
      }
    }

    /// <summary>Sends data to the client via UDP.</summary>
    /// <param name="_packet">The packet to send.</param>
    public void SendData(Packet _packet)
    {
      try
      {
        _packet.InsertInt(client.id); // Insert the client's ID at the start of the packet
        if (socket != null)
        {
          socket.BeginSend(_packet.ToArray(), _packet.Length(), null, null);
        }
      }
      catch (Exception _ex)
      {
        Debug.Log($"Error sending data to server via UDP: {_ex}");
      }
    }

    /// <summary>Receives incoming UDP data.</summary>
    private void ReceiveCallback(IAsyncResult _result)
    {
      try
      {
        byte[] _data = socket.EndReceive(_result, ref endPoint);
        socket.BeginReceive(ReceiveCallback, null);

        if (_data.Length < 4)
        {
          client.Disconnect();
          return;
        }

        HandleData(_data);
      }
      catch
      {
        Disconnect();
      }
    }

    /// <summary>Prepares received data to be used by the appropriate packet handler methods.</summary>
    /// <param name="_data">The recieved data.</param>
    private void HandleData(byte[] _data)
    {
      using (Packet _packet = new Packet(_data))
      {
        int _packetLength = _packet.ReadInt();
        _data = _packet.ReadBytes(_packetLength);
      }

      ThreadManager.ExecuteOnMainThread(() =>
      {
        using (Packet _packet = new Packet(_data))
        {
          int _packetId = _packet.ReadInt();
          NetworkAction action = Actions.GetByID(_packetId);
          action.FromServer(_packet);
        }
      });
    }

    /// <summary>Disconnects from the server and cleans up the UDP connection.</summary>
    private void Disconnect()
    {
      client.Disconnect();
      isConnected = false;
      endPoint = null;
      socket = null;
    }
  }

}
