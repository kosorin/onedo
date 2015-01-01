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

        private static ResourceDictionary Resources
        {
            get { return ((App)App.Current).Resources; }
        }

        private const string _themeKey = "Theme";
        public static Theme Theme
        {
            get
            {
                IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
                Theme theme;
                if (settings.Contains(_themeKey))
                {
                    theme = (Theme)settings[_themeKey];
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
                if (settings.Contains(_themeKey))
                {
                    if (!settings[_themeKey].Equals(value))
                    {
                        settings[_themeKey] = value;
                    }
                }
                else
                {
                    settings.Add(_themeKey, value);
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

        private static Theme _systemTheme = Theme.Dark;
        public static Theme SystemTheme
        {
            get { return _systemTheme; }
            private set { _systemTheme = value; }
        }

        private const string _themeColorKey = "ThemeColor";
        public static Color ThemeColor
        {
            get
            {
                IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
                Color color;
                if (settings.Contains(_themeColorKey))
                {
                    color = (Color)settings[_themeColorKey];
                }
                else
                {
                    color = Colors.Transparent;
                }
                return color;
            }
            set
            {
                IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
                if (settings.Contains(_themeColorKey))
                {
                    if (!settings[_themeColorKey].Equals(value))
                    {
                        settings[_themeColorKey] = value;
                    }
                }
                else
                {
                    settings.Add(_themeColorKey, value);
                }
                settings.Save();
            }
        }

        private static void ReplaceColor(ResourceDictionary rd, string key, Color newColor)
        {
            rd.Remove(key);
            rd.Add(key, newColor);
        }

        private static void ReplaceBrush(ResourceDictionary rd, string key, SolidColorBrush newBrush)
        {
            SolidColorBrush brush = (SolidColorBrush)rd[key];
            brush.Color = newBrush.Color;
            brush.Opacity = newBrush.Opacity;
        }

        private static void SetAccentColor(ResourceDictionary rd, Color color)
        {
            ReplaceColor(rd, "AccentColor", color);
            ReplaceBrush(rd, "AccentBrush", new SolidColorBrush(color));
        }

        public static void InitializeTheme()
        {
#if DEBUG
            Stopwatch sw = new Stopwatch();
            sw.Start();
#endif

            SystemTheme = (Visibility)Resources["PhoneDarkThemeVisibility"] == Visibility.Visible ? Theme.Dark : Theme.Light;
            CurrentTheme = (Theme == Theme.System) ? SystemTheme : Theme;

            // ResourceDictionary
            Uri sourceUri = new Uri(string.Format("/SimpleTasks;component/Themes/{0}.xaml", CurrentTheme.ToString()), UriKind.Relative);
            ResourceDictionary appTheme = Resources.MergedDictionaries[0];
            ResourceDictionary theme = new ResourceDictionary
            {
                Source = sourceUri
            };

            // Barvy
            foreach (var ck in theme.Where(x => x.Value is Color))
            {
                ReplaceColor(appTheme, (string)ck.Key, (Color)ck.Value);
            }

            // Brushe
            foreach (var ck in theme.Where(x => x.Value is SolidColorBrush))
            {
                ReplaceBrush(appTheme, (string)ck.Key, (SolidColorBrush)ck.Value);
            }

            // Accent barva
            if (CurrentTheme.IsSolarized())
            {
                SetAccentColor(appTheme, ThemeColor);
            }
            else if ((bool)theme["UsePhoneAccentColor"])
            {
                SetAccentColor(appTheme, (Color)Resources["PhoneAccentColor"]);
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
