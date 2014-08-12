using SimpleTasks.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
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

        protected T FindFirstChild<T>(FrameworkElement element, string name = null) where T : FrameworkElement
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(element);
            var children = new FrameworkElement[childrenCount];

            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(element, i) as FrameworkElement;
                children[i] = child;
                if (child is T)
                {
                    if (name == null || child.Name == name)
                        return (T)child;
                }
            }

            for (int i = 0; i < childrenCount; i++)
            {
                if (children[i] != null)
                {
                    var subChild = FindFirstChild<T>(children[i]);
                    if (subChild != null)
                        return subChild;
                }
            }

            return null;
        }
    }
}
