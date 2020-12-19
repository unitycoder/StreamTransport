using System.Runtime.InteropServices;

namespace Transport {
  public enum Commands : byte {
    ConnectRequest = 1, // from client to server when connecting first time
    ConnectionAccepted = 2, // from server to client when a connect request is accepted
  }
}