using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Transport {
  public class Peer {
    Timer  _clock;
    Socket _socket;
    Config _config;

    Dictionary<IPEndPoint, Connection> _connections;

    public Config Config => _config;

    public Peer(Config config) {
      _config = config;
      _clock  = Timer.StartNew();

      _connections = new Dictionary<IPEndPoint, Connection>();

      _socket          = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
      _socket.Blocking = false;
      _socket.Bind(config.EndPoint);

      SetConnReset(_socket);
    }

    public void Connect(IPEndPoint endpoint) {
      CreateConnection(endpoint).ChangeState(ConnectionState.Connecting);
    }

    public void Update() {
      // recv
      Recv();

      // process
      UpdateConnections();
    }

    public void SendUnconnected(byte[] data, EndPoint target) {
      _socket.SendTo(data, target);
    }

    void UpdateConnections() {
      foreach (var kvp in _connections) {
        UpdateConnection(kvp.Value);
      }
    }

    void UpdateConnection(Connection connection) {
      switch (connection.State) {
        case ConnectionState.Connecting:
          UpdateConnecting(connection);
          break;
      }
    }

    void UpdateConnecting(Connection connection) {
      if ((connection.ConnectionAttemptTime + _config.ConnectAttemptInterval) < _clock.ElapsedInSeconds) {
        if (connection.ConnectionAttempts == _config.MaxConnectAttempts) {
          Assert.AlwaysFail("connection failed handle this with a callback");
          return;
        }

        connection.ConnectionAttempts    += 1;
        connection.ConnectionAttemptTime =  _clock.ElapsedInSeconds;

        SendCommand(connection, Commands.ConnectRequest);
      }
    }

    byte[] GetMtuBuffer() {
      return new byte[_config.Mtu];
    }

    void Recv() {
      if (_socket.Poll(0, SelectMode.SelectRead) == false) {
        return;
      }

      var buffer        = GetMtuBuffer();
      var endpoint      = (EndPoint) new IPEndPoint(IPAddress.Any, 0);
      var bytesReceived = _socket.ReceiveFrom(buffer, SocketFlags.None, ref endpoint);
      if (bytesReceived > 0) {
        var packet = new Packet {
          Data   = buffer,
          Length = bytesReceived
        };

        Log.Trace($"received {bytesReceived} from {endpoint}");

        var ipEndpoint = (IPEndPoint) endpoint;

        if (_connections.TryGetValue(ipEndpoint, out var connection)) {
          switch ((PacketType) packet.Data[0]) {
            case PacketType.Command:
              HandleCommandPacket(connection, packet);
              break;
          }
        } else {
          HandleUnconnectedPacket(ipEndpoint, packet);
        }
      }
    }

    void HandleUnconnectedPacket(IPEndPoint endpoint, Packet packet) {
      // assume is garbage data from somewhere on the internet
      if (packet.Length != 2) {
        return;
      }

      // first packet has to be a command
      if (((PacketType) packet.Data[0]) != PacketType.Command) {
        return;
      }

      // first packet has to be a connect request command
      if (((Commands) packet.Data[1]) != Commands.ConnectRequest) {
        return;
      }

      if (_connections.Count >= _config.MaxConnections) {
        // TODO: Send "connection refused server is full packet" as a reply...
        return;
      }

      // 
      HandleCommandPacket(CreateConnection(endpoint), packet);
    }

    Connection CreateConnection(IPEndPoint endpoint) {
      Connection connection;
      connection = new Connection(endpoint);

      _connections.Add(endpoint, connection);

      Log.Trace($"created connection object for {endpoint}");

      return connection;
    }

    void HandleCommandPacket(Connection connection, Packet packet) {
      switch ((Commands) packet.Data[1]) {
        case Commands.ConnectRequest:
          HandleConnectRequest(connection, packet);
          break;

        case Commands.ConnectionAccepted:
          HandleConnectionAccepted(connection, packet);
          break;
      }
    }

    void HandleConnectRequest(Connection connection, Packet packet) {
      switch (connection.State) {
        case ConnectionState.Created:
          connection.ChangeState(ConnectionState.Connected);
          SendCommand(connection, Commands.ConnectionAccepted);
          break;

        case ConnectionState.Connected:
          SendCommand(connection, Commands.ConnectionAccepted);
          break;

        case ConnectionState.Connecting:
          Assert.AlwaysFail();
          break;
      }
    }

    void HandleConnectionAccepted(Connection connection, Packet packet) {
      switch (connection.State) {
        case ConnectionState.Created:
          Assert.AlwaysFail();
          break;

        case ConnectionState.Connected:
          // ignore this
          break;

        case ConnectionState.Connecting:
          connection.ChangeState(ConnectionState.Connected);
          break;
      }
    }

    void SendCommand(Connection connection, Commands command) {
      Log.Trace($"Sending command {command} to {connection}");
      Send(connection, new byte[2] {(byte) PacketType.Command, (byte) command});
    }

    void Send(Connection connection, byte[] data) {
      _socket.SendTo(data, connection.RemoteEndPoint);
    }


    static void SetConnReset(Socket s) {
      try {
        const uint IOC_IN     = 0x80000000;
        const uint IOC_VENDOR = 0x18000000;
        s.IOControl(unchecked((int) (IOC_IN | IOC_VENDOR | 12)), new byte[] {System.Convert.ToByte(false)}, null);
      } catch {
      }
    }
  }
}