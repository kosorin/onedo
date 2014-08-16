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

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            App.UpdateAllLiveTiles(e);
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
            Navigate("TimePickerPage", App.Settings.Tasks.DefaultTime, "TimePicker");
        }
    }
}