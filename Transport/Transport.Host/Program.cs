using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace Transport {
  class TestPeer {
    public const ushort SERVER_PORT = 25000;

    public static IPEndPoint ServerEndPoint => new IPEndPoint(IPAddress.Loopback, SERVER_PORT);

    bool _server;
    
    public Peer Peer;

    public TestPeer(bool server) {
      _server = server;
      Peer    = new Peer(GetConfig(server));

      if (_server == false) {
        Peer.Connect(ServerEndPoint);
      }
    }

    public static Config GetConfig(bool server) {
      Config config;
      config = new Config();

      if (server) {
        config.EndPoint = ServerEndPoint;
      } else {
        config.EndPoint = new IPEndPoint(IPAddress.Any, 0);
      }

      return config;
    }

    public void Update() {
      Peer.Update();
    }
  }

  static class Program {
    static List<TestPeer> Peers = new List<TestPeer>();

    static void Main(string[] args) {
      Log.InitForConsole();
      
      Peers.Add(new TestPeer(true));
      Peers.Add(new TestPeer(false));

      while (true) {
        for (int i = 0; i < Peers.Count; ++i) {
          Peers[i].Update();
        }

        Thread.Sleep(15);
      }

      Console.ReadLine();
    }
  }
}