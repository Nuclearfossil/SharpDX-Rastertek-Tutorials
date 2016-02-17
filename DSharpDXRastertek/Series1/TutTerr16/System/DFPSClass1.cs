using System;

namespace DSharpDXRastertek.TutTerr16.System
{
    public class DFPS                   // 42 lines
    {
        // Variables
        private int _Count;
        private TimeSpan _StartTime;

        // Propertues
        public int FPS { get; private set; }

        public void Initialize()
        {
            FPS = 0;
            _Count = 0;
            _StartTime = DateTime.Now.TimeOfDay;
        }
        public void Frame()
        {
            // Increment the number of frames passed this second.
            _Count++;

            // Determine if a second has passed since the last update of FPS.
            int secondsPassed = (DateTime.Now.TimeOfDay - _StartTime).Seconds;

            // When a second has elasped perform the following.
            if (secondsPassed >= 1)
            {
                // Assign the counted frames that poassed during this second to the 'Value' property
                FPS = _Count;

                // Reset the '_Count' variable to 0 to begin counting frames for the NEXT second
                _Count = 0;

                // Rreset '_StartTime' to current time for this next Frame.
                _StartTime = DateTime.Now.TimeOfDay;
            }
        }
    }
}