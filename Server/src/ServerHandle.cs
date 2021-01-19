using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
  class ServerHandle
  {
    public static void WelcomeReceived(int _fromClient, Packet _packet)
    {
      int _clientIdCheck = _packet.ReadInt();
      string _username = _packet.ReadString();

      Console.WriteLine($"{_username} connected.");
      if (_fromClient != _clientIdCheck)
      {
        Console.WriteLine($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
      }
      // TODO: send player into game
    }
  }
}
