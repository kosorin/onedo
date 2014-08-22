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
using System.Collections.ObjectModel;
using System.Globalization;

namespace SimpleTasks.Views
{
    public partial class SettingsPage : BasePage
    {
        public SettingsPage()
        {
            InitializeComponent();
            DataContext = Settings.Current;
            PinTileTextBox.Text = PinTileHelpText;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (IsSetNavigationParameter("TimePicker"))
            {
                Settings.Current.Tasks.DefaultTime = NavigationParameter<DateTime>("TimePicker");
            }
        }

        public string PinTileHelpText { get { return string.Format(AppResources.SettingsPinTile, AppInfo.Name); } }

        private void DefaultTime_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Navigate(typeof(TimePickerPage), Settings.Current.Tasks.DefaultTime, "TimePicker");
        }

        #region Feedback
        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            Settings.Current.General.Feedback = true;
            GoogleAnalyticsHelper.SetDimension(CustomDimension.Feedback, "True");
            GoogleAnalyticsHelper.SendEvent(EventCategory.Settings, EventAction.Edit, "set feedback");
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Settings.Current.General.Feedback)
            {
                // Pokud už byl feedback nastavený na false a check box se změní na unchecked,
                // tak se znovu neposílá info o změně.
                GoogleAnalyticsHelper.SetDimension(CustomDimension.Feedback, "False");
                GoogleAnalyticsHelper.SendEvent(EventCategory.Settings, EventAction.Edit, "set feedback");
                Settings.Current.General.Feedback = false;
            }
        }
        #endregion

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            App.FeedbackEmail();
        }
    }
}