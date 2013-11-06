using SimpleTasks.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SimpleTasks.Core.Tiles
{
    public class NormalListTile : SimpleListTile
    {
        public NormalListTile(int taskCount, int width, int height)
            : base(taskCount, width, height)
        {
        }

        protected override Border GetTaskItemBorder(TaskModel task)
        {
            Border border = new Border()
            {
                Height = (double)Height / (double)TaskCount,
                BorderThickness = new Thickness(0, 0, 0, 1),
                BorderBrush = BorderBrush,
            };
            Border innerBorder = new Border()
            {
                BorderThickness = new Thickness(10, 0, 0, 0),
                BorderBrush = BorderBrush,
            };
            if (task.DueDate == null || task.DueDate.Value > DateTime.Today)
            {
                innerBorder.Opacity = 0.7;
            }
            if (task.Priority == TaskPriority.High)
            {
                innerBorder.Background = ImportantBrush;
            }

            TextBlock textBlock = new TextBlock()
            {
                Text = task.Title,
                Margin = new Thickness(5, 0, 5, 0),
                Foreground = ForegroundBrush,
                FontSize = ((double)Height / (double)TaskCount) * 0.7,
                VerticalAlignment = VerticalAlignment.Bottom
            };
            innerBorder.Child = textBlock;

            border.Child = innerBorder;
            return border;
        }
    }
}
