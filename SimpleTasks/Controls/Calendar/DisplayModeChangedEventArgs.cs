using System;

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
