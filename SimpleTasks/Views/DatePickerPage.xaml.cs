using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SimpleTasks.Controls;
using System.Globalization;
using System.Diagnostics;
using SimpleTasks.Core.Helpers;
using WPControls;
using System.Windows.Media;
using SimpleTasks.Helpers.Analytics;

namespace SimpleTasks.Views
{
    public class CalendarColorConverter : IDateToBrushConverter
    {
        private static Brush _defaultForeground = null;
        public static Brush DefaultForeground
        {
            get
            {
                if (_defaultForeground == null)
                    _defaultForeground = Application.Current.Resources["CalendarItemBrush"] as Brush;
                return _defaultForeground;
            }
        }

        private static Brush _inactiveForeground = null;
        public static Brush InactiveForeground
        {
            get
            {
                if (_inactiveForeground == null)
                    _inactiveForeground = Application.Current.Resources["CalendarItemSubtleBrush"] as Brush;
                return _inactiveForeground;
            }
        }

        private static Brush _selectedForeground = null;
        public static Brush SelectedForeground
        {
            get
            {
                if (_selectedForeground == null)
                    _selectedForeground = Application.Current.Resources["CalendarSelectedItemBrush"] as Brush;
                return _selectedForeground;
            }
        }

        private static Brush _selectedBackground = null;
        public static Brush SelectedBackground
        {
            get
            {
                if (_selectedBackground == null)
                    _selectedBackground = Application.Current.Resources["PhoneAccentBrush"] as Brush;
                return _selectedBackground;
            }
        }

        public Brush Convert(DateTime dateTime, bool isSelected, Brush defaultValue, BrushType brushType)
        {
            if (isSelected)
            {
                if (brushType == BrushType.Background)
                    return SelectedBackground;
                else if (brushType == BrushType.Foreground)
                    return SelectedForeground;
            }

            if (brushType == BrushType.Foreground)
            {
                if (dateTime.Date < DateTime.Today)
                {
                    return InactiveForeground;
                }
                else
                {
                    return DefaultForeground;
                }
            }
            return defaultValue;
        }
    }

    public partial class DatePickerPage : BasePickerPage
    {
        private readonly DateTime _defaultDate = DateTime.Today;
        private DateTime _date;

        public DatePickerPage()
            : base("DatePicker")
        {
            _date = NavigationParameter<DateTime?>(_name) ?? _defaultDate;
            if (_date == DateTime.MaxValue)
            {
                _date = _defaultDate;
            }

            InitializeComponent();
            DataContext = this;

            Calendar.ColorConverter = new CalendarColorConverter();

            Calendar.MinimumDate = (_date < DateTime.Today ? _date : DateTime.Today).AddMonths(-1);
            Calendar.MaximumDate = (_date > DateTime.Today ? _date : DateTime.Today).AddYears(2);

            Calendar.Sunday = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[0].ToUpper();
            Calendar.Monday = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[1].ToUpper();
            Calendar.Tuesday = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[2].ToUpper();
            Calendar.Wednesday = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[3].ToUpper();
            Calendar.Thursday = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[4].ToUpper();
            Calendar.Friday = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[5].ToUpper();
            Calendar.Saturday = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[6].ToUpper();
            Calendar.StartingDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;

            SelectDate(_date);
        }

        protected override object Save()
        {
            DateTime date = new DateTime(
                Calendar.SelectedDate.Year,
                Calendar.SelectedDate.Month,
                Calendar.SelectedDate.Day,
                _date.Hour,
                _date.Minute,
                0);

            int days = (int)(date.Date - DateTime.Today).TotalDays;
            if (days > 0)
            {
                GoogleAnalyticsHelper.SetDimension(CustomDimension.TaskRelativeDate, days.ToString());
                GoogleAnalyticsHelper.SendEvent(EventCategory.Tasks, EventAction.Edit, "edit task date");
            }
            return date;
        }

        private void Today_Click(object sender, RoutedEventArgs e)
        {
            SelectDate(DateTimeExtensions.Today);
        }

        private void Tomorrow_Click(object sender, RoutedEventArgs e)
        {
            SelectDate(DateTimeExtensions.Tomorrow);
        }

        private void ThisWeek_Click(object sender, RoutedEventArgs e)
        {
            SelectDate(DateTimeExtensions.LastDayOfActualWeek);
        }

        private void NextWeek_Click(object sender, RoutedEventArgs e)
        {
            SelectDate(DateTimeExtensions.LastDayOfNextWeek);
        }

        public void SelectDate(DateTime date)
        {
            SelectDate(date.Year, date.Month, date.Day);
        }

        public void SelectDate(int year, int month, int day)
        {
            Calendar.SelectedDate = new DateTime(year, month, day);
            Calendar.SelectedMonth = month;
            Calendar.SelectedYear = year;
            Calendar.Refresh();
        }
    }
}