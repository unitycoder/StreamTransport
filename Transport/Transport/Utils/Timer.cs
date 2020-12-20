using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Transport {
  public struct Timer {
    long _start;
    long _elapsed;
    byte _running;

    public static Timer StartNew() {
      Timer t;
      t = default;
      t.Start();
      return t;
    }

    public long ElapsedInTicks {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _running == 1 ? _elapsed + GetDelta() : _elapsed;
    }

    public double ElapsedInMilliseconds {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => ElapsedInSeconds * 1000.0;
    }

    public double ElapsedInSeconds {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => ElapsedInTicks / (double) Stopwatch.Frequency;
    }

    public double Now {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => ElapsedInSeconds;
    }

    public bool IsRunning {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _running == 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Start() {
      if (_running == 0) {
        _start   = Stopwatch.GetTimestamp();
        _running = 1;
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Stop() {
      var dt = GetDelta();

      if (_running == 1) {
        _elapsed += dt;
        _running =  0;

        if (_elapsed < 0) {
          _elapsed = 0;
        }
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Reset() {
      _elapsed = 0;
      _running = 0;
      _start   = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Restart() {
      _elapsed = 0;
      _running = 1;
      _start   = Stopwatch.GetTimestamp();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    long GetDelta() {
      return Stopwatch.GetTimestamp() - _start;
    }
  }
}