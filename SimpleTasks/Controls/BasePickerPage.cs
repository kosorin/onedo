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
        #region Nastavení
        public BasePickerPage()
        {
            BuildAppBar();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            SystemTray.ForegroundColor = (Color)CurrentApp.Resources["BasePickerPageForegroundColor"];
            SystemTray.BackgroundColor = (Color)CurrentApp.Resources["BasePickerPageBackgroundColor"];
        }

        protected T RetrieveAndConfigure<T>(string page)
        {
            _page = page;
            return Retrieve<T>(page);
        }
        #endregion

        private string _page;

        private const string _pagePrefix = "Picker_";

        private const string _pageUriFormat = "/Views/{0}PickerPage.xaml";

        private object _valueToSave;

        /// <summary>
        /// V této metodě se musí zavolat metoda <see cref="SetValueToSave"/>,
        /// která nastaví hodnotu pro uložení (pokud se kliknulo na tlačítko AppBaru Done)
        /// </summary>
        protected abstract void Save();

        protected void SetValueToSave(object value)
        {
            _valueToSave = value;
        }

        #region AppBar
        protected virtual void BuildAppBar()
        {
            ApplicationBar = new ApplicationBar();

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
            if (NavigationService.CanGoBack)
            {
                Save();
                PhoneApplicationService.Current.State[_pagePrefix + _page] = _valueToSave;
                NavigationService.GoBack();
            }
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
        #endregion

        #region Navigace z venku
        public static void Navigate(string page, object value)
        {
            PhoneApplicationFrame frame = App.Current.RootVisual as PhoneApplicationFrame;
            if (frame != null)
            {
                PhoneApplicationService.Current.State[_pagePrefix + page] = value;
                frame.Navigate(new Uri(string.Format(_pageUriFormat, page), UriKind.Relative));
            }
        }

        public static T Retrieve<T>(string page)
        {
            T value = (T)PhoneApplicationService.Current.State[_pagePrefix + page];
            PhoneApplicationService.Current.State.Remove(_pagePrefix + page);
            return value;
        }

        public static bool CanRetrieve(string page)
        {
            return PhoneApplicationService.Current.State.ContainsKey(_pagePrefix + page);
        }
        #endregion
    }
}