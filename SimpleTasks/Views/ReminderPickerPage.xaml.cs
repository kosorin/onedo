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

namespace SimpleTasks.Views
{
    public partial class ReminderPickerPage : BasePickerPage
    {
        public ReminderPickerPage()
        {
            InitializeComponent();

            initReminder = TimeSpan.Zero;
            if (PhoneApplicationService.Current.State.ContainsKey("Reminder"))
            {
                initReminder = (PhoneApplicationService.Current.State["Reminder"] as TimeSpan?) ?? initReminder;
                PhoneApplicationService.Current.State.Remove("Reminder");
            }
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
            string tag = (string)((Button)sender).Tag;
            string[] values = tag.Split(':');
            DaySlider.SetSliderValue(Convert.ToInt32(values[0]));
            HourSlider.SetSliderValue(Convert.ToInt32(values[1]));
            MinuteSlider.SetSliderValue(Convert.ToInt32(values[2]));
        }
    }
}