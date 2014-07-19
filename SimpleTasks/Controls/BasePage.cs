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
    }
}
