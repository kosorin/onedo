using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTasks.Controls.Calendar
{
    public class DisplayModeChangedEventArgs : EventArgs
    {
        private DisplayModeChangedEventArgs() { }

        public DisplayModeChangedEventArgs(DisplayMode mode)
        {
            Mode = mode;
        }

        public DisplayMode Mode { get; private set; }
    }
}
