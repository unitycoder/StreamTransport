using System.Net;
using System.Threading;

/*
CLIENT: created connection object for 127.0.0.1:25000
CLIENT: 127.0.0.1:25000 changed state from Created to Connecting => SENDS CONNECT REQUEST
SERVER: received 2 from 127.0.0.1:57350
SERVER: created connection object for 127.0.0.1:57350
SERVER: 127.0.0.1:57350 changed state from Created to Connected => SENDS CONNECTION ACCEPTED
CLIENT: received 2 from 127.0.0.1:25000
CLIENT: 127.0.0.1:25000 changed state from Connecting to Connected
*/

namespace Transport {
  public class Connection {
    public ConnectionState State;
    public IPEndPoint      RemoteEndPoint;

    public int    ConnectionAttempts;
    public double ConnectionAttemptTime;

    public Connection(IPEndPoint remoteEndPoint) {
      State = ConnectionState.Created;

      RemoteEndPoint = remoteEndPoint;
    }

    public void ChangeState(ConnectionState state) {
      switch (state) {
        case ConnectionState.Connected:
          Assert.Check(State == ConnectionState.Created || State == ConnectionState.Connecting);
          break;
        
        case ConnectionState.Connecting:
          Assert.Check(State == ConnectionState.Created);
          break;
      }

      Log.Trace($"{RemoteEndPoint} changed state from {State} to {state}");
      State = state;
    }

    public override string ToString() {
      return $"[Connection RemoteEndPoint={RemoteEndPoint}]";
    }
  }
}