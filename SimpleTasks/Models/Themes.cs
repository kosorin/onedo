using System.Collections.Generic;
using System.Windows.Media;
using SimpleTasks.Resources;

namespace SimpleTasks.Models
{
    public static class Themes
    {
        static Themes()
        {
            Color defaultColor;
            List<Color> defaultColors;

            #region Default
            defaultColors = new List<Color>
            {
                Color.FromArgb(255, 164, 196, 0),
                (defaultColor = Color.FromArgb(255, 96, 169, 23)),
                Color.FromArgb(255, 0, 138, 0),
                Color.FromArgb(255, 0, 171, 169),
                Color.FromArgb(255, 27, 161, 226),
                Color.FromArgb(255, 0, 80, 239),
                Color.FromArgb(255, 106, 0, 255),
                Color.FromArgb(255, 170, 0, 255),
                Color.FromArgb(255, 244, 114, 208),
                Color.FromArgb(255, 216, 0, 115),
                Color.FromArgb(255, 162, 0, 37),
                Color.FromArgb(255, 229, 20, 0),
                Color.FromArgb(255, 250, 104, 0),
                Color.FromArgb(255, 240, 163, 10),
                Color.FromArgb(255, 227, 200, 0),
                Color.FromArgb(255, 130, 90, 44),
                Color.FromArgb(255, 109, 135, 100),
                Color.FromArgb(255, 100, 118, 135),
                Color.FromArgb(255, 118, 96, 138),
                Color.FromArgb(255, 135, 121, 78)
            };

            Dark = new Theme(AppResources.SettingsThemeDark, "Dark", defaultColor, defaultColors)
            {
                Preview = new ThemePreview
                {
                    NormalBrush = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)),
                    SubtleBrush = new SolidColorBrush(Color.FromArgb(153, 255, 255, 255)),
                    BackgroundBrush = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0)),
                    CheckBoxForegroundBrush = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)),
                    CheckBoxBorderBrush = new SolidColorBrush(Color.FromArgb(153, 255, 255, 255)) { Opacity = 0.45 }
                }
            };
            Light = new Theme(AppResources.SettingsThemeLight, "Light", defaultColor, defaultColors)
            {
                Preview = new ThemePreview
                {
                    NormalBrush = new SolidColorBrush(Color.FromArgb(222, 0, 0, 0)),
                    SubtleBrush = new SolidColorBrush(Color.FromArgb(102, 0, 0, 0)),
                    BackgroundBrush = new SolidColorBrush(Color.FromArgb(255, 238, 238, 238)),
                    CheckBoxForegroundBrush = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)),
                    CheckBoxBorderBrush = new SolidColorBrush(Color.FromArgb(102, 0, 0, 0)) { Opacity = 0.45 }
                }
            };

            #endregion // end of Default

            #region Solarized
            defaultColors = new List<Color>
            {
                Color.FromArgb(255, 181, 137, 0),
                (defaultColor = Color.FromArgb(255, 203, 75, 22)),
                Color.FromArgb(255, 220, 50, 47),
                Color.FromArgb(255, 211, 54, 130),
                Color.FromArgb(255, 108, 113, 196),
                Color.FromArgb(255, 38, 139, 210),
                Color.FromArgb(255, 42, 161, 152),
                Color.FromArgb(255, 133, 153, 0)
            };

            SolarizedDark = new Theme("Solarized Dark", "SolarizedDark", defaultColor, defaultColors)
            {
                Preview = new ThemePreview
                {
                    NormalBrush = new SolidColorBrush(Color.FromArgb(255, 253, 246, 227)),
                    SubtleBrush = new SolidColorBrush(Color.FromArgb(136, 253, 246, 227)),
                    BackgroundBrush = new SolidColorBrush(Color.FromArgb(255, 0, 43, 54)),
                    CheckBoxForegroundBrush = new SolidColorBrush(Color.FromArgb(255, 253, 246, 227)),
                    CheckBoxBorderBrush = new SolidColorBrush(Color.FromArgb(136, 253, 246, 227)) { Opacity = 0.45 }
                }
            };
            SolarizedLight = new Theme("Solarized Light", "SolarizedLight", defaultColor, defaultColors)
            {
                Preview = new ThemePreview
                {
                    NormalBrush = new SolidColorBrush(Color.FromArgb(255, 0, 43, 54)),
                    SubtleBrush = new SolidColorBrush(Color.FromArgb(153, 0, 43, 54)),
                    BackgroundBrush = new SolidColorBrush(Color.FromArgb(255, 253, 246, 227)),
                    CheckBoxForegroundBrush = new SolidColorBrush(Color.FromArgb(255, 238, 232, 213)),
                    CheckBoxBorderBrush = new SolidColorBrush(Color.FromArgb(153, 0, 43, 54)) { Opacity = 0.45 }
                }
            };
            #endregion // end of Solarized

            #region Ocean
            Ocean = new Theme("Ocean", "Ocean", Color.FromArgb(255, 0, 115, 105))
            {
                Preview = new ThemePreview
                {
                    NormalBrush = new SolidColorBrush(Color.FromArgb(255, 68, 253, 198)),
                    SubtleBrush = new SolidColorBrush(Color.FromArgb(153, 68, 253, 198)),
                    BackgroundBrush = new SolidColorBrush(Color.FromArgb(255, 0, 35, 41)),
                    CheckBoxForegroundBrush = new SolidColorBrush(Color.FromArgb(255, 68, 253, 198)),
                    CheckBoxBorderBrush = new SolidColorBrush(Color.FromArgb(153, 68, 253, 198)) { Opacity = 0.45 }
                }
            };
            #endregion // end of Ocean

            Default = Dark;
        }

        public static Theme GetTheme(string fileName, Theme defaultTheme)
        {
            if (fileName == Dark.FileName) return Dark;
            else if (fileName == Light.FileName) return Light;

            else if (fileName == SolarizedDark.FileName) return SolarizedDark;
            else if (fileName == SolarizedLight.FileName) return SolarizedLight;

            else if (fileName == Ocean.FileName) return Ocean;

            else return defaultTheme;
        }

        public static List<Theme> GetThemes()
        {
            return new List<Theme>
            {
                Dark,
                Light,
                SolarizedDark,
                SolarizedLight,
                Ocean
            };
        }

        public static Theme Default { get; set; }

        public static Theme Dark { get; private set; }

        public static Theme Light { get; private set; }

        public static Theme SolarizedDark { get; private set; }

        public static Theme SolarizedLight { get; private set; }

        public static Theme Ocean { get; private set; }
    }
}
