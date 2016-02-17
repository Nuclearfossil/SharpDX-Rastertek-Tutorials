using System;
using System.Diagnostics;

namespace DSharpDXRastertek.TutTerr09.System
{
    public class DCPU                   // 72 lines
    {
        // Variables
        private bool _CanReadCPU;
        private PerformanceCounter counter;
        private PerformanceCounter counter2;
        private TimeSpan _LastSampleTime;
        private long _CpuUsage;
        private long _CpuUsage0;

        // Properties
        public int CPUUsage { get { return _CanReadCPU ? (int)_CpuUsage : 0; } }
        public int CPUUsage0 { get { return _CanReadCPU ? (int)_CpuUsage0 : 0; } }

        // Public Methods.
        public void Initialize()
        {
            // Initialize the flag indicating whether this object can read the system cpu usage or not.
            _CanReadCPU = true;

            try
            {
                // Create performance counter.
                counter = new PerformanceCounter();
                counter.CategoryName = "Processor";
                counter.CounterName = "% Processor Time";
                counter.InstanceName = "_Total";

                counter2 = new PerformanceCounter();
                counter2.CategoryName = "Processor Information";
                counter2.CounterName = "% Processor Time";
                counter2.InstanceName = "0,0";

                _LastSampleTime = DateTime.Now.TimeOfDay;

                _CpuUsage = 0;
                _CpuUsage0 = 0;
            }
            catch
            {
                _CanReadCPU = false;
            }
        }
        public void Shutdown()
        {
            if (_CanReadCPU)
            {
                counter.Close();
                counter2.Close();
            }
        }
        public void Frame()
        {
            if (_CanReadCPU)
            {
                int secondsElapsed = (DateTime.Now.TimeOfDay - _LastSampleTime).Seconds;

                if (secondsElapsed >= 1)
                {
                    _LastSampleTime = DateTime.Now.TimeOfDay;
                    _CpuUsage0 = (int)counter2.NextValue();
                    _CpuUsage = (int)counter.NextValue();
                }
            }
        }
    }
}