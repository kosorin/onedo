using System;
using System.Windows;
using SimpleTasks.Controls;

namespace SimpleTasks.Views
{
    public partial class TimePickerPage : BasePickerPage
    {
        private readonly DateTime _defaultTime = DateTime.Now;
        private DateTime _time;

        public TimePickerPage()
            : base("TimePicker")
        {
            _time = NavigationParameter<DateTime?>(_name, null) ?? _defaultTime;

            InitializeComponent();
            TimePicker.SetTime(_time);
            DataContext = this;
        }

        protected override object Save()
        {
            DateTime time = new DateTime(
                _time.Year,
                _time.Month,
                _time.Day,
                TimePicker.Hours,
                TimePicker.Minutes,
                0);

            return time;
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