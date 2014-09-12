using System;

namespace SimpleTasks.Controls.Calendar
{
    public class DateChangedEventArgs : EventArgs
    {
        private DateChangedEventArgs() { }

        public DateChangedEventArgs(DateTime date)
        {
            Date = date;
        }

        public DateTime Date { get; private set; }
    }
}
