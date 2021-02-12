using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Reflection;

namespace Flow.Actions {


  public class FlowPackage {
    public string clientId { get; set; }
  }


  /// <summary>
  /// this is the base class for a Flow Action that gets executed either 
  /// on the server or the client
  /// </summary>
  public class FlowAction : MonoBehaviour {
    public int id;
    public bool isClient = false;
    public Type package;
    public FlowSettings settings;
    public NetDataWriter writer;
    public NetPacketProcessor processor;

    /// <summary>
    /// Different send methods available in the rudp protocoll
    /// </summary>
    public enum SendMethod {
      ReliableUnordered = DeliveryMethod.ReliableUnordered,
      ReliableOrdered = DeliveryMethod.ReliableOrdered,
      ReliableSequenced = DeliveryMethod.ReliableSequenced,
      Sequenced = DeliveryMethod.Sequenced,
      Unreliable = DeliveryMethod.Unreliable,
    }

    public void SetPackageType<FlowPackage>() {
      package = typeof(FlowPackage);
    }

    public void SubscribePackage() {
      string className = this.GetType().Name;
      string packageName;
      if (className.Contains("ServerAction")) {
        packageName = className.Replace("ServerAction", "ClientPackage");
      } else {
        packageName = className.Replace("ClientAction", "ServerPackage");
      }

      Type package = Type.GetType($"Flow.Actions.{packageName}");
      if (package == null) return;



      MethodInfo mi = GetType().GetMethod("SubscribeReusable");
      if (mi == null) return;

      Type[] types = new Type[1];
      types[0] = package;

      MethodInfo method = mi.MakeGenericMethod(types);
      Action<object, NetPeer> action = HandlePackage;
      method.Invoke(this, new object[] { action });
    }

    public void SubscribeReusable<T>(Action<object, NetPeer> onReceive) where T : class, new() {
      processor.SubscribeReusable<T, NetPeer>(onReceive);
    }

    /// <summary>
    /// used to subscribe to user defined packages
    /// </summary>
    public void HandlePackage(object package, NetPeer peer) {
      MethodInfo mi = GetType().GetMethod("Handle");
      if (mi == null) return;

      PropertyInfo[] fi = package.GetType().GetProperties();
      foreach (PropertyInfo field in fi) {
        if (field.Name == "clientId") {
          if ((string)field.GetValue(package) == "") {
            field.SetValue(package, Flow.CreateClientId(peer));
          }

        }
      }

      mi.Invoke(this, new object[] { package });
    }

    /// <summary>
    /// Used to send a package over the client/server peer
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="package"></param>
    /// <returns></returns>
    public ActionSender SendPackage<T>(T package) where T : class, new() {
      writer.Reset();

      processor.Write(writer, package);
      return new ActionSender(writer, isClient);
    }

    /// <summary>
    /// Used to send a package over the client/server peer
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="package"></param>
    /// <returns></returns>
    public ActionSender SendPackageFrom<T>(string clientId, T package) where T : class, new() {
      PropertyInfo[] fi = package.GetType().GetProperties();
      foreach (PropertyInfo field in fi) {
        if (field.Name == "clientId") {
          field.SetValue(package, clientId);
        }
      }

      return SendPackage(package);
    }

    /// <summary>
    /// Helper class to send the action via the correct method and to the correct peer
    /// </summary>
    public class ActionSender {
      private NetDataWriter writer;
      public static bool isClient = false;

      public ActionSender(NetDataWriter _writer, bool _isClient) {
        isClient = _isClient;
        writer = _writer;
      }


      /// <summary>
      /// Sends the package to a specific client
      /// </summary>
      /// <param name="method"></param>
      /// <param name="clientId"></param>
      public void Send(SendMethod method, string clientId = "") {
        if (isClient) {
          FlowClient.peer.Send(writer, (DeliveryMethod)method);
        } else {
          if (clientId == "") throw new ArgumentException("The clients id must be passed.");
          FlowServer.clients[clientId].peer.Send(writer, (DeliveryMethod)method);
        }
      }

      /// <summary>
      /// Sends the package to all connected peers
      /// </summary>
      /// <param name="method"></param>
      public void SendAll(SendMethod method) {
        if (isClient) {
          throw new NotSupportedException("SendAll is not supported on the client.");
        }
        FlowServer.IterateConnectedClients((FlowClientServer client) => {
          client.peer.Send(writer, (DeliveryMethod)method);
          return true;
        });
      }

      /// <summary>
      /// Sends the package to all clients except the ones defined in clientIds
      /// </summary>
      /// <param name="method"></param>
      /// <param name="clientIds"></param>
      public void SendExcept(SendMethod method, string[] clientIds) {
        if (isClient) {
          throw new NotSupportedException("SendAllExcept is not supported on the client.");
        }
        FlowServer.IterateConnectedClients((FlowClientServer client) => {
          if (!StringInArray(clientIds, client.id)) client.peer.Send(writer, (DeliveryMethod)method);
          return true;
        });
      }

      /// <summary>
      /// Sends the package to only the clients defined in clientIds
      /// </summary>
      /// <param name="method"></param>
      /// <param name="clientIds"></param>
      public void SendExclusively(SendMethod method, string[] clientIds) {
        if (isClient) {
          throw new NotSupportedException("SendMultiple is not supported on the client.");
        }
        FlowServer.IterateConnectedClients((FlowClientServer client) => {
          if (StringInArray(clientIds, client.id)) client.peer.Send(writer, (DeliveryMethod)method);
          return true;
        });
      }

      /// <summary>
      /// returns if given array contains the given int
      /// </summary>
      /// <param name="list"></param>
      /// <param name="value"></param>
      /// <returns></returns>
      private static bool StringInArray(Array list, string value) {
        bool contains = false;
        foreach (string n in list) {
          if (n == value) {
            contains = true;
            break;
          }
        }
        return contains;
      }
    }
  }
}