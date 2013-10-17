using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace SimpleTasks.Views
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            DataContext = App.Settings;
        }

        private void DeleteCompletedToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            DeleteCompletedDaysShowStoryboard.Begin();
        }

        private void DeleteCompletedToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            DeleteCompletedDaysHideStoryboard.Begin();
        }
    }
}