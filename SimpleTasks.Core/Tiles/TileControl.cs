using SimpleTasks.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace SimpleTasks.Core.Tiles
{
    public abstract class TileControl : UserControl
    {
        public static int SmallTileSize { get { return 159; } }

        public static int MediumTileSize { get { return 336; } }

        public static int WideTileSize { get { return 691; } }

        //public abstract WriteableBitmap Render(List<TaskModel> tasks);

        //public abstract WriteableBitmap Render(TaskModel task);
    }
}
