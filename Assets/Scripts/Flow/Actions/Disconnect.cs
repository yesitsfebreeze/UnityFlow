using UnityEngine;
using Flow;

namespace Flow
{
  public class DisconnectFlowAction : FlowAction
  {

    override public void FromClient(int clientID, FlowPackage package)
    {
    }

    public void ToServer(int clientID, string username)
    {
    }


    public void ToClient(int clientID, string msg)
    {
    }

    override public void FromServer(FlowPackage package)
    {
    }
  }
}


