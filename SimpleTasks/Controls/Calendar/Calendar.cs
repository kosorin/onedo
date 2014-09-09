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
        private CalendarItem _lastItem;
        private int _month = DateTime.Today.Month;
        private int _year = DateTime.Today.Year;
        private readonly DateTimeFormatInfo _dateTimeFormatInfo;
        private bool _ignoreMonthChange;

        private const short _columnCount = 7;
        private const short _rowCount = 6;
        private CalendarDayOfWeekItem[] _calendarDayOfWeekItems = new CalendarDayOfWeekItem[_columnCount];
        private CalendarItem[,] _calendarItems = new CalendarItem[_rowCount, _columnCount];
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

        #region ItemStyle
        public Style ItemStyle
        {
            get { return (Style)GetValue(ItemStyleProperty); }
            set { SetValue(ItemStyleProperty, value); }
        }

        public static readonly DependencyProperty ItemStyleProperty =
            DependencyProperty.Register("ItemStyle", typeof(Style), typeof(Calendar), new PropertyMetadata(null));
        #endregion // end of ItemStyle

        #region DayOfWeekItemStyle
        public Style DayOfWeekItemStyle
        {
            get { return (Style)GetValue(DayOfWeekItemStyleProperty); }
            set { SetValue(DayOfWeekItemStyleProperty, value); }
        }

        public static readonly DependencyProperty DayOfWeekItemStyleProperty =
            DependencyProperty.Register("DayOfWeekItemStyle", typeof(Style), typeof(Calendar), null);
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
                    foreach (CalendarItem item in calendar.ItemValues())
                    {
                        if (item.IsSelected && item.Date != newValue)
                        {
                            item.IsSelected = false;
                        }
                    }
                }
                calendar.OnSelectionChanged(newValue);
            }
        }
        #endregion // end of SelectedDate

        #region Current Year/Month
        /// <summary>
        /// Currently selected year
        /// </summary>
        public int CurrentYear
        {
            get { return (int)GetValue(CurrentYearProperty); }
            set { SetValue(CurrentYearProperty, value); }
        }

        /// <summary>
        /// Currently selected year
        /// </summary>
        public static readonly DependencyProperty CurrentYearProperty =
            DependencyProperty.Register("CurrentYear", typeof(int), typeof(Calendar), new PropertyMetadata(DateTime.Today.Year, OnCurrentYearMonthChanged));

        /// <summary>
        /// Currently selected month
        /// </summary>
        public int CurrentMonth
        {
            get { return (int)GetValue(CurrentMonthProperty); }
            set { SetValue(CurrentMonthProperty, value); }
        }

        /// <summary>
        /// Currently selected month
        /// </summary>
        public static readonly DependencyProperty CurrentMonthProperty =
            DependencyProperty.Register("CurrentMonth", typeof(int), typeof(Calendar), new PropertyMetadata(DateTime.Today.Month, OnCurrentYearMonthChanged));

        private static void OnCurrentYearMonthChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var calendar = sender as Calendar;
            if (calendar != null && (calendar._year != calendar.CurrentYear || calendar._month != calendar.CurrentMonth))
            {
                if (!calendar._ignoreMonthChange)
                {
                    calendar._year = calendar.CurrentYear;
                    calendar._month = calendar.CurrentMonth;
                    calendar.SetYearMonthLabel();
                }
            }
        }
        #endregion // end of CurrentMonth/Month

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

            var previousButton = GetTemplateChild("PreviousMonthButton") as Button;
            if (previousButton != null) previousButton.Click += PreviousButtonClick;
            var nextButton = GetTemplateChild("NextMonthButton") as Button;
            if (nextButton != null) nextButton.Click += NextButtonClick;
            _itemsGrid = GetTemplateChild("ItemsGrid") as Grid;

            CreateDayOfWeekItems();
            CreateItems();

            BuildDayOfWeekItems();
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
            CurrentMonth = month;
            CurrentYear = year;
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

        private void SetYearMonthLabel()
        {
            OnMonthChanging(_year, _month);
            YearMonthLabel = new DateTime(_year, _month, 1).ToString("Y", _dateTimeFormatInfo);
            _ignoreMonthChange = true;
            CurrentMonth = _month;
            CurrentYear = _year;
            _ignoreMonthChange = false;
            BuildItems();
            OnMonthChanged(_year, _month);
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

        private IEnumerable<CalendarItem> ItemValues()
        {
            for (int row = 0; row < _rowCount; row++)
            {
                for (var column = 0; column < _columnCount; column++)
                {
                    yield return _calendarItems[row, column];
                }
            }
        }

        private void CreateDayOfWeekItems()
        {
            if (_itemsGrid != null)
            {
                for (int column = 0; column < _columnCount; column++)
                {
                    CalendarDayOfWeekItem item = new CalendarDayOfWeekItem();
                    item.SetValue(Grid.RowProperty, 0);
                    item.SetValue(Grid.ColumnProperty, column);
                    if (DayOfWeekItemStyle != null)
                    {
                        item.Style = DayOfWeekItemStyle;
                    }

                    _itemsGrid.Children.Add(item);
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
                        item.SetValue(Grid.RowProperty, row + 1); // +1, protože 0 zabírá řádek pro názvy dnů
                        item.SetValue(Grid.ColumnProperty, column);
                        if (ItemStyle != null)
                        {
                            item.Style = ItemStyle;
                        }
                        item.Click += ItemClick;

                        _itemsGrid.Children.Add(item);
                        _calendarItems[row, column] = item;
                    }
                }
            }
        }

        private void BuildDayOfWeekItems()
        {
            if (_itemsGrid != null)
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
                DateTime currentMonth = new DateTime(_year, _month, 1);
                DateTime currentDate = currentMonth.AddDays(-ColumnFromDayOfWeek(currentMonth.DayOfWeek));
                if (currentDate.Day == 1)
                {
                    currentDate = currentDate.AddDays(-7);
                }

                foreach (CalendarItem item in ItemValues())
                {
                    item.Date = currentDate;
                    item.DayNumber = currentDate.Day;
                    item.IsSelected = ShowSelectedDate && SelectedDate == currentDate;
                    item.Opacity = (currentDate.Month == currentMonth.Month) ? 1.0 : 0.3;

                    currentDate = currentDate.AddDays(1);
                }
            }
        }
        #endregion
    }
}
