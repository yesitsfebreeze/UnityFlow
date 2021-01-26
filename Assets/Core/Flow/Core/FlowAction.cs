using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;
using System;

namespace Flow.Actions
{

  /// <summary>
  /// this is the base class for a Flow Action that gets executed either 
  /// on the server or the client
  /// </summary>
  public class FlowAction : MonoBehaviour
  {
    public int id;
    public bool isClient = false;
    public FlowSettings settings;
    public NetDataWriter writer;
    public NetPacketProcessor processor;

    /// <summary>
    /// Different send methods available in the rudp protocoll
    /// </summary>
    public enum SendMethod
    {
      ReliableUnordered = DeliveryMethod.ReliableUnordered,
      ReliableOrdered = DeliveryMethod.ReliableOrdered,
      ReliableSequenced = DeliveryMethod.ReliableSequenced,
      Sequenced = DeliveryMethod.Sequenced,
      Unreliable = DeliveryMethod.Unreliable,
    }

    /// <summary>
    /// used to subscribe to user defined packages
    /// </summary>
    public virtual void SubscribePackage()
    {
      // how to use
      ///
      // public override void SubscribePackage()
      // {
      //   processor.SubscribeReusable<SomePackageClass>(PrivateClassMethod);
      // }
    }

    /// <summary>
    /// Used to send a package over the client/server peer
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="package"></param>
    /// <returns></returns>
    public ActionSender SendPackage<T>(T package) where T : class, new()
    {
      writer.Reset();
      processor.Write(writer, package);
      return new ActionSender(writer, isClient);
    }

    /// <summary>
    /// Helper class to send the action via the correct method and to the correct peer
    /// </summary>
    public class ActionSender
    {
      private NetDataWriter writer;
      public static bool isClient = false;

      public ActionSender(NetDataWriter _writer, bool _isClient)
      {
        isClient = _isClient;
        writer = _writer;
      }


      /// <summary>
      /// Sends the package to a specific client
      /// </summary>
      /// <param name="method"></param>
      /// <param name="clientId"></param>
      public void Send(SendMethod method, int clientId = -999)
      {
        if (isClient)
        {
          FlowClient.peer.Send(writer, (DeliveryMethod)method);
        }
        else
        {
          if (clientId == -999) throw new ArgumentException("The clients id must be passed.");
          FlowServer.clients[clientId].peer.Send(writer, (DeliveryMethod)method);
        }
      }

      /// <summary>
      /// Sends the package to all connected peers
      /// </summary>
      /// <param name="method"></param>
      public void SendAll(SendMethod method)
      {
        if (isClient)
        {
          throw new NotSupportedException("SendAll is not supported on the client.");
        }
        FlowServer.IterateConnectedClients((FlowClientServer client) =>
        {
          client.peer.Send(writer, (DeliveryMethod)method);
          return true;
        });
      }

      /// <summary>
      /// Sends the package to all clients except the ones defined in clientIds
      /// </summary>
      /// <param name="method"></param>
      /// <param name="clientIds"></param>
      public void SendExcept(SendMethod method, int[] clientIds)
      {
        if (isClient)
        {
          throw new NotSupportedException("SendAllExcept is not supported on the client.");
        }
        FlowServer.IterateConnectedClients((FlowClientServer client) =>
        {
          if (!IntInArray(clientIds, client.id)) client.peer.Send(writer, (DeliveryMethod)method);
          return true;
        });
      }

      /// <summary>
      /// Sends the package to only the clients defined in clientIds
      /// </summary>
      /// <param name="method"></param>
      /// <param name="clientIds"></param>
      public void SendExclusively(SendMethod method, int[] clientIds)
      {
        if (isClient)
        {
          throw new NotSupportedException("SendMultiple is not supported on the client.");
        }
        FlowServer.IterateConnectedClients((FlowClientServer client) =>
        {
          if (IntInArray(clientIds, client.id)) client.peer.Send(writer, (DeliveryMethod)method);
          return true;
        });
      }

      /// <summary>
      /// returns if given array contains the given int
      /// </summary>
      /// <param name="list"></param>
      /// <param name="value"></param>
      /// <returns></returns>
      private static bool IntInArray(Array list, int value)
      {
        bool contains = false;
        foreach (int n in list) // go over every number in the list
        {
          if (n == value) // check if it matches
          {
            contains = true;
            break; // no need to check any further
          }
        }
        return contains;
      }
    }
  }
}