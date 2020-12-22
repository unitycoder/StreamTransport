using System;

namespace Transport {
  public class RingBuffer<T> {
    int _head;
    int _tail;
    int _count;

    T[] _array;

    public int  Count  => _count;
    public bool IsFull => _count == _array.Length;

    public RingBuffer(int capacity) {
      _array = new T[capacity];
    }

    public void Push(T item) {
      if (IsFull) {
        throw new InvalidOperationException();
      }

      _array[_head] =  item;
      _head         =  (_head + 1) % _array.Length;
      _count        += 1;
    }

    public T Pop() {
      Assert.Check(Count > 0);
      
      var item = _array[_tail];

      _array[_tail] =  default;
      _tail         =  (_tail + 1) % _array.Length;
      _count        -= 1;

      return item;
    }

    public void Clear() {
      _head  = 0;
      _tail  = 0;
      _count = 0;
      
      Array.Clear(_array, 0, _array.Length);
    }
  }
}