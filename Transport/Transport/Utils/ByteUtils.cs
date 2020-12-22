namespace Transport {
  public static unsafe class ByteUtils {

    public static void WriteULong(byte[] data, int offset, int bytes, ulong value) {
      byte* v = (byte*)&value;

      for (int i = 0; i < bytes; ++i) {
        data[offset + i] = v[i];
      }
    } 
    
    public static ulong ReadUlong(byte[] data, int offset, int bytes) {
      ulong value = 0;
      byte* v     = (byte*)&value;

      for (int i = 0; i < bytes; ++i) {
        v[i] = data[offset + i];
      }

      return value;
    } 
    
  }
}