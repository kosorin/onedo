using SimpleTasks.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SimpleTasks.Core.Tiles
{
    public abstract class TileControl : UserControl
    {
        public object Data
        {
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object), typeof(TileControl), null);

        public TileControl(object data)
        {
            Data = data;
        }

        public static int SmallTileSize { get { return 159; } }

        public static int MediumTileSize { get { return 336; } }

        public static int WideTileSize { get { return 691; } }

        public void Render(Stream stream)
        {
            if (stream != null)
            {
                int width = (int)Width;
                int height = (int)Height;
                WriteableBitmap wb = new WriteableBitmap(width, height);

                UpdateLayout();
                Measure(new Size(width, height));
                Arrange(new Rect(0, 0, width, height));
                UpdateLayout();

                wb.Render(this, null);
                wb.Invalidate();

                wb.WritePNG(stream);
            }
        }

        public abstract void Refresh();

        public void ToPng(string fileName)
        {
            using (IsolatedStorageFileStream stream = IsolatedStorageFile.GetUserStoreForApplication().OpenFile(fileName, System.IO.FileMode.Create))
            {
                Refresh();
                Render(stream);
            }
        }
    }
}
