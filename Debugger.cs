
using System.Diagnostics;

namespace dotnet {
  internal static class DebugInfo
    {
        private static Stopwatch stopwatch;
        private static long initialMemory;

        static DebugInfo()
        {
            stopwatch = new();
            initialMemory = GC.GetTotalMemory(true);
        }

        internal static void Start() => stopwatch.Start();
        internal static void Reset() { stopwatch = new(); initialMemory = GC.GetTotalMemory(true); }
        internal static Tuple<long, long> StopAndReturn()
        {
            stopwatch.Stop();
            return Tuple.Create(stopwatch.ElapsedMilliseconds, GC.GetTotalMemory(true) - initialMemory);
        }
    }
}