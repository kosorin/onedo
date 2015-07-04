using System.Collections.Generic;
using System.Windows.Media.Imaging;
using SimpleTasks.Core.Models;

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
