using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

namespace Networking
{
  public class LocalClient : MonoBehaviour
  {
    public static LocalClient instance;
    public SO_NetworkSettings NetworkSettings;
    public static SO_NetworkSettings settings;

    public int localClientID = 0;
    public string ip;
    public int port;
    public TCP tcp;
    public UDP udp;

    private bool isConnected = false;

    private void Awake()
    {
      if (instance == null)
      {
        instance = this;
      }
      else if (instance != this)
      {
        Debug.Log("Instance already exists, destroying object!");
        Destroy(this);
      }
    }

    private void Start()
    {
      settings = NetworkSettings;

      if (settings.DEVELOPMENT_MODE)
      {
        Screen.SetResolution(640, 480, false);
      }

      ip = settings.IP;
      port = settings.PORT;

      ConnectToServer();
    }

    private void OnApplicationQuit()
    {
      Disconnect(); // Disconnect when the game is closed
    }

    /// <summary>Attempts to connect to the server.</summary>
    public void ConnectToServer()
    {
      tcp = new TCP();
      udp = new UDP();

      isConnected = true;
      tcp.Connect(); // Connect tcp, udp gets connected once tcp is done
    }

    public class TCP
    {
      public TcpClient socket;

      private NetworkStream stream;
      private Packet receivedData;
      private byte[] receiveBuffer;

      /// <summary>Attempts to connect to the server via TCP.</summary>
      public void Connect()
      {
        socket = new TcpClient
        {
          ReceiveBufferSize = settings.DATA_BUFFER_SIZE,
          SendBufferSize = settings.DATA_BUFFER_SIZE
        };

        receiveBuffer = new byte[settings.DATA_BUFFER_SIZE];
        socket.BeginConnect(instance.ip, instance.port, ConnectCallback, socket);
      }

      /// <summary>Initializes the newly connected client's TCP-related info.</summary>
      private void ConnectCallback(IAsyncResult _result)
      {
        socket.EndConnect(_result);

        if (!socket.Connected)
        {
          return;
        }

        stream = socket.GetStream();

        receivedData = new Packet();

        stream.BeginRead(receiveBuffer, 0, settings.DATA_BUFFER_SIZE, ReceiveCallback, null);

        IPEndPoint enpoint = (IPEndPoint)LocalClient.instance.tcp.socket.Client.LocalEndPoint;
        LocalClient.instance.udp.Connect(enpoint.Port);
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
            instance.Disconnect();
            return;
          }

          byte[] _data = new byte[_byteLength];
          Array.Copy(receiveBuffer, _data, _byteLength);

          receivedData.Reset(HandleData(_data)); // Reset receivedData if all data was handled
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
              action.FromServer(_packet);
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

      /// <summary>Disconnects from the server and cleans up the TCP connection.</summary>
      private void Disconnect()
      {
        instance.Disconnect();

        stream = null;
        receivedData = null;
        receiveBuffer = null;
        socket = null;
      }
    }

    public class UDP
    {
      public UdpClient socket;
      public IPEndPoint endPoint;

      public UDP()
      {
        endPoint = new IPEndPoint(IPAddress.Parse(instance.ip), instance.port);
      }

      /// <summary>Attempts to connect to the server via UDP.</summary>
      /// <param name="_localPort">The port number to bind the UDP socket to.</param>
      public void Connect(int _localPort)
      {
        socket = new UdpClient(_localPort);

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
          _packet.InsertInt(instance.localClientID); // Insert the client's ID at the start of the packet
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
            instance.Disconnect();
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
            // Call appropriate method to handle the packet
            int _packetId = _packet.ReadInt();
            NetworkAction action = Actions.GetByID(_packetId);
            action.FromServer(_packet);
          }
        });
      }

      /// <summary>Disconnects from the server and cleans up the UDP connection.</summary>
      private void Disconnect()
      {
        instance.Disconnect();

        endPoint = null;
        socket = null;
      }
    }

    /// <summary>Disconnects from the server and stops all network traffic.</summary>
    private void Disconnect()
    {
      if (isConnected)
      {
        isConnected = false;

        if (tcp != null && tcp.socket != null) tcp.socket.Close();
        if (udp != null && udp.socket != null) udp.socket.Close();

        Debug.Log("Disconnected from server.");
      }
    }

    /// <summary>Sends a packet to the server via TCP.</summary>
    /// <param name="_packet">The packet to send to the sever.</param>
    public static void SendTCPData(Packet _packet)
    {
      _packet.WriteLength();
      LocalClient.instance.tcp.SendData(_packet);
    }

    /// <summary>Sends a packet to the server via UDP.</summary>
    /// <param name="_packet">The packet to send to the sever.</param>
    public static void SendUDPData(Packet _packet)
    {
      _packet.WriteLength();
      LocalClient.instance.udp.SendData(_packet);
    }
  }
}
