using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SimpleTasks.Helpers
{
    public static class ThemeHelper
    {
        private const string _themeSettingsKey = "Theme";

        private static ResourceDictionary Resources
        {
            get { return ((App)App.Current).Resources; }
        }

        public static Theme Theme
        {
            get
            {
                IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
                Theme theme;
                if (settings.Contains(_themeSettingsKey))
                {
                    theme = (Theme)settings[_themeSettingsKey];
                }
                else
                {
                    theme = Theme.System;
                }
                return theme;
            }
            set
            {
                IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
                if (settings.Contains(_themeSettingsKey))
                {
                    if (!settings[_themeSettingsKey].Equals(value))
                    {
                        settings[_themeSettingsKey] = value;
                    }
                }
                else
                {
                    settings.Add(_themeSettingsKey, value);
                }
                settings.Save();
            }
        }

        private static Theme _currentTheme = Theme.Dark;
        public static Theme CurrentTheme
        {
            get { return _currentTheme; }
            private set { _currentTheme = value; }
        }

        private static Theme _theme = Theme.Dark;
        public static Theme SystemTheme
        {
            get { return _theme; }
            private set { _theme = value; }
        }

        public static void InitializeTheme()
        {
#if DEBUG
            Stopwatch sw = new Stopwatch();
            sw.Start();
#endif

            SystemTheme = (Visibility)Resources["PhoneDarkThemeVisibility"] == Visibility.Visible ? Theme.Dark : Theme.Light;
            CurrentTheme = Theme == Theme.System ? SystemTheme : Theme;

            // ResourceDictionary
            Uri sourceUri = new Uri(string.Format("/SimpleTasks;component/Themes/{0}.xaml", CurrentTheme == Theme.Dark ? "Dark" : "Light"), UriKind.Relative);
            ResourceDictionary app = Resources.MergedDictionaries[0];
            ResourceDictionary theme = new ResourceDictionary
            {
                Source = sourceUri
            };

            // Barvy
            foreach (var ck in theme.Where(x => x.Value is Color))
            {
                app.Remove(ck.Key);
                app.Add(ck.Key, (Color)ck.Value);
            }

            // Brushe
            foreach (var ck in theme.Where(x => x.Value is SolidColorBrush))
            {
                SolidColorBrush brush = (SolidColorBrush)ck.Value;
                SolidColorBrush appBrush = (SolidColorBrush)app[ck.Key];

                appBrush.Color = brush.Color;
                appBrush.Opacity = brush.Opacity;
            }

            // RootFrame
            if (App.RootFrame != null && Resources.Contains("BackgroundBrush"))
            {
                SolidColorBrush rootFrameBackground = Resources["BackgroundBrush"] as SolidColorBrush;
                if (rootFrameBackground != null)
                {
                    App.RootFrame.Background = rootFrameBackground;
                }
            }

            // AppBar
            if (Resources.Contains("SystemTrayBackgroundColor") && Resources.Contains("SystemTrayForegroundColor"))
            {
                _appBarBackground = (Color)Resources["SystemTrayBackgroundColor"];
                _appBarForeground = (Color)Resources["SystemTrayForegroundColor"];
            }

#if DEBUG
            sw.Stop();
            Debug.WriteLine("## CHANGE THEME RESOURCE: ELAPSED TOTAL = {0}", sw.Elapsed);
#endif
        }

        private static Color _appBarBackground;

        private static Color _appBarForeground;

        public static ApplicationBar CreateApplicationBar()
        {
            ApplicationBar appBar = new ApplicationBar();
            appBar.Opacity = 1;
            appBar.BackgroundColor = _appBarBackground;
            appBar.ForegroundColor = _appBarForeground;
            return appBar;
        }
    }
}
