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

namespace SimpleTasks.Views
{
    public partial class DueDatePickerPage : BasePickerPage
    {
        public DueDatePickerPage()
        {
            InitializeComponent();

            if (PhoneApplicationService.Current.State.ContainsKey("DueDate"))
            {
                initDate = (PhoneApplicationService.Current.State["DueDate"] as DateTime?) ?? DateTime.Now;
                PhoneApplicationService.Current.State.Remove("DueDate");
            }
            else
            {
                initDate = DateTime.Now; // TODO: vychozi datum podle nastaveni
            }


            Calendar.MinimumDate = initDate.AddMonths(-1);
            Calendar.MaximumDate = (initDate > DateTime.Today ? initDate : DateTime.Today).AddYears(2);

            Calendar.Sunday = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[0].ToUpper();
            Calendar.Monday = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[1].ToUpper();
            Calendar.Tuesday = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[2].ToUpper();
            Calendar.Wednesday = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[3].ToUpper();
            Calendar.Thursday = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[4].ToUpper();
            Calendar.Friday = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[5].ToUpper();
            Calendar.Saturday = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[6].ToUpper();
            Calendar.StartingDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;

            SelectDate(initDate);

            DataContext = this;
        }

        private DateTime initDate;

        protected override void Save()
        {
            PhoneApplicationService.Current.State["DueDate"] = new DateTime(
                Calendar.SelectedDate.Year,
                Calendar.SelectedDate.Month,
                Calendar.SelectedDate.Day,
                initDate.Hour,
                initDate.Minute,
                0);
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
            SelectDate(DateTimeExtensions.LastDayOfWeek);
        }

        private void NextWeek_Click(object sender, RoutedEventArgs e)
        {
            SelectDate(DateTimeExtensions.LastDayOfNextWeek);
        }

        private void SelectDate(DateTime date)
        {
            Calendar.SelectedMonth = date.Month;
            Calendar.SelectedYear = date.Year;
            Calendar.SelectedDate = date;
            Calendar.Refresh();
        }
    }
}