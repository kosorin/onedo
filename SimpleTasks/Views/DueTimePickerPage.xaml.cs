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

namespace SimpleTasks.Views
{
    public partial class DueTimePickerPage : PhoneApplicationPage
    {
        public DueTimePickerPage()
        {
            InitializeComponent();

            if (PhoneApplicationService.Current.State.ContainsKey("DueTime"))
            {
                initTime = (PhoneApplicationService.Current.State["DueTime"] as DateTime?) ?? DateTime.Now;
                PhoneApplicationService.Current.State.Remove("DueTime");
            }
            else
            {
                initTime = DateTime.Now;
            }

            DataContext = this;
            BuildAppBar();
        }

        private DateTime initTime;

        private void SaveTime()
        {
            PhoneApplicationService.Current.State["DueTime"] = new DateTime(
                initTime.Year,
                initTime.Month,
                initTime.Day,
                TimePicker.Hours,
                TimePicker.Minutes,
                0);
        }

        #region AppBar

        private void BuildAppBar()
        {
            ApplicationBar = new ApplicationBar();

            ApplicationBarIconButton appBarDoneButton = new ApplicationBarIconButton(new Uri("/Toolkit.Content/ApplicationBar.Check.png", UriKind.Relative));
            appBarDoneButton.Text = AppResources.AppBarDone;
            appBarDoneButton.Click += appBarDoneButton_Click;
            ApplicationBar.Buttons.Add(appBarDoneButton);


            ApplicationBarIconButton appBarCancelButton = new ApplicationBarIconButton(new Uri("/Toolkit.Content/ApplicationBar.Cancel.png", UriKind.Relative));
            appBarCancelButton.Text = AppResources.AppBarCancel;
            appBarCancelButton.Click += appBarCancelButton_Click;
            ApplicationBar.Buttons.Add(appBarCancelButton);
        }

        void appBarDoneButton_Click(object sender, EventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                SaveTime();
                NavigationService.GoBack();
            }
        }

        void appBarCancelButton_Click(object sender, EventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        #endregion
    }
}