using SimpleTasks.Core.Models;
using System;
using System.Windows;
using System.Windows.Controls;

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
            if (task.DueDate != null && task.DueDate.Value > DateTime.Today.AddDays(1))
            {
                innerBorder.Opacity = 0.75;
            }

            if (task.Priority == TaskPriority.High)
            {
                innerBorder.Background = ImportantBackgroundBrush;
            }
            else
            {
                innerBorder.Background = NormalBackgroundBrush;
            }

            TextBlock textBlock = new TextBlock()
            {
                Text = task.TitleFirstLine,
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
