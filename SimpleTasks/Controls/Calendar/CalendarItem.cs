using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SimpleTasks.Controls.Calendar
{
    /// <summary>
    /// This class corresponds to a calendar item / cell
    /// </summary>
    [TemplateVisualState(Name = SelectedState, GroupName = SelectionStates)]
    [TemplateVisualState(Name = UnselectedState, GroupName = SelectionStates)]
    [TemplateVisualState(Name = CurrentMonthState, GroupName = MonthStates)]
    [TemplateVisualState(Name = OtherMonthState, GroupName = MonthStates)]
    public class CalendarItem : Button
    {
        #region Visual States
        private const string SelectionStates = "SelectionStates";
        private const string SelectedState = "Selected";
        private const string UnselectedState = "Unselected";

        private const string MonthStates = "MonthStates";
        private const string CurrentMonthState = "CurrentMonth";
        private const string OtherMonthState = "OtherMonth";

        private void UpdateVisualStates()
        {
            VisualStateManager.GoToState(this, IsSelected ? SelectedState : UnselectedState, true);
            VisualStateManager.GoToState(this, IsCurrentMonth ? CurrentMonthState : OtherMonthState, true);
        }
        #endregion

        #region Fields
        private readonly Calendar _owningCalendar;
        #endregion

        #region Constructor
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
        public int DayNumber
        {
            get { return (int)GetValue(DayNumberProperty); }
            private set { SetValue(DayNumberProperty, value); }
        }
        public static readonly DependencyProperty DayNumberProperty =
            DependencyProperty.Register("DayNumber", typeof(int), typeof(CalendarItem), new PropertyMetadata(42));
        #endregion

        #region Is Selected
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(CalendarItem), new PropertyMetadata(false, OnIsSelectedChanged));

        private static void OnIsSelectedChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var item = source as CalendarItem;
            if (item != null)
            {
                item.UpdateVisualStates();
            }
        }
        #endregion

        #region Is Current Month
        public bool IsCurrentMonth
        {
            get { return (bool)GetValue(IsCurrentMonthProperty); }
            set { SetValue(IsCurrentMonthProperty, value); }
        }

        public static readonly DependencyProperty IsCurrentMonthProperty =
            DependencyProperty.Register("IsCurrentMonth", typeof(bool), typeof(CalendarItem), new PropertyMetadata(true, OnIsCurrentMonthChanged));

        private static void OnIsCurrentMonthChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var item = source as CalendarItem;
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
            DependencyProperty.Register("Date", typeof(DateTime), typeof(CalendarItem), new PropertyMetadata(DateTime.MinValue, OnDateChanged));

        private static void OnDateChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var item = source as CalendarItem;
            if (item != null)
            {
                item.DayNumber = ((DateTime)e.NewValue).Day;
            }
        }
        #endregion

        #endregion

        #region Template
        /// <summary>
        /// Apply default template and perform initialization
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            UpdateVisualStates();
        }
        #endregion
    }
}
