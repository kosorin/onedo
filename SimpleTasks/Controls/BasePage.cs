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

namespace SimpleTasks.Controls
{
    public abstract class BasePage : PhoneApplicationPage, INotifyPropertyChanged
    {
        public App CurrentApp { get { return (App)Application.Current; } }

        public BasePage()
        {
            TransitionService.SetNavigationInTransition(this, new NavigationInTransition()
            {
                Backward = new SlideTransition() { Mode = SlideTransitionMode.SlideDownFadeIn },
                Forward = new SlideTransition() { Mode = SlideTransitionMode.SlideUpFadeIn }
            });
            TransitionService.SetNavigationOutTransition(this, new NavigationOutTransition()
            {
                Backward = new SlideTransition() { Mode = SlideTransitionMode.SlideDownFadeOut },
                Forward = new SlideTransition() { Mode = SlideTransitionMode.SlideUpFadeOut }
            });

            // WP Toolkit
            TiltEffect.SetIsTiltEnabled(this, true);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            SystemTray.ForegroundColor = (Color)CurrentApp.Resources["SystemTrayForegroundColor"];
            SystemTray.BackgroundColor = (Color)CurrentApp.Resources["SystemTrayBackgroundColor"];
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

        #region Parametry
        private const string _paramPrefix = "PageParam_";

        public static void SetParam(string param, object value)
        {
            PhoneApplicationService.Current.State[_paramPrefix + param] = value;
        }

        public static T GetParam<T>(string param)
        {
            T value = (T)PhoneApplicationService.Current.State[_paramPrefix + param];
            PhoneApplicationService.Current.State.Remove(_paramPrefix + param);
            return value;
        }

        public static bool IsSetParam(string param)
        {
            return PhoneApplicationService.Current.State.ContainsKey(_paramPrefix + param);
        }
        #endregion
    }
}
