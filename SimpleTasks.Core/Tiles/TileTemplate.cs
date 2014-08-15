using SimpleTasks.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SimpleTasks.Core.Tiles
{
    public abstract class TileTemplate
    {
        public static int SmallSize { get { return 159; } }

        public static int MediumSize { get { return 336; } }

        public static int WideSize { get { return 691; } }

        public int Width { get; set; }

        public int Height { get; set; }

        public abstract WriteableBitmap Render(List<TaskModel> tasks);

        public abstract WriteableBitmap Render(TaskModel task);
    }
}
