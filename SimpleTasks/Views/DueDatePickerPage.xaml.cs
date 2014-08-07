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

namespace SimpleTasks.Views
{
    public class CalendarColorConverter : IDateToBrushConverter
    {
        public CalendarColorConverter()
        {
            DefaultForeground = Application.Current.Resources["CalendarItemBrush"] as Brush;
            InactiveForeground = Application.Current.Resources["CalendarItemSubtleBrush"] as Brush;
        }

        public Brush DefaultForeground { get; set; }

        public Brush InactiveForeground { get; set; }

        public Brush Convert(DateTime dateTime, bool isSelected, Brush defaultValue, BrushType brushType)
        {
            if (brushType == BrushType.Foreground)
            {
                if (dateTime.Date < DateTime.Today && !isSelected)
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

    public partial class DueDatePickerPage : BasePickerPage
    {
        private readonly DateTime _defaultDate = DateTime.Today;
        private DateTime _date;

        public DueDatePickerPage()
        {
            _date = RetrieveAndConfigure<DateTime?>("DueDate") ?? _defaultDate;
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

        protected override void Save()
        {
            SetValueToSave(new DateTime(
                Calendar.SelectedDate.Year,
                Calendar.SelectedDate.Month,
                Calendar.SelectedDate.Day,
                _date.Hour,
                _date.Minute,
                0));
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