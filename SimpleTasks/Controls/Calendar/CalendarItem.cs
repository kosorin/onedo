using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SimpleTasks.Controls.Calendar
{
    /// <summary>
    /// This class corresponds to a calendar item / cell
    /// </summary>
    public class CalendarItem : Button
    {
        #region Fields
        private readonly Calendar _owningCalendar;
        #endregion

        #region Constructor
        /// <summary>
        /// Create new instance of a calendar cell
        /// </summary>
        [Obsolete("Internal use only")]
        public CalendarItem()
        {
            DefaultStyleKey = typeof(CalendarItem);
        }

        /// <summary>
        /// Create new instance of a calendar cell
        /// </summary>
        /// <param name="owner">Calenda control that a cell belongs to</param>
        public CalendarItem(Calendar owner)
        {
            DefaultStyleKey = typeof(CalendarItem);
            _owningCalendar = owner;
        }
        #endregion

        #region Properties

        #region Day Number
        /// <summary>
        /// Day number for this calendar cell.
        /// This changes depending on the month shown
        /// </summary>
        public int DayNumber
        {
            get { return (int)GetValue(DayNumberProperty); }
            internal set { SetValue(DayNumberProperty, value); }
        }

        /// <summary>
        /// Day number for this calendar cell.
        /// This changes depending on the month shown
        /// </summary>
        public static readonly DependencyProperty DayNumberProperty =
            DependencyProperty.Register("DayNumber", typeof(int), typeof(CalendarItem), new PropertyMetadata(0, OnDayNumberChanged));

        private static void OnDayNumberChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var item = source as CalendarItem;
            if (item != null)
            {
                item.SetForegroundColor();
                item.SetBackgroundColor();
            }
        }
        #endregion

        #region Is Selected
        internal bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        internal static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(CalendarItem), new PropertyMetadata(false, OnIsSelectedChanged));

        private static void OnIsSelectedChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var item = source as CalendarItem;
            if (item != null)
            {
                item.SetBackgroundColor();
                item.SetForegroundColor();
            }
        }
        #endregion

        #region Date
        /// <summary>
        /// Date for the calendar item
        /// </summary>
        public DateTime Date
        {
            get { return (DateTime)GetValue(DateProperty); }
            internal set { SetValue(DateProperty, value); }
        }

        /// <summary>
        /// Date for the calendar item
        /// </summary>
        internal static readonly DependencyProperty DateProperty =
            DependencyProperty.Register("Date", typeof(DateTime), typeof(CalendarItem), new PropertyMetadata(null));
        #endregion

        #endregion

        #region Template
        /// <summary>
        /// Apply default template and perform initialization
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            SetBackgroundColor();
            SetForegroundColor();
        }

        private bool IsConverterNeeded()
        {
            return _owningCalendar.DatesSource == null || _owningCalendar.DatesAssigned.Contains(Date);
        }

        internal void SetBackgroundColor()
        {
            Brush defaultSelectedBrush = new SolidColorBrush(Colors.Transparent);
            Brush defaultBrush = Application.Current.Resources["PhoneAccentBrush"] as Brush;
            if (_owningCalendar.ColorConverter != null && IsConverterNeeded())
            {
                Background = _owningCalendar.ColorConverter.Convert(Date, IsSelected, IsSelected ? defaultBrush : defaultSelectedBrush, BrushType.Background);
            }
            else
            {
                Background = IsSelected ? defaultBrush : defaultSelectedBrush;
            }
        }

        internal void SetForegroundColor()
        {
            Brush defaultBrush = Application.Current.Resources["PhoneForegroundBrush"] as Brush;
            if (_owningCalendar.ColorConverter != null && IsConverterNeeded())
            {
                Foreground = _owningCalendar.ColorConverter.Convert(Date, IsSelected, defaultBrush, BrushType.Foreground);
            }
            else
            {
                Foreground = defaultBrush;
            }
        }
        #endregion
    }
}
