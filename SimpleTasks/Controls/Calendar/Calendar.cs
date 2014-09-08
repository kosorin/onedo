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
            SetupDaysOfWeekLabels();
        }
        #endregion

        #region Fields/Constants
        private Grid _itemsGrid;
        private CalendarItem _lastItem;
        private bool _addedDefaultItems;
        private int _month = DateTime.Today.Month;
        private int _year = DateTime.Today.Year;
        private readonly DateTimeFormatInfo _dateTimeFormatInfo;
        private bool _ignoreMonthChange;

        private const short _rowCount = 6;
        private const short _columnCount = 8;
        #endregion

        #region Events
        /// <summary>
        /// Event that occurs before month/year combination is changed
        /// </summary>
        public event EventHandler<MonthChangedEventArgs> MonthChanging;

        /// <summary>
        /// Event that occurs after month/year combination is changed
        /// </summary>
        public event EventHandler<MonthChangedEventArgs> MonthChanged;

        /// <summary>
        /// Event that occurs after a date is selected on the calendar
        /// </summary>
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

        /// <summary>
        /// Event that occurs after a date is clicked on
        /// </summary>
        public event EventHandler<SelectionChangedEventArgs> DateClicked;

        /// <summary>
        /// Raises MonthChanging event
        /// </summary>
        /// <param name="year">Year for event arguments</param>
        /// <param name="month">Month for event arguments</param>
        protected void OnMonthChanging(int year, int month)
        {
            if (MonthChanging != null)
            {
                MonthChanging(this, new MonthChangedEventArgs(year, month));
            }
        }

        /// <summary>
        /// Raises MonthChanged event
        /// </summary>
        /// <param name="year">Year for event arguments</param>
        /// <param name="month">Month for event arguments</param>
        protected void OnMonthChanged(int year, int month)
        {
            if (MonthChanged != null)
            {
                MonthChanged(this, new MonthChangedEventArgs(year, month));
            }
        }

        /// <summary>
        /// Raises SelectedChanged event
        /// </summary>
        /// <param name="dateTime">Selected date</param>
        protected void OnSelectionChanged(DateTime dateTime)
        {
            if (SelectionChanged != null)
            {
                SelectionChanged(this, new SelectionChangedEventArgs(dateTime));
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
                DateClicked(this, new SelectionChangedEventArgs(dateTime));
            }
        }
        #endregion

        #region Properties

        #region CalendarItemStyle
        /// <summary>
        /// Style for the calendar item
        /// </summary>
        public Style CalendarItemStyle
        {
            get { return (Style)GetValue(CalendarItemStyleProperty); }
            set { SetValue(CalendarItemStyleProperty, value); }
        }

        /// <summary>
        /// Style for the calendar item
        /// </summary>
        public static readonly DependencyProperty CalendarItemStyleProperty =
            DependencyProperty.Register("CalendarItemStyle", typeof(Style), typeof(Calendar), new PropertyMetadata(null));
        #endregion // end of CalendarItemStyle

        #region CalendarWeekItemStyle
        /// <summary>
        /// Style for the calendar item
        /// </summary>
        public Style CalendarWeekItemStyle
        {
            get { return (Style)GetValue(CalendarWeekItemStyleStyleProperty); }
            set { SetValue(CalendarWeekItemStyleStyleProperty, value); }
        }

        /// <summary>
        /// Style for the calendar item
        /// </summary>
        public static readonly DependencyProperty CalendarWeekItemStyleStyleProperty =
            DependencyProperty.Register("CalendarWeekItemStyle", typeof(Style), typeof(Calendar), new PropertyMetadata(null));
        #endregion // end of CalendarWeekItemStyle

        #region YearMonthLabel
        /// <summary>
        /// This value is shown in calendar header and includes month and year
        /// </summary>
        public string YearMonthLabel
        {
            get { return (string)GetValue(YearMonthLabelProperty); }
            internal set { SetValue(YearMonthLabelProperty, value); }
        }

        /// <summary>
        /// This value is shown in calendar header and includes month and year
        /// </summary>
        public static readonly DependencyProperty YearMonthLabelProperty =
            DependencyProperty.Register("YearMonthLabel", typeof(string), typeof(Calendar), new PropertyMetadata(""));
        #endregion // end of YearMonthLabel

        #region SelectedDate
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
                var newValue = (DateTime)e.NewValue;
                if (calendar._itemsGrid != null)
                {
                    var query = from oneChild in calendar._itemsGrid.Children
                                where
                                    oneChild is CalendarItem && ((CalendarItem)oneChild).IsSelected &&
                                    ((CalendarItem)oneChild).Date != newValue
                                select (CalendarItem)oneChild;
                    query.ToList().ForEach(one => one.IsSelected = false);
                }
                calendar.OnSelectionChanged(newValue);
            }
        }
        #endregion // end of SelectedDate

        #region SelectedYear/Month
        /// <summary>
        /// Currently selected year
        /// </summary>
        public int SelectedYear
        {
            get { return (int)GetValue(SelectedYearProperty); }
            set { SetValue(SelectedYearProperty, value); }
        }

        /// <summary>
        /// Currently selected year
        /// </summary>
        public static readonly DependencyProperty SelectedYearProperty =
            DependencyProperty.Register("SelectedYear", typeof(int), typeof(Calendar), new PropertyMetadata(DateTime.Today.Year, OnSelectedYearMonthChanged));

        /// <summary>
        /// Currently selected month
        /// </summary>
        public int SelectedMonth
        {
            get { return (int)GetValue(SelectedMonthProperty); }
            set { SetValue(SelectedMonthProperty, value); }
        }

        /// <summary>
        /// Currently selected month
        /// </summary>
        public static readonly DependencyProperty SelectedMonthProperty =
            DependencyProperty.Register("SelectedMonth", typeof(int), typeof(Calendar), new PropertyMetadata(DateTime.Today.Month, OnSelectedYearMonthChanged));

        private static void OnSelectedYearMonthChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var calendar = sender as Calendar;
            if (calendar != null && (calendar._year != calendar.SelectedYear || calendar._month != calendar.SelectedMonth))
            {
                if (!calendar._ignoreMonthChange)
                {
                    calendar._year = calendar.SelectedYear;
                    calendar._month = calendar.SelectedMonth;
                    calendar.SetYearMonthLabel();
                }
            }
        }
        #endregion // end of SelectedMonth/Month

        #region ColorConverter
        /// <summary>
        /// This converter is used to dynamically color the background or day number of a calendar cell
        /// based on date and the fact that a date is selected and type of conversion
        /// </summary>
        public IDateToBrushConverter ColorConverter
        {
            get { return (IDateToBrushConverter)GetValue(ColorConverterProperty); }
            set { SetValue(ColorConverterProperty, value); }
        }

        /// <summary>
        /// This converter is used to dynamically color the background of a calendar cell
        /// based on date and the fact that a date is selected
        /// </summary>
        public static readonly DependencyProperty ColorConverterProperty =
            DependencyProperty.Register("ColorConverter", typeof(IDateToBrushConverter), typeof(Calendar), new PropertyMetadata(null));
        #endregion // end of ColorConverter

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

        #region ShowSelectedDate
        /// <summary>
        /// If set to false, selected date is not highlighted
        /// </summary>
        public bool ShowSelectedDate
        {
            get { return (bool)GetValue(ShowSelectedDateProperty); }
            set { SetValue(ShowSelectedDateProperty, value); }
        }

        /// <summary>
        /// If set to false, selected date is not highlighted
        /// </summary>
        public static readonly DependencyProperty ShowSelectedDateProperty =
            DependencyProperty.Register("ShowSelectedDate", typeof(bool), typeof(Calendar), new PropertyMetadata(true));
        #endregion // end of ShowSelectedDate

        #region WeekNumberDisplay
        /// <summary>
        /// Sets an option of how to display week number
        /// </summary>
        public WeekNumberDisplayOption WeekNumberDisplay
        {
            get { return (WeekNumberDisplayOption)GetValue(WeekNumberDisplayProperty); }
            set { SetValue(WeekNumberDisplayProperty, value); }
        }

        /// <summary>
        /// If set to false, selected date is not highlighted
        /// </summary>
        public static readonly DependencyProperty WeekNumberDisplayProperty =
            DependencyProperty.Register("WeekNumberDisplay", typeof(WeekNumberDisplayOption), typeof(Calendar),
            new PropertyMetadata(WeekNumberDisplayOption.None, OnWeekNumberDisplayChanged));

        /// <summary>
        /// Update calendar display when display option changes
        /// </summary>
        /// <param name="sender">Calendar control</param>
        /// <param name="e">Event arguments</param>
        public static void OnWeekNumberDisplayChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((Calendar)sender).BuildItems();
        }
        #endregion // end of WeekNumberDisplay

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

        #region FirstDayOfWeek
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
            ((Calendar)sender).SetupDayLabels();
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
            var previousButton = GetTemplateChild("PreviousMonthButton") as Button;
            if (previousButton != null) previousButton.Click += PreviousButtonClick;
            var nextButton = GetTemplateChild("NextMonthButton") as Button;
            if (nextButton != null) nextButton.Click += NextButtonClick;
            _itemsGrid = GetTemplateChild("ItemsGrid") as Grid;
            SetupDayLabels();
            SetYearMonthLabel();
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
            if (_lastItem != null)
            {
                _lastItem.IsSelected = false;
            }
            _lastItem = (sender as CalendarItem);
            if (_lastItem != null)
            {
                if (ShowSelectedDate)
                    _lastItem.IsSelected = true;
                SelectedDate = _lastItem.Date;
                OnDateClicked(_lastItem.Date);
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

        public void GoTo(int year, int month)
        {
            SelectedMonth = month;
            SelectedYear = year;
            Refresh();
        }
        #endregion // end of Public Methods

        #region Private Methods
        private void IncrementMonth()
        {
            if (CanMoveToMonthYear(_year, _month + 1))
            {
                _month += 1;
                if (_month == 13)
                {
                    _month = 1;
                    _year += 1;
                }
                SetYearMonthLabel();
            }
        }

        private void DecrementMonth()
        {
            if (CanMoveToMonthYear(_year, _month - 1))
            {
                _month -= 1;
                if (_month == 0)
                {
                    _month = 12;
                    _year -= 1;
                }
                SetYearMonthLabel();
            }
        }

        private bool CanMoveToMonthYear(int year, int month)
        {
            var returnValue = false;
            if (month == 0)
            {
                year = year - 1;
                month = 12;
            }
            else if (month == 13)
            {
                month = 1;
                year = year + 1;
            }
            var testDate = new DateTime(year, month, 1);
            if (testDate >= MinimumDate && testDate <= MaximumDate)
            {
                returnValue = true;
            }
            return returnValue;
        }

        private int DayColumnOffsetFromSunday()
        {
            switch (FirstDayOfWeek)
            {
            case DayOfWeek.Monday:
                return -1;
            case DayOfWeek.Tuesday:
                return -2;
            case DayOfWeek.Wednesday:
                return -3;
            case DayOfWeek.Thursday:
                return -4;
            case DayOfWeek.Friday:
                return -5;
            case DayOfWeek.Saturday:
                return -6;
            case DayOfWeek.Sunday:
                return 0;
            default:
                throw new ArgumentOutOfRangeException();
            }
        }

        private int DefaultDayColumnIndex(DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
            case DayOfWeek.Sunday:
                return 1;
            case DayOfWeek.Monday:
                return 2;
            case DayOfWeek.Tuesday:
                return 3;
            case DayOfWeek.Wednesday:
                return 4;
            case DayOfWeek.Thursday:
                return 5;
            case DayOfWeek.Friday:
                return 6;
            case DayOfWeek.Saturday:
                return 7;
            default:
                throw new ArgumentOutOfRangeException();
            }
        }

        private void SetupDayLabels()
        {
            if (FirstDayOfWeek != DayOfWeek.Sunday && _itemsGrid != null)
            {
                var offset = DayColumnOffsetFromSunday();
                var labels = _itemsGrid.Children.Where(one => one.GetValue(Grid.RowProperty).Equals(0));
                foreach (var label in labels)
                {
                    var textBlock = label as TextBlock;
                    if (textBlock != null)
                    {
                        if (!string.IsNullOrEmpty(textBlock.Text))
                        {
                            int column = 0;
                            if (textBlock.Text == Sunday)
                            {
                                column = DefaultDayColumnIndex(DayOfWeek.Sunday) + offset;
                            }
                            if (textBlock.Text == Monday)
                            {
                                column = DefaultDayColumnIndex(DayOfWeek.Monday) + offset;
                            }
                            if (textBlock.Text == Tuesday)
                            {
                                column = DefaultDayColumnIndex(DayOfWeek.Tuesday) + offset;
                            }
                            if (textBlock.Text == Wednesday)
                            {
                                column = DefaultDayColumnIndex(DayOfWeek.Wednesday) + offset;
                            }
                            if (textBlock.Text == Thursday)
                            {
                                column = DefaultDayColumnIndex(DayOfWeek.Thursday) + offset;
                            }
                            if (textBlock.Text == Friday)
                            {
                                column = DefaultDayColumnIndex(DayOfWeek.Friday) + offset;
                            }
                            if (textBlock.Text == Saturday)
                            {
                                column = DefaultDayColumnIndex(DayOfWeek.Saturday) + offset;
                            }
                            if (column <= 0)
                            {
                                column = column + 7;
                            }
                            textBlock.SetValue(Grid.ColumnProperty, column);
                        }
                    }
                }
            }
        }

        private void SetupDaysOfWeekLabels()
        {
            Sunday = _dateTimeFormatInfo.AbbreviatedDayNames[0].ToUpper();
            Monday = _dateTimeFormatInfo.AbbreviatedDayNames[1].ToUpper();
            Tuesday = _dateTimeFormatInfo.AbbreviatedDayNames[2].ToUpper();
            Wednesday = _dateTimeFormatInfo.AbbreviatedDayNames[3].ToUpper();
            Thursday = _dateTimeFormatInfo.AbbreviatedDayNames[4].ToUpper();
            Friday = _dateTimeFormatInfo.AbbreviatedDayNames[5].ToUpper();
            Saturday = _dateTimeFormatInfo.AbbreviatedDayNames[6].ToUpper();
        }

        private void SetYearMonthLabel()
        {
            OnMonthChanging(_year, _month);
            YearMonthLabel = new DateTime(_year, _month, 1).ToString("Y", _dateTimeFormatInfo);
            _ignoreMonthChange = true;
            SelectedMonth = _month;
            SelectedYear = _year;
            _ignoreMonthChange = false;
            BuildItems();
            OnMonthChanged(_year, _month);
        }

        private void AddDefaultItems()
        {
            if (!_addedDefaultItems && _itemsGrid != null)
            {
                for (int row = 1; row <= _rowCount; row++)
                {
                    for (int column = 1; column < _columnCount; column++)
                    {
                        var item = new CalendarItem(this);
                        item.SetValue(Grid.RowProperty, row);
                        item.SetValue(Grid.ColumnProperty, column);
                        item.Visibility = Visibility.Collapsed;
                        item.Tag = string.Concat(row.ToString(CultureInfo.InvariantCulture), ":", column.ToString(CultureInfo.InvariantCulture));
                        item.Click += ItemClick;
                        if (CalendarItemStyle != null)
                        {
                            item.Style = CalendarItemStyle;
                        }
                        _itemsGrid.Children.Add(item);
                    }
                    if (WeekNumberDisplay != WeekNumberDisplayOption.None)
                    {
                        const int columnCount = 0;
                        var item = new CalendarWeekItem();
                        item.SetValue(Grid.RowProperty, row);
                        item.SetValue(Grid.ColumnProperty, columnCount);
                        item.Visibility = Visibility.Collapsed;
                        item.Tag = string.Concat(row.ToString(CultureInfo.InvariantCulture), ":", columnCount.ToString(CultureInfo.InvariantCulture));
                        if (CalendarWeekItemStyle != null)
                        {
                            item.Style = CalendarWeekItemStyle;
                        }
                        _itemsGrid.Children.Add(item);
                    }
                }
                _addedDefaultItems = true;
            }
        }

        private void BuildItems()
        {
            Debug.WriteLine("> BuildItems ({0})", _itemsGrid);
            if (_itemsGrid != null)
            {
                AddDefaultItems();
                var startOfMonth = new DateTime(_year, _month, 1);
                DayOfWeek dayOfWeek = startOfMonth.DayOfWeek;
                int startColumn = DefaultDayColumnIndex(dayOfWeek);
                if (FirstDayOfWeek != DayOfWeek.Sunday)
                {
                    startColumn = startColumn + DayColumnOffsetFromSunday();
                    if (startColumn <= 0)
                    {
                        startColumn += 7;
                    }
                }
                var daysInMonth = (int)Math.Floor(startOfMonth.AddMonths(1).Subtract(startOfMonth).TotalDays);
                var addedDays = 0;
                int lastWeekNumber = 0;
                for (int rowCount = 1; rowCount <= _rowCount; rowCount++)
                {
                    for (var columnCount = 1; columnCount < _columnCount; columnCount++)
                    {
                        var item = (CalendarItem)(from oneChild in _itemsGrid.Children
                                                  where oneChild is CalendarItem &&
                                                  ((CalendarItem)oneChild).Tag.ToString() == string.Concat(rowCount.ToString(CultureInfo.InvariantCulture), ":", columnCount.ToString(CultureInfo.InvariantCulture))
                                                  select oneChild).First();
                        if (rowCount == 1 && columnCount < startColumn)
                        {
                            item.Visibility = Visibility.Collapsed;
                        }
                        else if (addedDays < daysInMonth)
                        {
                            item.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            item.Visibility = Visibility.Collapsed;
                        }

                        var weekItem = (CalendarWeekItem)(from oneChild in _itemsGrid.Children
                                                          where oneChild is CalendarWeekItem &&
                                                          ((CalendarWeekItem)oneChild).Tag.ToString() == string.Concat(rowCount.ToString(CultureInfo.InvariantCulture), ":0")
                                                          select oneChild).FirstOrDefault();

                        if (item.Visibility == Visibility.Visible)
                        {
                            item.Date = startOfMonth.AddDays(addedDays);
                            if (SelectedDate == DateTime.MinValue && item.Date == DateTime.Today)
                            {
                                SelectedDate = item.Date;
                                if (ShowSelectedDate)
                                    item.IsSelected = true;
                                _lastItem = item;
                            }
                            else
                            {
                                if (item.Date == SelectedDate)
                                {
                                    if (ShowSelectedDate)
                                        item.IsSelected = true;
                                }
                                else
                                {
                                    item.IsSelected = false;
                                }
                            }
                            addedDays += 1;
                            item.DayNumber = addedDays;
                            item.SetBackgroundColor();
                            item.SetForegroundColor();

                            if (WeekNumberDisplay != WeekNumberDisplayOption.None)
                            {
                                int weekNumber;

                                if (WeekNumberDisplay == WeekNumberDisplayOption.WeekOfYear)
                                {
                                    var systemCalendar = System.Threading.Thread.CurrentThread.CurrentCulture.Calendar;
                                    weekNumber = systemCalendar.GetWeekOfYear(
                                        item.Date,
                                        System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.CalendarWeekRule,
                                        System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.FirstDayOfWeek);
                                }
                                else
                                {
                                    weekNumber = rowCount;
                                }
                                if (weekItem != null)
                                {
                                    weekItem.WeekNumber = weekNumber;
                                    lastWeekNumber = weekNumber;
                                    weekItem.Visibility = Visibility.Visible;
                                }
                            }
                        }
                        else
                        {
                            if (WeekNumberDisplay != WeekNumberDisplayOption.None && weekItem != null && weekItem.WeekNumber != lastWeekNumber)
                            {
                                weekItem.Visibility = Visibility.Collapsed;
                            }
                        }
                    }
                }
            }
        }
        #endregion
    }
}
