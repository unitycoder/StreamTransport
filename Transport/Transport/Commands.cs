using System.Runtime.InteropServices;

namespace Transport {
  public enum Commands : byte {
    ConnectRequest = 1, // from client to server when connecting first time
    ConnectionAccepted = 2, // from server to client when a connect request is accepted
    ConnectionRefused = 3,
    Disconnect = 4,
  }

  public enum ConnectionFailedReason : byte {
    ServerFull = 1,
  }

  public enum DisconnectedReason : byte {
    Timeout = 1,
    RequestedByPeer = 2,
    SequenceOutOfBounds = 3,
  }
}