using UnityEngine;

namespace Networking
{

  public class LocalClient : MonoBehaviour
  {
    public static LocalClient instance;
    public SO_NetworkSettings NetworkSettings;
    public static SO_NetworkSettings settings;

    public int id = 0;
    public string ip;
    public int port;
    public LocalClientTCP tcp;
    public LocalClientUDP udp;

    public enum Protocol
    {
      TCP,
      UDP
    }

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

      Connect();
    }

    private void OnApplicationQuit()
    {
      Disconnect(); // Disconnect when the game is closed
    }

    /// <summary>Attempts to connect to the server.</summary>
    public void Connect()
    {
      tcp = new LocalClientTCP();
      udp = new LocalClientUDP();

      isConnected = true;
      tcp.Connect(); // Connect tcp, udp gets connected once tcp is done
    }

    /// <summary>Disconnects from the server and stops all network traffic.</summary>
    public void Disconnect()
    {
      if (isConnected)
      {
        isConnected = false;
        bool wasConnected = false;
        if (tcp != null && tcp.socket != null)
        {
          wasConnected = true;
          tcp.socket.Close();
        }

        if (udp != null && udp.socket != null)
        {
          wasConnected = true;
          udp.socket.Close();
        }

        if (!wasConnected) return;
        Debug.Log("Disconnected from server.");
      }
    }


    #region DataSending
    public static void Send(Protocol protocol, Package package)
    {
      package.WriteLength();
      if (protocol == Protocol.TCP) instance.tcp.SendData(package);
      if (protocol == Protocol.UDP) instance.udp.SendData(package);
    }

    /// <summary>Sends a package to the server via TCP.</summary>
    /// <param name="package">The package to send to the sever.</param>
    public static void SendTCPData(Package package)
    {
      Send(Protocol.TCP, package);
    }

    /// <summary>Sends a package to the server via UDP.</summary>
    /// <param name="package">The package to send to the sever.</param>
    public static void SendUDPData(Package package)
    {
      Send(Protocol.UDP, package);
    }
    #endregion

  }
}
