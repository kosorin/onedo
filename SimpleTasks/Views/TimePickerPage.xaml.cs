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
using SimpleTasks.Helpers.Analytics;

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

            if (PhoneApplicationService.Current.State.ContainsKey("GA_TimePicker"))
            {
                // 0 - task time
                // 1 - default task time (zatím nepoužito)
                int type = (int)PhoneApplicationService.Current.State["GA_TimePicker"];
                PhoneApplicationService.Current.State.Remove("GA_TimePicker");
                if (type == 0)
                {
                    GoogleAnalyticsHelper.SetDimension(CustomDimension.TaskTime, string.Format("{0}:{1:00}", time.Hour, time.Minute));
                    GoogleAnalyticsHelper.SendEvent(EventCategory.Tasks, EventAction.Edit, "edit task time");
                }
            }

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