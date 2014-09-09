using System;

namespace SimpleTasks.Controls.Calendar
{
    /// <summary>
    /// Event arguments used for MonthChanging and MonthChanged events of the calendar
    /// </summary>
    public class CurrentDateChangedEventArgs : EventArgs
    {
        private CurrentDateChangedEventArgs() { }

        public CurrentDateChangedEventArgs(DateTime date)
        {
            Date = date;
        }

        public DateTime Date { get; private set; }
    }
}
