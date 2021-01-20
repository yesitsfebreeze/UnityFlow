using UnityEngine;

namespace Flow
{

  public class LocalClient : MonoBehaviour
  {
    public static LocalClient instance;
    public FlowSettings FlowSettings;
    public static FlowSettings settings;
    public bool wasConnected = false;

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
      settings = FlowSettings;
      ThreadManager threadManager = gameObject.AddComponent<ThreadManager>();
      threadManager.isClient = true;
      FlowActions actions = gameObject.AddComponent<FlowActions>();
      actions.FlowSettings = settings;

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
      tcp.Connect();
      tcp.SetOnConnectedCallback((int Port) =>
      {
        if (!udp.isConnected) udp.Connect(Port);
      });
    }

    /// <summary>Disconnects from the server and stops all network traffic.</summary>
    public void Disconnect()
    {
      if (isConnected)
      {
        isConnected = false;
        wasConnected = false;
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

    public bool IsConnected()
    {
      Debug.Log("udp:" + udp.isConnected);
      Debug.Log("tcp:" + tcp.isConnected);

      if (udp.isConnected && tcp.isConnected)
      {
        wasConnected = true;
        isConnected = true;
      }

      isConnected = false;
      return isConnected;
    }


    #region DataSending
    public static void Send(Protocol protocol, FlowPackage package)
    {
      package.WriteLength();
      if (protocol == Protocol.TCP) instance.tcp.SendData(package);
      if (protocol == Protocol.UDP) instance.udp.SendData(package);
    }

    /// <summary>Sends a package to the server via TCP.</summary>
    /// <param name="package">The package to send to the sever.</param>
    public static void TCPSend(FlowPackage package)
    {
      Send(Protocol.TCP, package);
    }

    /// <summary>Sends a package to the server via UDP.</summary>
    /// <param name="package">The package to send to the sever.</param>
    public static void UDPSend(FlowPackage package)
    {
      Send(Protocol.UDP, package);
    }
    #endregion

  }
}
