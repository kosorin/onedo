using SimpleTasks.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SimpleTasks.Models
{
    public static class Themes
    {
        static Themes()
        {
            Color defaultColor;
            List<Color> defaultColors;

            #region Default
            defaultColors = new List<Color>();
            defaultColors.Add(Color.FromArgb(255, 164, 196, 0));
            defaultColors.Add(defaultColor = Color.FromArgb(255, 96, 169, 23));
            defaultColors.Add(Color.FromArgb(255, 0, 138, 0));
            defaultColors.Add(Color.FromArgb(255, 0, 171, 169));
            defaultColors.Add(Color.FromArgb(255, 27, 161, 226));
            defaultColors.Add(Color.FromArgb(255, 0, 80, 239));
            defaultColors.Add(Color.FromArgb(255, 106, 0, 255));
            defaultColors.Add(Color.FromArgb(255, 170, 0, 255));
            defaultColors.Add(Color.FromArgb(255, 244, 114, 208));
            defaultColors.Add(Color.FromArgb(255, 216, 0, 115));
            defaultColors.Add(Color.FromArgb(255, 162, 0, 37));
            defaultColors.Add(Color.FromArgb(255, 229, 20, 0));
            defaultColors.Add(Color.FromArgb(255, 250, 104, 0));
            defaultColors.Add(Color.FromArgb(255, 240, 163, 10));
            defaultColors.Add(Color.FromArgb(255, 227, 200, 0));
            defaultColors.Add(Color.FromArgb(255, 130, 90, 44));
            defaultColors.Add(Color.FromArgb(255, 109, 135, 100));
            defaultColors.Add(Color.FromArgb(255, 100, 118, 135));
            defaultColors.Add(Color.FromArgb(255, 118, 96, 138));
            defaultColors.Add(Color.FromArgb(255, 135, 121, 78));

            Dark = new Theme(AppResources.SettingsThemeDark, "Dark", defaultColor, defaultColors);
            Light = new Theme(AppResources.SettingsThemeLight, "Light", defaultColor, defaultColors);
            #endregion // end of Default

            #region Solarized
            defaultColors = new List<Color>();
            defaultColors.Add(Color.FromArgb(255, 181, 137, 0));
            defaultColors.Add(defaultColor = Color.FromArgb(255, 203, 75, 22));
            defaultColors.Add(Color.FromArgb(255, 220, 50, 47));
            defaultColors.Add(Color.FromArgb(255, 211, 54, 130));
            defaultColors.Add(Color.FromArgb(255, 108, 113, 196));
            defaultColors.Add(Color.FromArgb(255, 38, 139, 210));
            defaultColors.Add(Color.FromArgb(255, 42, 161, 152));
            defaultColors.Add(Color.FromArgb(255, 133, 153, 0));

            SolarizedDark = new Theme("Solarized Dark", "SolarizedDark", defaultColor, defaultColors);
            SolarizedLight = new Theme("Solarized Light", "SolarizedLight", defaultColor, defaultColors);
            #endregion // end of Solarized

            #region Ocean
            Ocean = new Theme("Ocean", "Ocean", Color.FromArgb(255, 0, 115, 105));
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
