using System.Net;

namespace Transport {
  public class Config {
    public IPEndPoint EndPoint;
    public int        Mtu                    = 1280 - 28; // UDP + IP header = 28 bytes
    public int        MaxConnections         = 10;
    public int        MaxConnectAttempts     = 10;
    public double     ConnectAttemptInterval = 0.25;
    public double     ConnectionTimeout      = 5;
    public double     DisconnectIdleTime     = 2;
  }
}