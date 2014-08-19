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
using SimpleTasks.Core.Models;
using DefaultDateTypes = SimpleTasks.Core.Models.Settings.TasksSettings.DefaultDateTypes;
using SimpleTasks.Resources;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using SimpleTasks.Core.Helpers;
using SimpleTasks.Helpers.Analytics;

namespace SimpleTasks.Views
{
    public partial class SettingsPage : BasePage
    {
        public SettingsPage()
        {
            InitializeComponent();
            DataContext = App.Settings;
            PinTileTextBox.Text = PinTileHelpText;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (IsSetNavigationParameter("TimePicker"))
            {
                App.Settings.Tasks.DefaultTime = NavigationParameter<DateTime>("TimePicker");
            }
        }

        public string PinTileHelpText { get { return string.Format(AppResources.SettingsPinTile, AppInfo.Name); } }

        private void DefaultTime_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Navigate(typeof(TimePickerPage), App.Settings.Tasks.DefaultTime, "TimePicker");
        }

        #region Feedback
        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            App.Settings.General.Feedback = true;
            GoogleAnalyticsHelper.SetDimension(CustomDimension.Feedback, "True");
            GoogleAnalyticsHelper.SendEvent(EventCategory.Settings, EventAction.Edit, "set feedback");
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            App.Settings.General.Feedback = true; // Aby šla odeslat následující zpráva.
            GoogleAnalyticsHelper.SetDimension(CustomDimension.Feedback, "False");
            GoogleAnalyticsHelper.SendEvent(EventCategory.Settings, EventAction.Edit, "set feedback");
            App.Settings.General.Feedback = false;
        }
        #endregion
    }
}