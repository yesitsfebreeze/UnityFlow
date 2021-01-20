
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
    private Package receivedPacket;
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
    private void ConnectCallback(IAsyncResult result)
    {
      socket.EndConnect(result);

      if (!socket.Connected) return;

      if (!client.udp.isConnected)
      {
        IPEndPoint enpoint = (IPEndPoint)socket.Client.LocalEndPoint;
        client.udp.Connect(enpoint.Port);
        return;
      }

      stream = socket.GetStream();

      receivedPacket = new Package();

      stream.BeginRead(receiveBuffer, 0, settings.DATA_BUFFER_SIZE, ReceiveCallback, null);
    }

    /// <summary>Sends data to the client via TCP.</summary>
    /// <param name="package">The package to send.</param>
    public void SendData(Package package)
    {
      try
      {
        if (socket != null)
        {
          stream.BeginWrite(package.ToArray(), 0, package.Length(), null, null); // Send data to server
        }
      }
      catch (Exception ex)
      {
        Debug.Log($"Error sending data to server via TCP: {ex}");
      }
    }

    /// <summary>Reads incoming data from the stream.</summary>
    private void ReceiveCallback(IAsyncResult result)
    {
      try
      {
        int byteLength = stream.EndRead(result);
        if (byteLength <= 0)
        {
          client.Disconnect();
          return;
        }

        byte[] data = new byte[byteLength];
        Array.Copy(receiveBuffer, data, byteLength);

        receivedPacket.Reset(HandleData(data)); // Reset receivedPacket if all data was handled
        stream.BeginRead(receiveBuffer, 0, settings.DATA_BUFFER_SIZE, ReceiveCallback, null);
      }
      catch
      {
        Disconnect();
      }
    }

    /// <summary>Prepares received data to be used by the appropriate package handler methods.</summary>
    /// <param name="data">The recieved data.</param>
    private bool HandleData(byte[] data)
    {
      // init the package with the data
      receivedPacket.SetBytes(data);

      int packageLength = 0;
      if (CheckPacketLength(ref packageLength)) return true;

      while (packageLength > 0 && packageLength <= receivedPacket.UnreadLength())
      {
        HandlePacket(packageLength);
        if (CheckPacketLength(ref packageLength)) return true;
      }

      if (packageLength <= 1) return true;

      return false;
    }


    private void HandlePacket(int packageLength)
    {
      // While package contains data AND package data length doesn't exceed the length of the package we're reading
      byte[] packageBytes = receivedPacket.ReadBytes(packageLength);
      ThreadManager.ExecuteOnMainThread(() =>
      {
        // get package from server
        using (Package package = new Package(packageBytes))
        {
          int packageId = package.ReadInt();
          NetworkAction action = Actions.GetByID(packageId);
          action.FromServer(package);
        }
      });
    }

    private bool CheckPacketLength(ref int packageLength)
    {
      packageLength = 0;
      if (receivedPacket.UnreadLength() >= 4)
      {
        packageLength = receivedPacket.ReadInt();
        if (packageLength <= 0) return true;
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