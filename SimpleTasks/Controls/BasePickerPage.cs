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
using SimpleTasks.Helpers;

namespace SimpleTasks.Controls
{
    public abstract class BasePickerPage : BasePage
    {
        protected string _name;

        public BasePickerPage(string name)
        {
            _name = name;
            BuildAppBar();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        protected abstract object Save();

        protected virtual void BuildAppBar()
        {
            ApplicationBar = ThemeHelper.CreateApplicationBar();

            ApplicationBarIconButton appBarDoneButton = new ApplicationBarIconButton(new Uri("/Toolkit.Content/ApplicationBar.Check.png", UriKind.Relative));
            appBarDoneButton.Text = AppResources.AppBarDone;
            appBarDoneButton.Click += Save_Click;
            ApplicationBar.Buttons.Add(appBarDoneButton);

            ApplicationBarIconButton appBarCancelButton = new ApplicationBarIconButton(new Uri("/Toolkit.Content/ApplicationBar.Cancel.png", UriKind.Relative));
            appBarCancelButton.Text = AppResources.AppBarCancel;
            appBarCancelButton.Click += Cancel_Click;
            ApplicationBar.Buttons.Add(appBarCancelButton);
        }

        protected void Save_Click(object sender, EventArgs e)
        {
            NavigateBack(Save(), _name);
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            NavigateBack();
        }
    }
}