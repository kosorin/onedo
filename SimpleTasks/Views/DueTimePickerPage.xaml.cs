using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using SimpleTasks.ViewModels;
using Microsoft.Phone.Shell;
using SimpleTasks.Resources;
using SimpleTasks.Controls;

namespace SimpleTasks.Views
{
    public partial class DueTimePickerPage : BasePickerPage
    {
        public DueTimePickerPage()
        {
            InitializeComponent();

            if (PhoneApplicationService.Current.State.ContainsKey("DueTime"))
            {
                initTime = (PhoneApplicationService.Current.State["DueTime"] as DateTime?) ?? DateTime.Now;
                PhoneApplicationService.Current.State.Remove("DueTime");
            }
            else
            {
                initTime = DateTime.Now;
            }

            TimePicker.SetTime(initTime);
            DataContext = this;
        }

        private DateTime initTime;

        protected override void Save()
        {
            PhoneApplicationService.Current.State["DueTime"] = new DateTime(
                initTime.Year,
                initTime.Month,
                initTime.Day,
                TimePicker.Hours,
                TimePicker.Minutes,
                0);
        }

        private void Morning_Click(object sender, RoutedEventArgs e)
        {
            TimePicker.SetTime(7, 0);
        }

        private void LateMorning_Click(object sender, RoutedEventArgs e)
        {
            TimePicker.SetTime(10, 0);
        }

        private void Noon_Click(object sender, RoutedEventArgs e)
        {
            TimePicker.SetTime(12, 0);
        }

        private void Afternoon_Click(object sender, RoutedEventArgs e)
        {
            TimePicker.SetTime(15, 0);
        }

        private void Evening_Click(object sender, RoutedEventArgs e)
        {
            TimePicker.SetTime(19, 0);
        }

        private void Night_Click(object sender, RoutedEventArgs e)
        {
            TimePicker.SetTime(22, 0);
        }
    }
}