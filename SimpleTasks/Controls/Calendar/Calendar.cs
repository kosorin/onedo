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
using System.Diagnostics;

namespace SimpleTasks.Controls.Calendar
{
    /// <summary>
    /// Calendar control for Windows Phone 7
    /// </summary>
    public class Calendar : Control
    {
        #region Constructor
        /// <summary>
        /// Create new instance of a calendar
        /// </summary>
        public Calendar()
        {
            DefaultStyleKey = typeof(Calendar);

            _dateTimeFormatInfo = CultureInfo.CurrentCulture.DateTimeFormat;
            FirstDayOfWeek = _dateTimeFormatInfo.FirstDayOfWeek;

            SetDefaultDayOfWeekLabels();
        }
        #endregion

        #region Private Fields/Constants
        private Grid _itemsGrid;
        private Grid _dayOfWeekItemsGrid;
        private readonly DateTimeFormatInfo _dateTimeFormatInfo;

        private const short _columnCount = 7;
        private const short _rowCount = 6;
        private CalendarDayOfWeekItem[] _calendarDayOfWeekItems = new CalendarDayOfWeekItem[_columnCount];
        private CalendarItem[,] _calendarItems = new CalendarItem[_rowCount, _columnCount];
        #endregion

        #region Events
        public event EventHandler<CurrentDateChangedEventArgs> CurrentDateChanged;

        /// <summary>
        /// Event that occurs after a date is selected on the calendar
        /// </summary>
        public event EventHandler<SelectedDateChangedEventArgs> SelectedDateChanged;

        /// <summary>
        /// Event that occurs after a date is clicked on
        /// </summary>
        public event EventHandler<SelectedDateChangedEventArgs> DateClicked;

        protected void OnCurrentDateChanged(DateTime date)
        {
            if (CurrentDateChanged != null)
            {
                CurrentDateChanged(this, new CurrentDateChangedEventArgs(date));
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
                SelectedDateChanged(this, new SelectedDateChangedEventArgs(dateTime));
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
                DateClicked(this, new SelectedDateChangedEventArgs(dateTime));
            }
        }
        #endregion

        #region Properties

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
                if (date == calendar.CurrentDate)
                {
                    if (calendar._itemsGrid != null)
                    {
                        foreach (CalendarItem item in calendar.GetItems())
                        {
                            item.IsSelected = item.Date == date;
                        }
                    }
                }
                else
                {
                    calendar.CurrentDate = date;
                }
                calendar.OnSelectedDateChanged(date);
            }
        }
        #endregion // end of SelectedDate

        #region CurrentDate +
        public DateTime CurrentDate
        {
            get { return (DateTime)GetValue(CurrentDateProperty); }
            set { SetValue(CurrentDateProperty, value); }
        }
        public static readonly DependencyProperty CurrentDateProperty =
            DependencyProperty.Register("CurrentDate", typeof(DateTime), typeof(Calendar), new PropertyMetadata(DateTime.Today, OnCurrentDateChanged));

        private static void OnCurrentDateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var calendar = sender as Calendar;
            if (calendar != null)
            {
                DateTime currentDate = (DateTime)e.NewValue;
                calendar.UpdateYearMonthLabel();
                calendar.BuildItems();
                calendar.OnCurrentDateChanged(currentDate);
            }
        }
        #endregion // end of CurrentDate

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
            ((Calendar)sender).BuildDayOfWeekItems();
            ((Calendar)sender).BuildItems();
        }
        #endregion // end of FirstDayOfWeek

        #endregion

        #region Template
        /// <summary>
        /// Apply default template and perform initialization
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Tlačítka
            var previousButton = GetTemplateChild("PreviousMonthButton") as Button;
            if (previousButton != null) previousButton.Click += PreviousButtonClick;
            var nextButton = GetTemplateChild("NextMonthButton") as Button;
            if (nextButton != null) nextButton.Click += NextButtonClick;

            // Items Grid 
            _dayOfWeekItemsGrid = GetTemplateChild("DayOfWeekItemsGrid") as Grid;
            _itemsGrid = GetTemplateChild("ItemsGrid") as Grid;

            CreateGrids();
            CreateDayOfWeekItems();
            CreateItems();

            UpdateYearMonthLabel();
            BuildDayOfWeekItems();
            BuildItems();
        }
        #endregion

        #region Event handling
        private void NextButtonClick(object sender, RoutedEventArgs e)
        {
            IncrementMonth();
        }

        private void PreviousButtonClick(object sender, RoutedEventArgs e)
        {
            DecrementMonth();
        }

        private void ItemClick(object sender, RoutedEventArgs e)
        {
            CalendarItem item = (sender as CalendarItem);
            if (item != null)
            {
                SelectedDate = item.Date;
                OnDateClicked(item.Date);
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Explicitly refresh the calendar
        /// </summary>
        public void Refresh()
        {
            BuildItems();
        }
        #endregion // end of Public Methods

        #region Private Methods
        private void IncrementMonth()
        {
            DateTime next = NextMonth(CurrentDate);
            if (next <= MaximumDate)
            {
                CurrentDate = next;
            }
        }

        private void DecrementMonth()
        {
            DateTime previous = PreviousMonth(CurrentDate);
            if (previous >= MinimumDate)
            {
                CurrentDate = previous;
            }
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

        private int ColumnFromDayOfWeek(DayOfWeek dayOfWeek)
        {
            return ((int)dayOfWeek - (int)FirstDayOfWeek + 7) % 7;
        }

        private DayOfWeek DayOfWeekFromColumn(int column)
        {
            return (DayOfWeek)(((int)FirstDayOfWeek + column) % 7);
        }

        private void SetDefaultDayOfWeekLabels()
        {
            Sunday = _dateTimeFormatInfo.AbbreviatedDayNames[0].ToUpper();
            Monday = _dateTimeFormatInfo.AbbreviatedDayNames[1].ToUpper();
            Tuesday = _dateTimeFormatInfo.AbbreviatedDayNames[2].ToUpper();
            Wednesday = _dateTimeFormatInfo.AbbreviatedDayNames[3].ToUpper();
            Thursday = _dateTimeFormatInfo.AbbreviatedDayNames[4].ToUpper();
            Friday = _dateTimeFormatInfo.AbbreviatedDayNames[5].ToUpper();
            Saturday = _dateTimeFormatInfo.AbbreviatedDayNames[6].ToUpper();
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

        private void CreateGrids()
        {
            if (_dayOfWeekItemsGrid != null)
            {
                for (int column = 0; column < _columnCount; column++)
                {
                    _dayOfWeekItemsGrid.ColumnDefinitions.Add(new ColumnDefinition());
                }
            }

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
            }
        }

        private void CreateDayOfWeekItems()
        {
            if (_dayOfWeekItemsGrid != null)
            {
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
                        item.Click += ItemClick;

                        _itemsGrid.Children.Add(item);
                        _calendarItems[row, column] = item;
                    }
                }
            }
        }

        private void UpdateYearMonthLabel()
        {
            YearMonthLabel = CurrentDate.ToString("Y", _dateTimeFormatInfo);
        }

        private void BuildDayOfWeekItems()
        {
            if (_dayOfWeekItemsGrid != null)
            {
                for (var column = 0; column < _columnCount; column++)
                {
                    CalendarDayOfWeekItem item = _calendarDayOfWeekItems[column];

                    item.DayOfWeek = DayOfWeekFromColumn(column);
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
                DateTime currentMonthDate = new DateTime(CurrentDate.Year, CurrentDate.Month, 1);
                DateTime date = currentMonthDate.AddDays(-ColumnFromDayOfWeek(currentMonthDate.DayOfWeek));
                if (date.Day == 1)
                {
                    date = date.AddDays(-7);
                }

                foreach (CalendarItem item in GetItems())
                {
                    item.Date = date;
                    item.IsSelected = SelectedDate == date;
                    item.IsCurrentMonth = date.Month == currentMonthDate.Month;
                    item.IsEnabled = date >= MinimumDate && date <= MaximumDate;

                    date = date.AddDays(1);
                }
            }
        }
        #endregion
    }
}
