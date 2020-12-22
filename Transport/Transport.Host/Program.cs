using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace Transport {
  class TestPeer {
    public const ushort SERVER_PORT = 25005;

    public static IPEndPoint ServerEndPoint => new IPEndPoint(IPAddress.Loopback, SERVER_PORT);

    bool _server;

    public Peer Peer;

    public bool IsServer => _server;
    public bool IsClient => _server == false;
    
    public TestPeer(bool server) {
      _server = server;
      Peer    = new Peer(GetConfig(server));
      Peer.OnConnected += PeerOnConnected;
      Peer.OnUnreliablePacket += PeerOnUnreliablePacket;

      if (IsClient) {
        Peer.Connect(ServerEndPoint);
      }
    }

    void PeerOnUnreliablePacket(Connection connection, Packet packet) {
      Log.Trace($"Got Data: {BitConverter.ToUInt32(packet.Data, packet.Offset)}"); 
    }

    void PeerOnConnected(Connection connection) {
      if (IsClient) {
        Peer.SendNotify(connection, BitConverter.GetBytes(uint.MaxValue), null);
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
      Peer?.Update();
    }
  }

  static class Program {
    static List<TestPeer> Peers = new List<TestPeer>();

    static void Main(string[] args) {
      Log.InitForConsole();

      //Sequencer sequencer = new Sequencer(1);
      // ... 254 255 0 1 2 3 ... 
      //Log.Info(sequencer.Distance(255, 0));
      
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