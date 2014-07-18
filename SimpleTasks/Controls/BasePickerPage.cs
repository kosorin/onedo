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

namespace SimpleTasks.Controls
{
    public abstract class BasePickerPage : BasePage
    {
        public BasePickerPage()
        {
            BuildAppBar();
        }

        protected abstract void Save();

        protected void BuildAppBar()
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

        protected void appBarDoneButton_Click(object sender, EventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                Save();
                NavigationService.GoBack();
            }
        }

        protected void appBarCancelButton_Click(object sender, EventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
    }
}