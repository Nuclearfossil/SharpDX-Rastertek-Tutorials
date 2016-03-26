using System.Diagnostics;

namespace DSharpDXRastertek.Series2.Tut02.System
{
    public class DTimer
    {
        private Stopwatch _StopWatch;
        private float m_ticksPerMs;
        private long m_LastFrameTime = 0;

        public float FrameTime { get; private set; }
        public float CumulativeFrameTime { get; private set; }

        public bool Initialize()
        {
            if (!Stopwatch.IsHighResolution)
                return false;
            if (Stopwatch.Frequency == 0)
                return false;

            _StopWatch = Stopwatch.StartNew();
            m_ticksPerMs = (Stopwatch.Frequency / 1000.0f);
            return true;
        }
        public void Frame2()
        {
            long currentTime = _StopWatch.ElapsedTicks;
            float timeDifference = currentTime - m_LastFrameTime;
            FrameTime = timeDifference / m_ticksPerMs;
            CumulativeFrameTime += FrameTime;
            m_LastFrameTime = currentTime;
        }
    }
}
