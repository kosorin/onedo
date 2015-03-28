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
using System.Windows.Media;
using SimpleTasks.Controls.Calendar;

namespace SimpleTasks.Views
{
    public partial class DatePickerPage : BasePickerPage
    {
        private readonly DateTime _defaultDate = DateTime.Today;
        private DateTime _date;

        public DatePickerPage()
            : base("DatePicker")
        {
            _date = NavigationParameter<DateTime?>(_name, null) ?? _defaultDate;
            if (_date == DateTime.MaxValue)
            {
                _date = _defaultDate;
            }

            InitializeComponent();
            DataContext = this;

            // Nastavení kalendáře
            SelectDate(_date);
            BackKeyPress += Calendar.BackKeyPress;
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
            Calendar.DisplayDate = Calendar.SelectedDate;
            Calendar.DisplayMode = DisplayMode.Days;
        }
    }
}