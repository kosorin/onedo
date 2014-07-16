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

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (!e.IsNavigationInitiator)
            {
                App.UpdateAllLiveTiles();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (PhoneApplicationService.Current.State.ContainsKey("DueTime"))
            {
                App.Settings.DefaultTimeSetting = (DateTime)PhoneApplicationService.Current.State["DueTime"];
                DefaultTimeButton.Content = App.Settings.DefaultTimeSetting.ToShortTimeString();
                PhoneApplicationService.Current.State.Remove("DueTime");
            }
        }

        private void DefaultTime_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var phoneApplicationFrame = Application.Current.RootVisual as PhoneApplicationFrame;
            if (phoneApplicationFrame != null)
            {
                PhoneApplicationService.Current.State["DueTime"] = App.Settings.DefaultTimeSetting;
                phoneApplicationFrame.Navigate(new Uri("/Views/DueTimePickerPage.xaml", UriKind.Relative));
            }
        }
    }
}