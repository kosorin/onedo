using System;
using System.Windows;
using System.Windows.Controls;

namespace SimpleTasks.Controls.Calendar
{
    public class CalendarDayOfWeekItem : Control
    {
        #region Constructor
        public CalendarDayOfWeekItem()
        {
            DefaultStyleKey = typeof(CalendarDayOfWeekItem);
        }
        #endregion

        #region Properties

        #region Day of Week
        public DayOfWeek DayOfWeek
        {
            get { return (DayOfWeek)GetValue(DayOfWeekProperty); }
            set { SetValue(DayOfWeekProperty, value); }
        }
        public static readonly DependencyProperty DayOfWeekProperty =
            DependencyProperty.Register("DayOfWeek", typeof(DayOfWeek), typeof(CalendarDayOfWeekItem), new PropertyMetadata(DayOfWeek.Sunday));
        #endregion // end of Day of Week

        #region Text
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(CalendarDayOfWeekItem), new PropertyMetadata(""));
        #endregion

        #endregion
    }
}
