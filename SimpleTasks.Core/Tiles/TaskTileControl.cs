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
    public abstract class TaskTileControl : UserControl
    {
        public TaskModel Task
        {
            get { return (TaskModel)GetValue(TaskProperty); }
            set { SetValue(TaskProperty, value); }
        }
        public static readonly DependencyProperty TaskProperty =
            DependencyProperty.Register("Task", typeof(TaskModel), typeof(TaskTileControl), null);

        public TaskTileControl() { }

        public TaskTileControl(TaskModel task)
        {
            Task = task;
        }

        public abstract void Refresh();

        public void RenderToStream(Stream stream)
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

        public void SaveToPng(string fileName)
        {
            using (IsolatedStorageFileStream stream = IsolatedStorageFile.GetUserStoreForApplication().OpenFile(fileName, System.IO.FileMode.Create))
            {
                Refresh();
                RenderToStream(stream);
            }
        }
    }
}
