using DSharpDXRastertek.Series2.Tut02.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSharpDXRastertek.Series2.Tut02.Graphics
{
    public class DGraphics
    {
        public bool Initialize(DSystemConfiguration Configuration)
        {
            return true;
        }
        public void ShutDown() { }
        public bool Frame()
        {
            return true;
        }
        public bool Render()
        {
            return true;
        }
    }
}