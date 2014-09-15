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

        public static Theme SystemTheme
        {
            get { return (Visibility)Resources["PhoneDarkThemeVisibility"] == Visibility.Visible ? Theme.Dark : Theme.Light; }
        }

        public static void InitializeTheme()
        {

            //Debug.WriteLine("> Theme start: {0}", themeType);
            //Stopwatch sw = Stopwatch.StartNew();
            //if (themeType == Theme.Light)
            //{
            //    Debug.WriteLine(": to light theme");
            //    ThemeManager.ToLightTheme();
            //}
            //else if (themeType == Theme.Dark)
            //{
            //    Debug.WriteLine(": to dark theme");
            //    ThemeManager.ToDarkTheme();
            //}
            //sw.Stop();
            //Debug.WriteLine("Theme stop {0}", sw.ElapsedMilliseconds);

            string source = (Visibility)Resources["PhoneDarkThemeVisibility"] == Visibility.Visible ?
                "Dark" :
                "Light";
            string currentSource = (Visibility)Resources["DarkThemeVisibility"] == Visibility.Visible ?
                "Dark" :
                "Light";

            if (source == currentSource)
            {
                // Téma už je nastavené
                return;
            }
            else
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                // ResourceDictionary
                Uri sourceUri = new Uri(string.Format("/SimpleTasks;component/Themes/{0}.xaml", source), UriKind.Relative);
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

                sw.Stop();
                Debug.WriteLine("## CHANGE THEME RESOURCE: ELAPSED TOTAL = {0}", sw.Elapsed);
            }
        }
    }
}
