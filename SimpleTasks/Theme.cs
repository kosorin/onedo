using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SimpleTasks
{
    public enum Theme
    {
        // Neměnit hodnoty!
        System = 0,
        Light = 1,
        Dark = 2,
        SolarizedLight = 3,
        SolarizedDark = 4,
        Ocean = 5
    }

    public static class ThemeExtensions
    {
        public static bool IsSolarized(this Theme theme)
        {
            return theme == Theme.SolarizedLight || theme == Theme.SolarizedDark;
        }
    }

    public class ThemeC
    {
        public string Name { get; set; }

        public List<Color> AccentColors { get; set; }
    }
}
