using SimpleTasks.Core.Helpers;
using SimpleTasks.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SimpleTasks.Models
{
    public class Theme : BindableBase
    {
        public static string EmptyFileName { get { return null; } }

        public static Color EmptyColor { get { return System.Windows.Media.Colors.Transparent; } }

        public Theme(string name, string fileName, Color defaultColor, List<Color> colors = null)
        {
            Name = name;
            FileName = fileName;

            DefaultColor = defaultColor;
            if (colors == null || colors.Count == 0)
            {
                Colors = new List<Color> { defaultColor };
            }
            else
            {
                Colors = colors;
            }
        }

        public string Name { get; set; }

        public string FileName { get; set; }

        public Uri ResourcesPath
        {
            get
            {
                return new Uri(string.Format("/SimpleTasks;component/Themes/{0}.xaml", FileName), UriKind.Relative);
            }
        }

        public Color DefaultColor { get; set; }

        public List<Color> Colors { get; set; }
    }
}
