namespace Transport {
  public struct Sequencer {
    int _shift;
    int _bytes;
    
    ulong _mask;
    ulong _sequence;

    public int Bytes => _bytes;

    public Sequencer(int bytes) {
      // 1 byte
      // (1 << 8) = 256
      // - 1      = 255
      //          = 1111 1111
      
      _bytes    = bytes;
      _sequence = 0;
      _mask     = (1UL << (bytes * 8)) - 1UL;
      _shift    = (sizeof(ulong) - bytes) * 8;
    }

    public ulong Next() {
      return _sequence = NextAfter(_sequence);
    }

    public ulong NextAfter(ulong sequence) {
      return (sequence + 1UL) & _mask;
    }

    public long Distance(ulong from, ulong to) {
      to <<= _shift;
      from <<= _shift;
      return ((long) (from - to)) >> _shift;
    }
    
    // 0 1 2 3 4 5 6 7 8 9 ... 255
    // wraps around back to 0
    
  }
}