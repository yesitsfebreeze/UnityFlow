using LiteNetLib;
using System.Text;
using System.Security.Cryptography;

namespace Flow {
  static class Flow {
    public static bool isServer = false;
    public static bool isClient = false;

    public static string CreateClientId(NetPeer peer) {
      string clientId = $"{ peer.EndPoint.Address}:{ peer.EndPoint.Port}";
      MD5 md5 = new MD5CryptoServiceProvider();
      byte[] value = Encoding.Default.GetBytes(clientId);
      byte[] result = md5.ComputeHash(value);
      string hash = System.BitConverter.ToString(result);
      hash = hash.Replace("-", "");

      return hash.ToLower();
    }
  }
}