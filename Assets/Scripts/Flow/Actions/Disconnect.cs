using UnityEngine;

namespace Flow
{
  namespace ClientSide
  {
    public class Disconnect : FlowAction
    {


      override public void In(FlowPackage package)
      {
      }

      public void Out(int clientID, string username)
      {
      }
    }
  }

  namespace ServerSide
  {
    public class Disconnect : FlowAction
    {

      override public void In(FlowPackage package, int clientID)
      {
      }

      public void Out(int clientID, string msg)
      {
      }
    }
  }
}


