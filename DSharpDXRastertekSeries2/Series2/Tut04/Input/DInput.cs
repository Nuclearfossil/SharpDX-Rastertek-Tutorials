using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DSharpDXRastertek.Series2.Tut04.Input
{
    public class DInput
    {
        private Dictionary<Keys, bool> InputKeys = new Dictionary<Keys, bool>();

        internal void Initialize()
        {
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
                InputKeys[(Keys)key] = false;
        }
        internal bool IsKeyDown(Keys key)
        {
            return InputKeys[key];
        }
        internal void KeyDown(Keys key)
        {
            InputKeys[key] = true;
        }
        internal void KeyUp(Keys key)
        {
            InputKeys[key] = false;
        }
    }
}
