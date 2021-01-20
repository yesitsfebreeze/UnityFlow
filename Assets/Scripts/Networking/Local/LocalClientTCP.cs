
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

namespace Networking
{
  public class LocalClientTCP
  {
    public TcpClient socket;

    private NetworkStream stream;
    private Packet receivedPacket;
    private byte[] receiveBuffer;
    private SO_NetworkSettings settings;
    private LocalClient client;


    public LocalClientTCP()
    {
      client = LocalClient.instance;
      settings = LocalClient.settings;
    }


    /// <summary>Attempts to connect to the server via TCP.</summary>
    public void Connect()
    {
      socket = new TcpClient
      {
        ReceiveBufferSize = settings.DATA_BUFFER_SIZE,
        SendBufferSize = settings.DATA_BUFFER_SIZE
      };

      receiveBuffer = new byte[settings.DATA_BUFFER_SIZE];
      socket.BeginConnect(client.ip, client.port, ConnectCallback, socket);
    }

    /// <summary>Initializes the newly connected client's TCP-related info.</summary>
    private void ConnectCallback(IAsyncResult _result)
    {
      socket.EndConnect(_result);

      if (!socket.Connected) return;

      if (!client.udp.isConnected)
      {
        IPEndPoint enpoint = (IPEndPoint)socket.Client.LocalEndPoint;
        client.udp.Connect(enpoint.Port);
        return;
      }

      stream = socket.GetStream();

      receivedPacket = new Packet();

      stream.BeginRead(receiveBuffer, 0, settings.DATA_BUFFER_SIZE, ReceiveCallback, null);
    }

    /// <summary>Sends data to the client via TCP.</summary>
    /// <param name="_packet">The packet to send.</param>
    public void SendData(Packet _packet)
    {
      try
      {
        if (socket != null)
        {
          stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null); // Send data to server
        }
      }
      catch (Exception _ex)
      {
        Debug.Log($"Error sending data to server via TCP: {_ex}");
      }
    }

    /// <summary>Reads incoming data from the stream.</summary>
    private void ReceiveCallback(IAsyncResult _result)
    {
      try
      {
        int _byteLength = stream.EndRead(_result);
        if (_byteLength <= 0)
        {
          client.Disconnect();
          return;
        }

        byte[] _data = new byte[_byteLength];
        Array.Copy(receiveBuffer, _data, _byteLength);

        receivedPacket.Reset(HandleData(_data)); // Reset receivedPacket if all data was handled
        stream.BeginRead(receiveBuffer, 0, settings.DATA_BUFFER_SIZE, ReceiveCallback, null);
      }
      catch
      {
        Disconnect();
      }
    }

    /// <summary>Prepares received data to be used by the appropriate packet handler methods.</summary>
    /// <param name="_data">The recieved data.</param>
    private bool HandleData(byte[] _data)
    {
      // init the packet with the data
      receivedPacket.SetBytes(_data);

      int packetLength = 0;
      if (CheckPacketLength(ref packetLength)) return true;

      while (packetLength > 0 && packetLength <= receivedPacket.UnreadLength())
      {
        HandlePacket(packetLength);
        if (CheckPacketLength(ref packetLength)) return true;
      }

      if (packetLength <= 1)
      {
        return true; // Reset receivedPacket instance to allow it to be reused
      }

      return false;
    }


    private void HandlePacket(int packetLength)
    {
      // While packet contains data AND packet data length doesn't exceed the length of the packet we're reading
      byte[] _packetBytes = receivedPacket.ReadBytes(packetLength);
      ThreadManager.ExecuteOnMainThread(() =>
      {
        // get package from server
        using (Packet _packet = new Packet(_packetBytes))
        {
          int _packetId = _packet.ReadInt();
          NetworkAction action = Actions.GetByID(_packetId);
          action.FromServer(_packet);
        }
      });
    }

    private bool CheckPacketLength(ref int packetLength)
    {
      packetLength = 0;
      if (receivedPacket.UnreadLength() >= 4)
      {
        packetLength = receivedPacket.ReadInt();
        if (packetLength <= 0) return true;
      }
      return false;
    }

    /// <summary>Disconnects from the server and cleans up the TCP connection.</summary>
    private void Disconnect()
    {
      client.Disconnect();

      stream = null;
      receivedPacket = null;
      receiveBuffer = null;
      socket = null;
    }

  }
}