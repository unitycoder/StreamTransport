namespace Transport {
  public enum ConnectionState {
    Created    = 1,
    Connecting = 2,
    Connected  = 3,

    // ..

    Disconnected = 9,
    Destroyed    = 10,
  }
}