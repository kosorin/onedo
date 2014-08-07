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
        private readonly DateTime _defaultTime = DateTime.Now;
        private DateTime _time;

        public DueTimePickerPage()
        {
            _time = RetrieveAndConfigure<DateTime?>("DueTime") ?? _defaultTime;

            InitializeComponent();
            TimePicker.SetTime(_time);
            DataContext = this;
        }

        protected override void Save()
        {
            SetValueToSave(new DateTime(
                _time.Year,
                _time.Month,
                _time.Day,
                TimePicker.Hours,
                TimePicker.Minutes,
                0));
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