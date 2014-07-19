using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.LocalizedResources;
using SimpleTasks.ViewModels;
using Microsoft.Phone.Shell;
using SimpleTasks.Resources;
using SimpleTasks.Controls;
using System.Diagnostics;

namespace SimpleTasks.Views
{
    public partial class ReminderPickerPage : BasePickerPage
    {
        public ReminderPickerPage()
        {
            initReminder = TimeSpan.Zero;
            if (PhoneApplicationService.Current.State.ContainsKey("Reminder"))
            {
                initReminder = (PhoneApplicationService.Current.State["Reminder"] as TimeSpan?) ?? initReminder;
                PhoneApplicationService.Current.State.Remove("Reminder");
            }

            InitializeComponent();

            SetTimeSpan(initReminder);
            DataContext = this;
        }

        private TimeSpan initReminder;

        protected override void Save()
        {
            PhoneApplicationService.Current.State["Reminder"] = new TimeSpan(
                DaySlider.RoundValue,
                HourSlider.RoundValue,
                MinuteSlider.RoundValue,
                0);
        }

        private void QuickButton_Click(object sender, RoutedEventArgs e)
        {
            SetTimeSpan(TimeSpan.Parse((string)((Button)sender).Tag));
        }

        private void SetTimeSpan(TimeSpan ts)
        {
            DaySlider.SetSliderValue(ts.Days);
            HourSlider.SetSliderValue(ts.Hours);
            MinuteSlider.SetSliderValue(ts.Minutes);
        }
    }
}