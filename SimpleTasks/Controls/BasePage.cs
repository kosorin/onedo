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
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using SimpleTasks.Controls.Transitions;
using SimpleTasks.Views;
using SimpleTasks.Core.Helpers;
using SimpleTasks.Core.Models;
using SimpleTasks.Helpers;
using SimpleTasks.Helpers.Analytics;

namespace SimpleTasks.Controls
{
    public abstract class BasePage : PhoneApplicationPage, INotifyPropertyChanged
    {
        public BasePage()
        {
            Debug.WriteLine("CTOR {0}", this);

            TransitionService.SetNavigationInTransition(this, new NavigationInTransition()
            {
                Backward = new OneTransition() { Mode = OneTransitionMode.BackwardIn },
                Forward = new OneTransition() { Mode = OneTransitionMode.ForwardIn }
            });
            TransitionService.SetNavigationOutTransition(this, new NavigationOutTransition()
            {
                Backward = new OneTransition() { Mode = OneTransitionMode.BackwardOut },
                Forward = new OneTransition() { Mode = OneTransitionMode.ForwardOut }
            });
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Debug.WriteLine("NAV TO: {0} ({1})", this, e.NavigationMode);
            base.OnNavigatedTo(e);

            SystemTray.ForegroundColor = (Color)App.Current.Resources["SystemTrayForegroundColor"];
            SystemTray.BackgroundColor = (Color)App.Current.Resources["SystemTrayBackgroundColor"];

            GoogleAnalyticsHelper.SendPage(this);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Debug.WriteLine("NAV FROM: {0} ({1})", this, e.NavigationMode);
            base.OnNavigatedFrom(e);

            if (!e.IsNavigationInitiator)
            {
                LiveTile.UpdateOrReset(Settings.Current.Tiles.Enable, App.Tasks.Tasks);
                foreach (TaskModel task in App.Tasks.Tasks)
                {
                    if (task.ModifiedSinceStart)
                    {
                        task.ModifiedSinceStart = false;
                        LiveTile.Update(task);
                    }
                }
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }
            storage = value;

            OnPropertyChanged(propertyName);
            return true;
        }
        #endregion

        #region Navigace
        private const string _navigationKey = "NavigationParameter";

        public void Navigate(Type pageType)
        {
            if (pageType == null || !pageType.IsSubclassOf(typeof(BasePage)))
                return;
            NavigationService.Navigate(new Uri("/Views/" + pageType.Name + ".xaml", UriKind.Relative));
        }

        public void NavigateQuery(Type pageType, string query)
        {
            if (pageType == null || !pageType.IsSubclassOf(typeof(BasePage)))
                return;
            NavigationService.Navigate(new Uri("/Views/" + pageType.Name + ".xaml" + query, UriKind.Relative));
        }

        public void NavigateQuery(Type pageType, string queryFormat, params object[] queryParams)
        {
            if (pageType == null || !pageType.IsSubclassOf(typeof(BasePage)))
                return;
            NavigationService.Navigate(new Uri("/Views/" + pageType.Name + ".xaml" + string.Format(queryFormat, queryParams), UriKind.Relative));
        }

        public void Navigate(Type pageType, object parameter, string parameterKey = "")
        {
            if (pageType == null || !pageType.IsSubclassOf(typeof(BasePage)))
                return;
            PhoneApplicationService.Current.State[_navigationKey + parameterKey] = parameter;
            NavigationService.Navigate(new Uri("/Views/" + pageType.Name + ".xaml", UriKind.Relative));
        }

        public bool NavigateBack()
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
                return true;
            }
            return false;
        }

        public bool NavigateBack(object parameter, string parameterKey = "")
        {
            if (NavigationService.CanGoBack)
            {
                PhoneApplicationService.Current.State[_navigationKey + parameterKey] = parameter;
                NavigationService.GoBack();
                return true;
            }
            return false;
        }

        public static T NavigationParameter<T>(string parameterKey = "")
        {
            T param = (T)PhoneApplicationService.Current.State[_navigationKey + parameterKey];
            PhoneApplicationService.Current.State.Remove(_navigationKey + parameterKey);
            return param;
        }

        public static bool IsSetNavigationParameter(string parameterKey = "")
        {
            return PhoneApplicationService.Current.State.ContainsKey(_navigationKey + parameterKey);
        }
        #endregion

        #region Other
        T FindFirstChild<T>(FrameworkElement element, string name = null) where T : FrameworkElement
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(element);
            var children = new FrameworkElement[childrenCount];

            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(element, i) as FrameworkElement;
                children[i] = child;
                if (child is T)
                {
                    if (name == null || child.Name == name)
                        return (T)child;
                }
            }

            for (int i = 0; i < childrenCount; i++)
            {
                if (children[i] != null)
                {
                    var subChild = FindFirstChild<T>(children[i]);
                    if (subChild != null)
                        return subChild;
                }
            }

            return null;
        }
        #endregion
    }
}
