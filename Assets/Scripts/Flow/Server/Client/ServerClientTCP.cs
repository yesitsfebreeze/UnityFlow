using System;
using System.Net.Sockets;
using UnityEngine;
using FlowActions;

namespace Flow
{
  public class ServerClientTCP
  {
    public TcpClient socket;
    public bool isConnected = false;

    private readonly int id;
    private NetworkStream stream;
    private FlowPackage receivedPackage;
    private byte[] receiveBuffer;

    public ServerClientTCP(int clientID)
    {
      id = clientID;
    }

    /// <summary>Initializes the newly connected client's TCP-related info.</summary>
    /// <param name="socket">The TcpClient instance of the newly connected client.</param>
    public void Connect(TcpClient tcpSocket)
    {
      try
      {
        socket = tcpSocket;
        socket.ReceiveBufferSize = Server.settings.DATA_BUFFER_SIZE;
        socket.SendBufferSize = Server.settings.DATA_BUFFER_SIZE;

        stream = socket.GetStream();

        receivedPackage = new FlowPackage();
        receiveBuffer = new byte[Server.settings.DATA_BUFFER_SIZE];

        stream.BeginRead(receiveBuffer, 0, Server.settings.DATA_BUFFER_SIZE, ReceiveCallback, null);

        ConnectFlow action = Actions.Get("Connect") as ConnectFlow;
        action.ToClient(id, "Successfully connected to the server");
        isConnected = true;
      }
      catch (Exception ex)
      {
        Debug.Log($"Error connecting the player {id} via TCP: {ex}");
      }
    }

    /// <summary>Sends data to the client via TCP.</summary>
    /// <param name="package">The package to send.</param>
    public void SendData(FlowPackage package)
    {
      try
      {
        if (socket == null) return;
        stream.BeginWrite(package.ToArray(), 0, package.Length(), null, null);
      }
      catch (Exception ex)
      {
        Debug.Log($"Error sending data to player {id} via TCP: {ex}");
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
          Server.clients[id].Disconnect();
          return;
        }

        byte[] data = new byte[byteLength];
        Array.Copy(receiveBuffer, data, byteLength);

        receivedPackage.Reset(HandleData(data)); // Reset receivedPackage if all data was handled
        stream.BeginRead(receiveBuffer, 0, Server.settings.DATA_BUFFER_SIZE, ReceiveCallback, null);
      }
      catch (Exception ex)
      {
        Debug.Log($"Error receiving TCP data: {ex}");
        Server.clients[id].Disconnect();
      }
    }

    /// <summary>Prepares received data to be used by the appropriate package handler methods.</summary>
    /// <param name="data">The recieved data.</param>
    private bool HandleData(byte[] data)
    {
      receivedPackage.SetBytes(data);

      int packageLength = 0;
      if (CheckPackageLength(ref packageLength)) return true;

      while (packageLength > 0 && packageLength <= receivedPackage.UnreadLength())
      {
        HandlePackage(packageLength);
        if (CheckPackageLength(ref packageLength)) return true;
      }

      if (packageLength <= 1) return true;

      return false;
    }


    private void HandlePackage(int packageLength)
    {
      // While package contains data AND package data length doesn't exceed the length of the package we're reading
      byte[] packageBytes = receivedPackage.ReadBytes(packageLength);
      ThreadManager.ExecuteOnMainThread(() =>
      {
        // get package from server
        using (FlowPackage package = new FlowPackage(packageBytes))
        {
          int packageId = package.ReadInt();
          FlowAction action = Actions.GetByID(packageId);
          action.FromClient(id, package);
        }
      });
    }

    private bool CheckPackageLength(ref int packageLength)
    {
      packageLength = 0;
      if (receivedPackage.UnreadLength() >= 4)
      {
        packageLength = receivedPackage.ReadInt();
        if (packageLength <= 0) return true;
      }
      return false;
    }

    /// <summary>Closes and cleans up the TCP connection.</summary>
    public void Disconnect()
    {
      socket.Close();
      stream = null;
      receivedPackage = null;
      receiveBuffer = null;
      socket = null;
    }
  }
}
