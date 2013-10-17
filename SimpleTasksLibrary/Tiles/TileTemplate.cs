using SimpleTasks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SimpleTasks.Tiles
{
    public abstract class TileTemplate
    {
        public SolidColorBrush BackgroundBrush = (SolidColorBrush)Application.Current.Resources["PhoneAccentBrush"];

        public SolidColorBrush ForegroundBrush = new SolidColorBrush(Colors.White);

        public SolidColorBrush BorderBrush = new SolidColorBrush(Colors.Black) { Opacity = 0.5 };

        public SolidColorBrush ImportantBrush = new SolidColorBrush(Colors.Black) { Opacity = 0.3 };

        public int Width { get; set; }

        public int Height { get; set; }

        public abstract WriteableBitmap Render(List<TaskModel> tasks);
    }
}
