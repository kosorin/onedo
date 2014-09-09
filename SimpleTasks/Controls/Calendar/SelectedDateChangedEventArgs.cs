using System;

namespace SimpleTasks.Controls.Calendar
{
    /// <summary>
    /// Event arguments for SelectionChanged event of the calendar
    /// </summary>
    public class SelectedDateChangedEventArgs : EventArgs
    {
        private SelectedDateChangedEventArgs() { }

        public SelectedDateChangedEventArgs(DateTime dateTime)
        {
            SelectedDate = dateTime;
        }

        /// <summary>
        /// Date that is currently selected on the calendar
        /// </summary>
        public DateTime SelectedDate { get; private set; }
    }
}
