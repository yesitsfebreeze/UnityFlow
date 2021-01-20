using System;
using System.Net.Sockets;
using UnityEngine;

namespace Networking
{
  public class ServerClientTCP
  {
    public TcpClient socket;
    public bool isConnected = false;

    private readonly int id;
    private NetworkStream stream;
    private Packet receivedData;
    private byte[] receiveBuffer;

    public ServerClientTCP(int _id)
    {
      id = _id;
    }

    /// <summary>Initializes the newly connected client's TCP-related info.</summary>
    /// <param name="_socket">The TcpClient instance of the newly connected client.</param>
    public void Connect(TcpClient _socket)
    {
      try
      {
        socket = _socket;
        socket.ReceiveBufferSize = Server.settings.DATA_BUFFER_SIZE;
        socket.SendBufferSize = Server.settings.DATA_BUFFER_SIZE;

        stream = socket.GetStream();

        receivedData = new Packet();
        receiveBuffer = new byte[Server.settings.DATA_BUFFER_SIZE];

        stream.BeginRead(receiveBuffer, 0, Server.settings.DATA_BUFFER_SIZE, ReceiveCallback, null);



        ConnectAction action = Actions.Get("Connect") as ConnectAction;
        action.ToClient(id, "Successfully connected to the server");
        isConnected = true;
      }
      catch (Exception ex)
      {
        Debug.Log($"Error connecting the player {id} via TCP: {ex}");
      }
    }

    /// <summary>Sends data to the client via TCP.</summary>
    /// <param name="_packet">The packet to send.</param>
    public void SendData(Packet _packet)
    {
      try
      {
        if (socket != null)
        {
          stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null); // Send data to appropriate client
        }
      }
      catch (Exception ex)
      {
        Debug.Log($"Error sending data to player {id} via TCP: {ex}");
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
          Server.clients[id].Disconnect();
          return;
        }

        byte[] _data = new byte[_byteLength];
        Array.Copy(receiveBuffer, _data, _byteLength);

        receivedData.Reset(HandleData(_data)); // Reset receivedData if all data was handled
        stream.BeginRead(receiveBuffer, 0, Server.settings.DATA_BUFFER_SIZE, ReceiveCallback, null);
      }
      catch (Exception ex)
      {
        Debug.Log($"Error receiving TCP data: {ex}");
        Server.clients[id].Disconnect();
      }
    }

    /// <summary>Prepares received data to be used by the appropriate packet handler methods.</summary>
    /// <param name="_data">The recieved data.</param>
    private bool HandleData(byte[] _data)
    {
      int _packetLength = 0;

      receivedData.SetBytes(_data);

      if (receivedData.UnreadLength() >= 4)
      {
        // If client's received data contains a packet
        _packetLength = receivedData.ReadInt();
        if (_packetLength <= 0)
        {
          // If packet contains no data
          return true; // Reset receivedData instance to allow it to be reused
        }
      }

      while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
      {
        // While packet contains data AND packet data length doesn't exceed the length of the packet we're reading
        byte[] _packetBytes = receivedData.ReadBytes(_packetLength);
        ThreadManager.ExecuteOnMainThread(() =>
        {
          using (Packet _packet = new Packet(_packetBytes))
          {
            // Call appropriate method to handle the packet
            int _packetId = _packet.ReadInt();
            NetworkAction action = Actions.GetByID(_packetId);
            action.FromClient(id, _packet);
          }
        });

        _packetLength = 0; // Reset packet length
        if (receivedData.UnreadLength() >= 4)
        {
          // If client's received data contains another packet
          _packetLength = receivedData.ReadInt();
          if (_packetLength <= 0)
          {
            // If packet contains no data
            return true; // Reset receivedData instance to allow it to be reused
          }
        }
      }

      if (_packetLength <= 1)
      {
        return true; // Reset receivedData instance to allow it to be reused
      }

      return false;
    }

    /// <summary>Closes and cleans up the TCP connection.</summary>
    public void Disconnect()
    {
      socket.Close();
      stream = null;
      receivedData = null;
      receiveBuffer = null;
      socket = null;
    }
  }
}
