using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows.Input;
using Microsoft.Xna.Framework.Input.Touch;
using SimpleTasks.Core.Helpers;
using System.Diagnostics;
using SimpleTasks.Helpers;
using System.Windows.Media;
using System.Windows.Media.Animation;
using SimpleTasks.Core.Models;

namespace SimpleTasks.Controls.Calendar
{
    [TemplatePart(Name = YearMonthBorderName, Type = typeof(Border))]
    [TemplatePart(Name = PreviousButtonName, Type = typeof(Button))]
    [TemplatePart(Name = NextMonthName, Type = typeof(Button))]
    [TemplatePart(Name = DayOfWeekItemsGridName, Type = typeof(Grid))]
    [TemplatePart(Name = ItemsGridName, Type = typeof(Grid))]
    [TemplatePart(Name = DayItemsGridName, Type = typeof(Grid))]
    [TemplatePart(Name = MonthItemsGridName, Type = typeof(Grid))]
    [TemplateVisualState(Name = DaysModeState, GroupName = DisplayStates)]
    [TemplateVisualState(Name = MonthsModeState, GroupName = DisplayStates)]
    public class Calendar : Control
    {
        #region Constructor
        /// <summary>
        /// Create new instance of a calendar
        /// </summary>
        public Calendar()
        {
            DefaultStyleKey = typeof(Calendar);

            DateTimeFormatInfo = CultureInfo.CurrentCulture.DateTimeFormat;
            FirstDayOfWeek = DateTimeFormatInfo.FirstDayOfWeek;
            Sunday = DateTimeFormatInfo.AbbreviatedDayNames[0].ToUpper();
            Monday = DateTimeFormatInfo.AbbreviatedDayNames[1].ToUpper();
            Tuesday = DateTimeFormatInfo.AbbreviatedDayNames[2].ToUpper();
            Wednesday = DateTimeFormatInfo.AbbreviatedDayNames[3].ToUpper();
            Thursday = DateTimeFormatInfo.AbbreviatedDayNames[4].ToUpper();
            Friday = DateTimeFormatInfo.AbbreviatedDayNames[5].ToUpper();
            Saturday = DateTimeFormatInfo.AbbreviatedDayNames[6].ToUpper();
        }
        #endregion

        #region Fields
        private Grid _dayOfWeekItemsGrid = null;
        private Grid _itemsGrid = null;
        private Grid _dayItemsGrid = null;
        private Grid _monthItemsGrid = null;
        private DateTime _oldDisplayDate = DateTime.MinValue;

        private const double _flickTreshold = 1000;

        private const short _rowCount = 6;
        private const short _columnCount = 7;
        private CalendarDayOfWeekItem[] _calendarDayOfWeekItems = new CalendarDayOfWeekItem[_columnCount];
        private CalendarItem[,] _calendarItems = new CalendarItem[_rowCount, _columnCount];

        private const short _monthRowCount = 4;
        private const short _monthColumnCount = 3;
        private CalendarMonthItem[,] _calendarMonthItems = new CalendarMonthItem[_monthRowCount, _monthColumnCount];
        #endregion

        #region Events
        public event EventHandler<DateChangedEventArgs> DisplayDateChanged;

        public event EventHandler<DisplayModeChangedEventArgs> DisplayModeChanged;

        /// <summary>
        /// Event that occurs after a date is selected on the calendar
        /// </summary>
        public event EventHandler<DateChangedEventArgs> SelectedDateChanged;

        /// <summary>
        /// Event that occurs after a date is clicked on
        /// </summary>
        public event EventHandler<DateChangedEventArgs> DateClicked;

        protected void OnDisplayDateChanged(DateTime date)
        {
            if (DisplayDateChanged != null)
            {
                DisplayDateChanged(this, new DateChangedEventArgs(date));
            }
        }

        protected void OnDisplayModeChanged(DisplayMode mode)
        {
            if (DisplayModeChanged != null)
            {
                DisplayModeChanged(this, new DisplayModeChangedEventArgs(mode));
            }
        }

        /// <summary>
        /// Raises SelectedChanged event
        /// </summary>
        /// <param name="dateTime">Selected date</param>
        protected void OnSelectedDateChanged(DateTime dateTime)
        {
            if (SelectedDateChanged != null)
            {
                SelectedDateChanged(this, new DateChangedEventArgs(dateTime));
            }
        }

        /// <summary>
        /// Raises DateClicked event
        /// </summary>
        /// <param name="dateTime">Selected date</param>
        protected void OnDateClicked(DateTime dateTime)
        {
            if (DateClicked != null)
            {
                DateClicked(this, new DateChangedEventArgs(dateTime));
            }
        }
        #endregion

        #region Properties

        public DateTimeFormatInfo DateTimeFormatInfo { get; private set; }

        #region ItemTemplate
        public ControlTemplate ItemTemplate
        {
            get { return (ControlTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(ControlTemplate), typeof(Calendar), new PropertyMetadata(null));
        #endregion // end of ItemTemplate

        #region DayOfWeekItemTemplate
        public ControlTemplate DayOfWeekItemTemplate
        {
            get { return (ControlTemplate)GetValue(DayOfWeekItemTemplateProperty); }
            set { SetValue(DayOfWeekItemTemplateProperty, value); }
        }

        public static readonly DependencyProperty DayOfWeekItemTemplateProperty =
            DependencyProperty.Register("DayOfWeekItemTemplate", typeof(ControlTemplate), typeof(Calendar), null);
        #endregion // end of DayOfWeekItemTemplate

        #region Minimum/Maximum Date
        /// <summary>
        /// Minimum Date that calendar navigation supports
        /// </summary>
        public DateTime MinimumDate
        {
            get { return (DateTime)GetValue(MinimumDateProperty); }
            set { SetValue(MinimumDateProperty, value); }
        }
        /// <summary>
        /// Minimum Date that calendar navigation supports
        /// </summary>
        public static readonly DependencyProperty MinimumDateProperty =
            DependencyProperty.Register("MinimumDate", typeof(DateTime), typeof(Calendar), new PropertyMetadata(new DateTime(1753, 1, 1)));

        /// <summary>
        /// Maximum Date that calendar navigation supports
        /// </summary>
        public DateTime MaximumDate
        {
            get { return (DateTime)GetValue(MaximumDateProperty); }
            set { SetValue(MaximumDateProperty, value); }
        }
        /// <summary>
        /// Maximum Date that calendar navigation supports
        /// </summary>
        public static readonly DependencyProperty MaximumDateProperty =
            DependencyProperty.Register("MaximumDate", typeof(DateTime), typeof(Calendar), new PropertyMetadata(new DateTime(2499, 12, 31)));
        #endregion // end of Minimum/Maximum Date

        #region SelectedDate +
        /// <summary>
        /// This value currently selected date on the calendar
        /// This property can be bound to
        /// </summary>
        public DateTime SelectedDate
        {
            get { return (DateTime)GetValue(SelectedDateProperty); }
            set { SetValue(SelectedDateProperty, value); }
        }

        /// <summary>
        /// This value currently selected date on the calendar
        /// This property can be bound to
        /// </summary>
        public static readonly DependencyProperty SelectedDateProperty =
            DependencyProperty.Register("SelectedDate", typeof(DateTime), typeof(Calendar), new PropertyMetadata(DateTime.MinValue, OnSelectedDateChanged));

        private static void OnSelectedDateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var calendar = sender as Calendar;
            if (calendar != null)
            {
                DateTime date = (DateTime)e.NewValue;
                if (date.IsSameMonth(calendar.DisplayDate))
                {
                    if (calendar._itemsGrid != null)
                    {
                        foreach (CalendarItem item in calendar.GetItems())
                        {
                            item.IsSelected = item.Date == date;
                        }
                    }
                    if (calendar._monthItemsGrid != null)
                    {
                        foreach (CalendarMonthItem item in calendar.GetMonthItems())
                        {
                            item.IsSelected = item.Date.IsSameMonth(date);
                        }
                    }
                }
                else
                {
                    calendar.DisplayDate = date;
                }

                calendar.OnSelectedDateChanged(date);
            }
        }
        #endregion // end of SelectedDate

        #region DisplayDate +
        public DateTime DisplayDate
        {
            get { return (DateTime)GetValue(DisplayDateProperty); }
            set { SetValue(DisplayDateProperty, value); }
        }
        public static readonly DependencyProperty DisplayDateProperty =
            DependencyProperty.Register("DisplayDate", typeof(DateTime), typeof(Calendar), new PropertyMetadata(DateTime.Today, OnDisplayDateChanged));

        private static void OnDisplayDateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var calendar = sender as Calendar;
            if (calendar != null)
            {
                DateTime oldDisplayDate = (DateTime)e.OldValue;
                DateTime displayDate = (DateTime)e.NewValue;

                if (!displayDate.IsSameMonth(oldDisplayDate))
                {
                    calendar.UpdateYearMonthLabel();
                    calendar.BuildItems();
                }
                if (!displayDate.IsSameYear(oldDisplayDate))
                {
                    calendar.UpdateYearLabel();
                    calendar.BuildMonthItems();
                }

                calendar.OnDisplayDateChanged(displayDate);
            }
        }
        #endregion // end of DisplayDate

        #region DisplayMode +
        public DisplayMode DisplayMode
        {
            get { return (DisplayMode)GetValue(DisplayModeProperty); }
            set { SetValue(DisplayModeProperty, value); }
        }
        public static readonly DependencyProperty DisplayModeProperty =
            DependencyProperty.Register("DisplayMode", typeof(DisplayMode), typeof(Calendar), new PropertyMetadata(DisplayMode.Days, OnDisplayModeChanged));

        private static void OnDisplayModeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var calendar = sender as Calendar;
            if (calendar != null)
            {
                Point origin = new Point
                {
                    X = (((calendar.DisplayDate.Month - 1) % _monthColumnCount) / (double)(_monthColumnCount - 1)),
                    Y = ((calendar.DisplayDate.Month - 1) / _monthColumnCount) / (double)(_monthRowCount - 1)
                };
                if (calendar._dayItemsGrid != null)
                {
                    calendar._dayItemsGrid.RenderTransformOrigin = origin;
                }
                if (calendar._monthItemsGrid != null)
                {
                    calendar._monthItemsGrid.RenderTransformOrigin = origin;
                }

                if ((DisplayMode)e.OldValue == DisplayMode.Days)
                {
                    calendar._oldDisplayDate = calendar.DisplayDate;
                }

                DisplayMode mode = (DisplayMode)e.NewValue;
                calendar.UpdateVisualStates();
                calendar.OnDisplayModeChanged(mode);
            }
        }
        #endregion // end of DisplayMode +

        #region YearMonthLabel
        /// <summary>
        /// This value is shown in calendar header and includes month and year
        /// </summary>
        public string YearMonthLabel
        {
            get { return (string)GetValue(YearMonthLabelProperty); }
            private set { SetValue(YearMonthLabelProperty, value); }
        }

        /// <summary>
        /// This value is shown in calendar header and includes month and year
        /// </summary>
        public static readonly DependencyProperty YearMonthLabelProperty =
            DependencyProperty.Register("YearMonthLabel", typeof(string), typeof(Calendar), new PropertyMetadata(""));
        #endregion // end of YearMonthLabel

        #region YearLabel
        public string YearLabel
        {
            get { return (string)GetValue(YearLabelProperty); }
            set { SetValue(YearLabelProperty, value); }
        }
        public static readonly DependencyProperty YearLabelProperty =
            DependencyProperty.Register("YearLabel", typeof(string), typeof(Calendar), new PropertyMetadata(""));
        #endregion // end of YearLabel

        #region ShowNavigationButtons
        /// <summary>
        /// If true, previous and next month buttons are shown
        /// </summary>
        public bool ShowNavigationButtons
        {
            get { return (bool)GetValue(ShowNavigationButtonsProperty); }
            set { SetValue(ShowNavigationButtonsProperty, value); }
        }

        /// <summary>
        /// If true, previous and next month buttons are shown
        /// </summary>
        public static readonly DependencyProperty ShowNavigationButtonsProperty =
            DependencyProperty.Register("ShowNavigationButtons", typeof(bool), typeof(Calendar), new PropertyMetadata(true));
        #endregion // end of ShowNavigationButtons

        #region DayNames
        /// <summary>
        /// Gets or sets the label for Sunday
        /// </summary>
        /// <value>
        /// The label for Sunday
        /// </value>
        public string Sunday
        {
            get { return (string)GetValue(SundayProperty); }
            set { SetValue(SundayProperty, value); }
        }

        /// <summary>
        /// Gets or sets the label for Sunday
        /// </summary>
        /// <value>
        /// The label for Sunday
        /// </value>
        public static readonly DependencyProperty SundayProperty =
            DependencyProperty.Register("Sunday", typeof(string), typeof(Calendar), new PropertyMetadata("Su"));


        /// <summary>
        /// Gets or sets the label for Monday
        /// </summary>
        /// <value>
        /// The label for Monday
        /// </value>
        public string Monday
        {
            get { return (string)GetValue(MondayProperty); }
            set { SetValue(MondayProperty, value); }
        }

        /// <summary>
        /// Gets or sets the label for Monday
        /// </summary>
        /// <value>
        /// The label for Monday
        /// </value>
        public static readonly DependencyProperty MondayProperty =
            DependencyProperty.Register("Monday", typeof(string), typeof(Calendar), new PropertyMetadata("Mo"));


        /// <summary>
        /// Gets or sets the label for Tuesday
        /// </summary>
        /// <value>
        /// The label for Tuesday
        /// </value>
        public string Tuesday
        {
            get { return (string)GetValue(TuesdayProperty); }
            set { SetValue(TuesdayProperty, value); }
        }

        /// <summary>
        /// Gets or sets the label for Tuesday
        /// </summary>
        /// <value>
        /// The label for Tuesday
        /// </value>
        public static readonly DependencyProperty TuesdayProperty =
            DependencyProperty.Register("Tuesday", typeof(string), typeof(Calendar), new PropertyMetadata("Tu"));


        /// <summary>
        /// Gets or sets the label for Wednesday
        /// </summary>
        /// <value>
        /// The label for Wednesday
        /// </value>
        public string Wednesday
        {
            get { return (string)GetValue(WednesdayProperty); }
            set { SetValue(WednesdayProperty, value); }
        }

        /// <summary>
        /// Gets or sets the label for Wednesday
        /// </summary>
        /// <value>
        /// The label for Wednesday
        /// </value>
        public static readonly DependencyProperty WednesdayProperty =
            DependencyProperty.Register("Wednesday", typeof(string), typeof(Calendar), new PropertyMetadata("We"));


        /// <summary>
        /// Gets or sets the label for Thursday
        /// </summary>
        /// <value>
        /// The label for Thursday
        /// </value>
        public string Thursday
        {
            get { return (string)GetValue(ThursdayProperty); }
            set { SetValue(ThursdayProperty, value); }
        }
        /// <summary>
        /// Gets or sets the label for Thursday
        /// </summary>
        /// <value>
        /// The label for Thursday
        /// </value>
        public static readonly DependencyProperty ThursdayProperty =
            DependencyProperty.Register("Thursday", typeof(string), typeof(Calendar), new PropertyMetadata("Th"));


        /// <summary>
        /// Gets or sets the label for Friday
        /// </summary>
        /// <value>
        /// The label for Friday
        /// </value>
        public string Friday
        {
            get { return (string)GetValue(FridayProperty); }
            set { SetValue(FridayProperty, value); }
        }

        /// <summary>
        /// Gets or sets the label for Friday
        /// </summary>
        /// <value>
        /// The label for Friday
        /// </value>
        public static readonly DependencyProperty FridayProperty =
            DependencyProperty.Register("Friday", typeof(string), typeof(Calendar), new PropertyMetadata("Fr"));


        /// <summary>
        /// Gets or sets the label for Saturday
        /// </summary>
        /// <value>
        /// The label for Saturday
        /// </value>
        public string Saturday
        {
            get { return (string)GetValue(SaturdayProperty); }
            set { SetValue(SaturdayProperty, value); }
        }

        /// <summary>
        /// Gets or sets the label for Saturday
        /// </summary>
        /// <value>
        /// The label for Saturday
        /// </value>
        public static readonly DependencyProperty SaturdayProperty =
            DependencyProperty.Register("Saturday", typeof(string), typeof(Calendar), new PropertyMetadata("Sa"));
        #endregion // end of DayNames

        #region FirstDayOfWeek +
        /// <summary>
        /// Gets or sets the first day of week.
        /// </summary>
        /// <value>
        /// The first day of week.
        /// </value>
        public DayOfWeek FirstDayOfWeek
        {
            get { return (DayOfWeek)GetValue(FirstDayOfWeekProperty); }
            set { SetValue(FirstDayOfWeekProperty, value); }
        }

        /// <summary>
        /// The first day of week property
        /// </summary>
        public static readonly DependencyProperty FirstDayOfWeekProperty =
            DependencyProperty.Register("FirstDayOfWeek", typeof(DayOfWeek), typeof(Calendar), new PropertyMetadata(DayOfWeek.Sunday, OnFirstDayOfWeekChanged));

        private static void OnFirstDayOfWeekChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            Calendar calendar = (Calendar)sender;
            if (calendar != null)
            {
                calendar.BuildDayOfWeekItems();
                calendar.BuildItems();
            }
        }
        #endregion // end of FirstDayOfWeek

        #endregion

        #region Template
        private const string YearMonthBorderName = "YearMonthBorder";
        private const string PreviousButtonName = "PreviousButton";
        private const string NextMonthName = "NextButton";
        private const string DayOfWeekItemsGridName = "DayOfWeekItemsGrid";
        private const string ItemsGridName = "ItemsGrid";
        private const string DayItemsGridName = "DayItemsGrid";
        private const string MonthItemsGridName = "MonthItemsGrid";

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // YearMonth Border
            Border yearMonthBorder = GetTemplateChild(YearMonthBorderName) as Border;
            if (yearMonthBorder != null)
            {
                yearMonthBorder.Tap += YearMonthBorderTap;
            }

            // Tlačítka
            Button previousButton = GetTemplateChild(PreviousButtonName) as Button;
            if (previousButton != null)
            {
                previousButton.Click += PreviousButtonClick;
            }
            Button nextButton = GetTemplateChild(NextMonthName) as Button;
            if (nextButton != null)
            {
                nextButton.Click += NextButtonClick;
            }

            // Items Grid 
            _dayOfWeekItemsGrid = GetTemplateChild(DayOfWeekItemsGridName) as Grid;
            _itemsGrid = GetTemplateChild(ItemsGridName) as Grid;
            _dayItemsGrid = GetTemplateChild(DayItemsGridName) as Grid;
            _monthItemsGrid = GetTemplateChild(MonthItemsGridName) as Grid;
            if (_dayItemsGrid != null)
            {
                _dayItemsGrid.ManipulationCompleted -= DayItemsManipulationCompleted;
                _dayItemsGrid.ManipulationCompleted += DayItemsManipulationCompleted;
            }
            if (_monthItemsGrid != null)
            {
                _monthItemsGrid.ManipulationCompleted -= MonthItemsManipulationCompleted;
                _monthItemsGrid.ManipulationCompleted += MonthItemsManipulationCompleted;
            }

            CreateDayOfWeekItems();
            CreateItems();
            CreateMonthItems();

            UpdateYearMonthLabel();
            UpdateYearLabel();
            BuildDayOfWeekItems();
            BuildItems();
            BuildMonthItems();

            UpdateVisualStates();
        }
        #endregion

        #region Visual States
        private const string DisplayStates = "DisplayStates";
        private const string DaysModeState = "DaysMode";
        private const string MonthsModeState = "MonthsMode";

        private void UpdateVisualStates()
        {
            string mode;
            switch (DisplayMode)
            {
            case DisplayMode.Days: mode = DaysModeState; break;
            case DisplayMode.Months: mode = MonthsModeState; break;
            default: mode = DaysModeState; break;
            }
            VisualStateManager.GoToState(this, mode, true);
        }
        #endregion

        #region Event handling
        public void BackKeyPress(object sender, CancelEventArgs e)
        {
            if (GoBackToDaysDisplayMode())
            {
                e.Cancel = true;
            }
        }

        private void YearMonthBorderTap(object sender, GestureEventArgs e)
        {
            DisplayMode = DisplayMode.Months;
        }

        private void NextButtonClick(object sender, RoutedEventArgs e)
        {
            if (DisplayMode == DisplayMode.Months)
            {
                IncrementYear();
            }
            else
            {
                IncrementMonth();
            }
        }

        private void PreviousButtonClick(object sender, RoutedEventArgs e)
        {
            if (DisplayMode == DisplayMode.Months)
            {
                DecrementYear();
            }
            else
            {
                DecrementMonth();
            }
        }

        private void ItemTap(object sender, EventArgs e)
        {
            CalendarItem item = (sender as CalendarItem);
            if (item != null)
            {
                SelectedDate = item.Date;
                OnDateClicked(item.Date);
            }
        }

        private void MonthItemTap(object sender, EventArgs e)
        {
            CalendarMonthItem item = (sender as CalendarMonthItem);
            if (item != null)
            {
                DisplayDate = item.Date;
                DisplayMode = DisplayMode.Days;
            }
        }

        private void DayItemsManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            switch (GetGestureType(e))
            {
            case GestureType.FlickRightToLeft:
                IncrementMonth();
                break;
            case GestureType.FlickLeftToRight:
                DecrementMonth();
                break;
            }
        }

        private void MonthItemsManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            switch (GetGestureType(e))
            {
            case GestureType.FlickRightToLeft:
                IncrementYear();
                break;
            case GestureType.FlickLeftToRight:
                DecrementYear();
                break;
            }
        }
        #endregion

        #region Public Methods
        public bool GoBackToDaysDisplayMode()
        {
            if (DisplayMode != DisplayMode.Days)
            {
                if (_oldDisplayDate == DateTime.MinValue)
                {
                    _oldDisplayDate = DisplayDate;
                }
                DisplayDate = _oldDisplayDate;
                DisplayMode = DisplayMode.Days;
                return true;
            }
            return false;
        }
        #endregion // end of Public Methods

        #region Private Methods

        #region Month Inc/Dec
        private bool IncrementMonth()
        {
            DateTime next = NextMonth(DisplayDate);
            if (next <= MaximumDate)
            {
                DisplayDate = next;
                return true;
            }
            return false;
        }

        private bool DecrementMonth()
        {
            DateTime previous = PreviousMonth(DisplayDate);
            if (previous >= MinimumDate)
            {
                DisplayDate = previous;
                return true;
            }
            return false;
        }

        private DateTime NextMonth(DateTime date)
        {
            int year = date.Year;
            int month = date.Month + 1;
            if (month == 13)
            {
                year++;
                month = 1;
            }
            return new DateTime(year, month, 1);
        }

        private DateTime PreviousMonth(DateTime date)
        {
            int year = date.Year;
            int month = date.Month - 1;
            if (month == 0)
            {
                year--;
                month = 12;
            }
            return new DateTime(year, month, DateTime.DaysInMonth(year, month));
        }
        #endregion // end of Month Inc/Dec

        #region Year Inc/Dec
        private bool IncrementYear()
        {
            DateTime next = NextYear(DisplayDate);
            if (next <= MaximumDate)
            {
                DisplayDate = next;
                return true;
            }
            return false;
        }

        private bool DecrementYear()
        {
            DateTime previous = PreviousYear(DisplayDate);
            if (previous >= MinimumDate)
            {
                DisplayDate = previous;
                return true;
            }
            return false;
        }

        private DateTime NextYear(DateTime date)
        {
            return new DateTime(date.Year + 1, 1, 1);
        }

        private DateTime PreviousYear(DateTime date)
        {
            return new DateTime(date.Year - 1, 12, 31);
        }
        #endregion // end of Year Inc/Dec

        #region Helpers
        private int ItemColumnFromDayOfWeek(DayOfWeek dayOfWeek)
        {
            return ((int)dayOfWeek - (int)FirstDayOfWeek + 7) % 7;
        }

        private DayOfWeek DayOfWeekFromItemColumn(int column)
        {
            return (DayOfWeek)(((int)FirstDayOfWeek + column) % 7);
        }

        private IEnumerable<CalendarItem> GetItems()
        {
            for (int row = 0; row < _rowCount; row++)
            {
                for (var column = 0; column < _columnCount; column++)
                {
                    yield return _calendarItems[row, column];
                }
            }
        }

        private IEnumerable<CalendarMonthItem> GetMonthItems()
        {
            for (int row = 0; row < _monthRowCount; row++)
            {
                for (var column = 0; column < _monthColumnCount; column++)
                {
                    yield return _calendarMonthItems[row, column];
                }
            }
        }

        private GestureType GetGestureType(ManipulationCompletedEventArgs e)
        {
            if (e.IsInertial)
            {
                double horizontal = e.TotalManipulation.Translation.X;
                double vertical = e.TotalManipulation.Translation.Y;
                if (Math.Abs(horizontal) >= Math.Abs(vertical))
                {
                    if (Math.Abs(e.FinalVelocities.LinearVelocity.X) >= _flickTreshold)
                    {
                        if (horizontal > 0)
                        {
                            return GestureType.FlickLeftToRight;
                        }
                        else
                        {
                            return GestureType.FlickRightToLeft;
                        }
                    }
                }
                else
                {
                    if (Math.Abs(e.FinalVelocities.LinearVelocity.Y) >= _flickTreshold)
                    {
                        if (vertical > 0)
                        {
                            return GestureType.FlickBottomToTop;
                        }
                        else
                        {
                            return GestureType.FlickTopToBottom;
                        }
                    }
                }
            }
            return GestureType.None;
        }
        #endregion // end of Helpers

        #region Create/Build
        private void CreateDayOfWeekItems()
        {
            if (_dayOfWeekItemsGrid != null)
            {
                for (int column = 0; column < _columnCount; column++)
                {
                    _dayOfWeekItemsGrid.ColumnDefinitions.Add(new ColumnDefinition());
                }

                for (int column = 0; column < _columnCount; column++)
                {
                    CalendarDayOfWeekItem item = new CalendarDayOfWeekItem();
                    item.SetValue(Grid.ColumnProperty, column);
                    if (DayOfWeekItemTemplate != null)
                    {
                        item.Template = DayOfWeekItemTemplate;
                    }

                    _dayOfWeekItemsGrid.Children.Add(item);
                    _calendarDayOfWeekItems[column] = item;
                }
            }
        }

        private void CreateItems()
        {
            if (_itemsGrid != null)
            {
                for (int column = 0; column < _columnCount; column++)
                {
                    _itemsGrid.ColumnDefinitions.Add(new ColumnDefinition());
                }
                for (int row = 0; row < _rowCount; row++)
                {
                    _itemsGrid.RowDefinitions.Add(new RowDefinition());
                }

                for (int row = 0; row < _rowCount; row++)
                {
                    for (int column = 0; column < _columnCount; column++)
                    {
                        CalendarItem item = new CalendarItem(this);
                        item.SetValue(Grid.RowProperty, row);
                        item.SetValue(Grid.ColumnProperty, column);
                        if (ItemTemplate != null)
                        {
                            item.Template = ItemTemplate;
                        }
                        item.Tap += ItemTap;

                        _itemsGrid.Children.Add(item);
                        _calendarItems[row, column] = item;
                    }
                }
            }
        }

        private void CreateMonthItems()
        {
            if (_monthItemsGrid != null)
            {
                for (int column = 0; column < _monthColumnCount; column++)
                {
                    _monthItemsGrid.ColumnDefinitions.Add(new ColumnDefinition());
                }
                for (int row = 0; row < _monthRowCount; row++)
                {
                    _monthItemsGrid.RowDefinitions.Add(new RowDefinition());
                }

                for (int row = 0; row < _monthRowCount; row++)
                {
                    for (var column = 0; column < _monthColumnCount; column++)
                    {
                        CalendarMonthItem item = new CalendarMonthItem(this);
                        item.SetValue(Grid.RowProperty, row);
                        item.SetValue(Grid.ColumnProperty, column);
                        //if (DayOfWeekItemTemplate != null)
                        //{
                        //    item.Template = DayOfWeekItemTemplate;
                        //}
                        item.Tap += MonthItemTap;

                        _monthItemsGrid.Children.Add(item);
                        _calendarMonthItems[row, column] = item;
                    }
                }
            }
        }

        private void UpdateYearMonthLabel()
        {
            YearMonthLabel = DisplayDate.ToString("Y", DateTimeFormatInfo);
        }

        private void UpdateYearLabel()
        {
            YearLabel = DisplayDate.ToString("yyyy", DateTimeFormatInfo);
        }

        private void BuildDayOfWeekItems()
        {
            if (_dayOfWeekItemsGrid != null)
            {
                for (var column = 0; column < _columnCount; column++)
                {
                    CalendarDayOfWeekItem item = _calendarDayOfWeekItems[column];

                    item.DayOfWeek = DayOfWeekFromItemColumn(column);
                    switch (item.DayOfWeek)
                    {
                    case DayOfWeek.Monday: item.Text = Monday; break;
                    case DayOfWeek.Tuesday: item.Text = Tuesday; break;
                    case DayOfWeek.Wednesday: item.Text = Wednesday; break;
                    case DayOfWeek.Thursday: item.Text = Thursday; break;
                    case DayOfWeek.Friday: item.Text = Friday; break;
                    case DayOfWeek.Saturday: item.Text = Saturday; break;
                    case DayOfWeek.Sunday: item.Text = Sunday; break;
                    default: break;
                    }
                }
            }
        }

        private void BuildItems()
        {
            if (_itemsGrid != null)
            {
                DateTime currentMonthDate = new DateTime(DisplayDate.Year, DisplayDate.Month, 1);
                DateTime date = currentMonthDate.AddDays(-ItemColumnFromDayOfWeek(currentMonthDate.DayOfWeek));
                if (date.Day == 1)
                {
                    date = date.AddDays(-7);
                }

                foreach (CalendarItem item in GetItems())
                {
                    item.Date = date;
                    item.IsSelected = SelectedDate == date;
                    item.IsCurrentMonth = date.IsSameMonth(currentMonthDate);
                    item.IsEnabled = date >= MinimumDate && date <= MaximumDate;

                    date = date.AddDays(1);
                }
            }
        }

        private void BuildMonthItems()
        {
            if (_monthItemsGrid != null)
            {
                DateTime minimum = new DateTime(MinimumDate.Year, MinimumDate.Month, 1);
                DateTime maximum = new DateTime(MaximumDate.Year, MaximumDate.Month, DateTime.DaysInMonth(MaximumDate.Year, MaximumDate.Month));

                DateTime date = new DateTime(DisplayDate.Year, 1, 1);
                foreach (CalendarMonthItem item in GetMonthItems())
                {
                    item.Date = date;
                    item.IsSelected = date.IsSameMonth(SelectedDate);
                    item.IsEnabled = date >= minimum && date <= maximum;

                    date = date.AddMonths(1);
                }
            }
        }
        #endregion // end of Create/Build

        #endregion
    }
}
