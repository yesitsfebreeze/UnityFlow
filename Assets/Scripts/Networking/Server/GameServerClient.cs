using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using Networking;


namespace Networking
{

  public class GameServerClient
  {
    public static SO_NetworkSettings settings;

    public int id;
    // public Player player;
    public TCP tcp;
    public UDP udp;

    public GameServerClient(int _clientId, SO_NetworkSettings networkSettings)
    {
      settings = networkSettings;
      id = _clientId;
      tcp = new TCP(id);
      udp = new UDP(id);
    }

    public class TCP
    {
      public TcpClient socket;

      private readonly int id;
      private NetworkStream stream;
      private Packet receivedData;
      private byte[] receiveBuffer;

      public TCP(int _id)
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
          socket.ReceiveBufferSize = settings.DATA_BUFFER_SIZE;
          socket.SendBufferSize = settings.DATA_BUFFER_SIZE;

          stream = socket.GetStream();

          receivedData = new Packet();
          receiveBuffer = new byte[settings.DATA_BUFFER_SIZE];

          stream.BeginRead(receiveBuffer, 0, settings.DATA_BUFFER_SIZE, ReceiveCallback, null);

          ConnectAction action = Actions.Get("Connect") as ConnectAction;
          action.ToClient(id, "Successfully connected to the server");
        }
        catch (Exception _ex)
        {
          Debug.Log($"Error connecting the player {id} via TCP: {_ex}");
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
        catch (Exception _ex)
        {
          Debug.Log($"Error sending data to player {id} via TCP: {_ex}");
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
            GameServer.clients[id].Disconnect();
            return;
          }

          byte[] _data = new byte[_byteLength];
          Array.Copy(receiveBuffer, _data, _byteLength);

          receivedData.Reset(HandleData(_data)); // Reset receivedData if all data was handled
          stream.BeginRead(receiveBuffer, 0, settings.DATA_BUFFER_SIZE, ReceiveCallback, null);
        }
        catch (Exception _ex)
        {
          Debug.Log($"Error receiving TCP data: {_ex}");
          GameServer.clients[id].Disconnect();
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

    public class UDP
    {
      public IPEndPoint endPoint;

      private int id;

      public UDP(int _id)
      {
        id = _id;
      }

      /// <summary>Initializes the newly connected client's UDP-related info.</summary>
      /// <param name="_endPoint">The IPEndPoint instance of the newly connected client.</param>
      public void Connect(IPEndPoint _endPoint)
      {
        endPoint = _endPoint;
      }

      /// <summary>Sends data to the client via UDP.</summary>
      /// <param name="_packet">The packet to send.</param>
      public void SendData(Packet _packet)
      {
        GameServer.SendUDPData(endPoint, _packet);
      }

      /// <summary>Prepares received data to be used by the appropriate packet handler methods.</summary>
      /// <param name="_packetData">The packet containing the recieved data.</param>
      public void HandleData(Packet _packetData)
      {
        int _packetLength = _packetData.ReadInt();
        byte[] _packetBytes = _packetData.ReadBytes(_packetLength);

        ThreadManager.ExecuteOnMainThread(() =>
        {

          // Call appropriate method to handle the packet
          using (Packet _packet = new Packet(_packetBytes))
          {
            int _packetId = _packet.ReadInt();
            NetworkAction action = Actions.GetByID(_packetId);
            action.FromClient(id, _packet);
          }
        });
      }

      /// <summary>Cleans up the UDP connection.</summary>
      public void Disconnect()
      {
        endPoint = null;
      }
    }

    // /// <summary>Sends the client into the game and informs other clients of the new player.</summary>
    // /// <param name="_playerName">The username of the new player.</param>
    // public void SendIntoGame(string _playerName)
    // {
    //   player = NetworkManager.instance.InstantiatePlayer();
    //   player.Initialize(id, _playerName);

    //   // Send all players to the new player
    //   foreach (Client _client in GameServer.clients.Values)
    //   {
    //     if (_client.player != null)
    //     {
    //       if (_client.id != id)
    //       {
    //         ServerSend.SpawnPlayer(id, _client.player);
    //       }
    //     }
    //   }

    //   // Send the new player to all players (including himself)
    //   foreach (Client _client in GameServer.clients.Values)
    //   {
    //     if (_client.player != null)
    //     {
    //       ServerSend.SpawnPlayer(_client.id, player);
    //     }
    //   }

    //   foreach (ItemSpawner _itemSpawner in ItemSpawner.spawners.Values)
    //   {
    //     ServerSend.CreateItemSpawner(id, _itemSpawner.spawnerId, _itemSpawner.transform.position, _itemSpawner.hasItem);
    //   }

    //   foreach (Enemy _enemy in Enemy.enemies.Values)
    //   {
    //     ServerSend.SpawnEnemy(id, _enemy);
    //   }
    // }

    /// <summary>Disconnects the client and stops all network traffic.</summary>
    private void Disconnect()
    {
      Debug.Log($"Player {id} has disconnected.");

      // ThreadManager.ExecuteOnMainThread(() =>
      // {
      //   // UnityEngine.Object.Destroy(player.gameObject);
      //   // player = null;
      // });

      tcp.Disconnect();
      udp.Disconnect();

      // ServerSend.PlayerDisconnected(id);
    }
  }
}