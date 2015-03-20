using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SimpleTasks.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SimpleTasks.Helpers
{
    public static class ThemeHelper
    {
        #region Public Methods
        public static float Lerp(this float start, float end, float amount)
        {
            float difference = end - start;
            float adjusted = difference * amount;
            return start + adjusted;
        }

        public static Color Lerp(this Color colour, Color to, float amount)
        {
            // start colours as lerp-able floats
            float sr = colour.R, sg = colour.G, sb = colour.B;

            // end colours as lerp-able floats
            float er = to.R, eg = to.G, eb = to.B;

            // lerp the colours to get the difference
            byte r = (byte)sr.Lerp(er, amount),
                 g = (byte)sg.Lerp(eg, amount),
                 b = (byte)sb.Lerp(eb, amount);

            // return the new colour
            return Color.FromArgb(colour.A, r, g, b);
        }
        #endregion // end of Public Methods

        #region Resources
        private static ResourceDictionary Resources
        {
            get { return ((App)App.Current).Resources; }
        }

        private static void ReplaceDouble(ResourceDictionary rd, string key, double newValue)
        {
            rd.Remove(key);
            rd.Add(key, newValue);
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
        #endregion // end of Resources

        private const string _themeFileNameKey = "ThemeFileName";
        public static string ThemeFileName
        {
            get
            {
                IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
                string theme;
                if (settings.Contains(_themeFileNameKey))
                {
                    theme = (string)settings[_themeFileNameKey];
                }
                else
                {
                    theme = Theme.EmptyFileName;
                }
                return theme;
            }
            set
            {
                IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
                if (settings.Contains(_themeFileNameKey))
                {
                    if (!settings[_themeFileNameKey].Equals(value))
                    {
                        settings[_themeFileNameKey] = value;
                    }
                }
                else
                {
                    settings.Add(_themeFileNameKey, value);
                }
                settings.Save();
            }
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
                    color = Theme.EmptyColor;
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

        private static Theme _currentTheme = Themes.Default;
        public static Theme CurrentTheme
        {
            get { return _currentTheme; }
            set { _currentTheme = value; }
        }

        private static Theme _systemTheme = Themes.Default;
        public static Theme SystemTheme
        {
            get { return _systemTheme; }
            private set { _systemTheme = value; }
        }

        public static void InitializeTheme()
        {
#if DEBUG
            Stopwatch sw = new Stopwatch();
            sw.Start();
#endif

            SystemTheme = (Visibility)Resources["PhoneDarkThemeVisibility"] == Visibility.Visible
                ? Themes.Dark
                : Themes.Light;
            CurrentTheme = Themes.GetTheme(ThemeFileName, SystemTheme);

            // ResourceDictionary
            Uri sourceUri = CurrentTheme.ResourcesPath;
            ResourceDictionary appTheme = Resources.MergedDictionaries[0];
            ResourceDictionary theme = new ResourceDictionary
            {
                Source = sourceUri
            };

            // Čísla
            foreach (var ck in theme.Where(x => x.Value is double))
            {
                ReplaceDouble(appTheme, (string)ck.Key, (double)ck.Value);
            }

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
            if (ThemeColor != Theme.EmptyColor)
            {
                SetAccentColor(appTheme, ThemeColor);
            }
            else
            {
                SetAccentColor(appTheme, CurrentTheme.DefaultColor);
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

            // Speciální
            if (Resources.Contains("DarkerAccentBrush"))
            {
                Color color = (Color)Resources["AccentColor"];
                ((SolidColorBrush)Resources["DarkerAccentBrush"]).Color = color.Lerp(Colors.Black, 0.12f);
            }
            if (Resources.Contains("DarkerTasksListHeaderBackgroundBrush"))
            {
                Color color = (Color)Resources["TasksListHeaderBackgroundColor"];
                ((SolidColorBrush)Resources["DarkerTasksListHeaderBackgroundBrush"]).Color = color.Lerp(Colors.Black, 0.1f);
            }

#if DEBUG
            sw.Stop();
            Debug.WriteLine("## CHANGE THEME RESOURCE: ELAPSED TOTAL = {0}", sw.Elapsed);
#endif
        }

        #region AppBar
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
        #endregion // end of AppBar
    }
}
