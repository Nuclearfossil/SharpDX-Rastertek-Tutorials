using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSharpDXRastertek.Series2.Tut02.Input
{
    public class DInput
    {
        private Dictionary<Keys, bool> InputKeys = new Dictionary<Keys, bool>();

        internal bool Initialize()
        {
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
                InputKeys[(Keys)key] = false;

            return true;
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
