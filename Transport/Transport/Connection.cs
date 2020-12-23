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
  public struct SendEnvelope {
    public ulong  Sequence;
    public double SendTime;
    public object UserData;
  }

  public class Connection {
    public ConnectionState State;
    public IPEndPoint      RemoteEndPoint;

    public int    ConnectionAttempts;
    public double ConnectionAttemptTime;
    public double LastSentPacketTime;
    public double LastRecvPacketTime;
    public double DisconnectTime;

    public double Rtt;

    // notify fields

    public Sequencer                SendSequencer;
    public RingBuffer<SendEnvelope> SendWindow;

    public ulong RecvSequence;
    public ulong RecvMask;

    // initial state
    // seq: 0
    // mas: 0000 0000 0000 0000 

    // first packet
    // seq: 1
    // mas: 1000 0000 0000 0000 

    // second packet
    // seq: 2
    // mas: 1100 0000 0000 0000 

    // third packet
    // seq: 3
    // mas: 1110 0000 0000 0000 

    // fourth packet: lost

    // fifth packet
    // seq: 5 
    // mas: 1011 1000 0000 0000

    public Connection(Config config, IPEndPoint remoteEndPoint) {
      State          = ConnectionState.Created;
      RemoteEndPoint = remoteEndPoint;
      SendSequencer  = new Sequencer(config.SequenceNumberBytes);
      SendWindow     = new RingBuffer<SendEnvelope>(config.SendWindowSize);
    }

    public void ChangeState(ConnectionState state) {
      switch (state) {
        case ConnectionState.Connected:
          Assert.Check(State == ConnectionState.Created || State == ConnectionState.Connecting);
          break;

        case ConnectionState.Connecting:
          Assert.Check(State == ConnectionState.Created);
          break;

        case ConnectionState.Disconnected:
          Assert.Check(State == ConnectionState.Connected);
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