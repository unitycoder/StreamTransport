namespace Transport {
  public enum PacketTypes : byte {
    Command = 1,
    Unreliable = 2,
    Notify = 3,
    KeepAlive = 4
  }
}


/*
 * Notify
 * - We DONT provide any reliable delivery
 * - We DO   protect against duplicates and out-of-order packets
 * - We DO   tell the sender *if* the packet arrived
 * - We DO   tell the sender if the packet *most likely* was lost
 */
 
 // EXAMPLE:
 // 1 2 3 4 5
 // 1     4 5
 //
 // DELIVERED 1 4 5 
 // LOST      2 3
 