using System;
using System.Windows;
using System.Windows.Controls;
using SimpleTasks.Core.Helpers;

namespace SimpleTasks.Controls.Calendar
{
    /// <summary>
    /// This class corresponds to a calendar item / cell
    /// </summary>
    [TemplateVisualState(Name = SelectedState, GroupName = SelectionStates)]
    [TemplateVisualState(Name = UnselectedState, GroupName = SelectionStates)]
    [TemplateVisualState(Name = IsThisMonthState, GroupName = ThisMonthStates)]
    [TemplateVisualState(Name = NotThisMonthState, GroupName = ThisMonthStates)]
    public class CalendarMonthItem : Button
    {
        #region Fields
        private readonly Calendar _owningCalendar;
        #endregion

        #region Constructor
        public CalendarMonthItem(Calendar owner)
        {
            DefaultStyleKey = typeof(CalendarMonthItem);
            _owningCalendar = owner;
        }
        #endregion

        #region Properties

        #region Month Number
        public int MonthNumber
        {
            get { return (int)GetValue(MonthNumberProperty); }
            private set { SetValue(MonthNumberProperty, value); }
        }
        public static readonly DependencyProperty MonthNumberProperty =
            DependencyProperty.Register("MonthNumber", typeof(int), typeof(CalendarMonthItem), new PropertyMetadata(42));
        #endregion

        #region Month Text
        public string MonthText
        {
            get { return (string)GetValue(MonthTextProperty); }
            private set { SetValue(MonthTextProperty, value); }
        }
        public static readonly DependencyProperty MonthTextProperty =
            DependencyProperty.Register("MonthText", typeof(string), typeof(CalendarMonthItem), new PropertyMetadata("dummy"));
        #endregion

        #region Is Selected
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(CalendarMonthItem), new PropertyMetadata(false, OnIsSelectedChanged));

        private static void OnIsSelectedChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var item = source as CalendarMonthItem;
            if (item != null)
            {
                item.UpdateVisualStates();
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
            set { SetValue(DateProperty, value); }
        }

        /// <summary>
        /// Date for the calendar item
        /// </summary>
        public static readonly DependencyProperty DateProperty =
            DependencyProperty.Register("Date", typeof(DateTime), typeof(CalendarMonthItem), new PropertyMetadata(DateTime.MinValue, OnDateChanged));

        private static void OnDateChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var item = source as CalendarMonthItem;
            if (item != null)
            {
                DateTime date = (DateTime)e.NewValue;
                item.MonthNumber = date.Month;
                item.MonthText = item._owningCalendar.DateTimeFormatInfo.MonthNames[date.Month - 1];

                item.UpdateVisualStates();
            }
        }
        #endregion

        #endregion

        #region Template
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            UpdateVisualStates();
        }
        #endregion

        #region Visual States
        private const string SelectionStates = "SelectionStates";
        private const string SelectedState = "Selected";
        private const string UnselectedState = "Unselected";

        private const string ThisMonthStates = "ThisMonthStates";
        private const string IsThisMonthState = "IsThisMonth";
        private const string NotThisMonthState = "NotThisMonth";

        private void UpdateVisualStates()
        {
            VisualStateManager.GoToState(this, IsSelected ? SelectedState : UnselectedState, true);
            VisualStateManager.GoToState(this, Date.IsSameMonth(DateTime.Today) ? IsThisMonthState : NotThisMonthState, true);
        }
        #endregion
    }
}
