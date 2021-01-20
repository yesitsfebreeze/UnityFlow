using System.Net;

namespace Networking
{
  public class ServerClientUDP
  {
    public bool isConnected = false;
    public IPEndPoint endPoint;

    private int id;

    public ServerClientUDP(int clientID)
    {
      id = clientID;
    }

    /// <summary>Initializes the newly connected client's UDP-related info.</summary>
    /// <param name="endPoint">The IPEndPoint instance of the newly connected client.</param>
    public void Connect(IPEndPoint udpEndPoint)
    {
      isConnected = true;
      endPoint = udpEndPoint;
    }

    /// <summary>Sends data to the client via UDP.</summary>
    /// <param name="package">The package to send.</param>
    public void SendData(Package package)
    {
      ServerUDP.SendData(endPoint, package);
    }

    /// <summary>Prepares received data to be used by the appropriate package handler methods.</summary>
    /// <param name="packageData">The package containing the recieved data.</param>
    public void HandleData(Package packageData)
    {
      int packageLength = packageData.ReadInt();
      byte[] packageBytes = packageData.ReadBytes(packageLength);

      ThreadManager.ExecuteOnMainThread(() =>
      {

        // Call appropriate method to handle the package
        using (Package package = new Package(packageBytes))
        {
          int packageId = package.ReadInt();
          NetworkAction action = Actions.GetByID(packageId);
          action.FromClient(id, package);
        }
      });
    }

    /// <summary>Cleans up the UDP connection.</summary>
    public void Disconnect()
    {
      endPoint = null;
      isConnected = false;
    }
  }
}