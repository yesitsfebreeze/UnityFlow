using System.Net;

namespace Flow
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
    public void SendData(FlowPackage package)
    {
      ServerUDP.SendData(endPoint, package);
    }

    /// <summary>Prepares received data to be used by the appropriate package handler methods.</summary>
    /// <param name="packageData">The package containing the recieved data.</param>
    public void HandleData(FlowPackage packageData)
    {
      int packageLength = packageData.ReadInt();
      byte[] packageBytes = packageData.ReadBytes(packageLength);

      ThreadManager.ExecuteOnMainThread(() =>
      {

        // Call appropriate method to handle the package
        using (FlowPackage package = new FlowPackage(packageBytes))
        {
          int packageId = package.ReadInt();
          FlowAction action = FlowActions.GetByID(packageId);
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