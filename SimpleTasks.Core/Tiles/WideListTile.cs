using SimpleTasks.Core.Helpers;
using SimpleTasks.Core.Models;
using SimpleTasks.Core.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SimpleTasks.Core.Tiles
{
    class WideListTile : SimpleListTile
    {
        public WideListTile(int taskCount, int width, int height)
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
            if (task.IsImportant)
            {
                innerBorder.Background = ImportantBrush;
            }

            Grid contentGrid = new Grid();
            contentGrid.ColumnDefinitions.Add(new ColumnDefinition());
            contentGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
            innerBorder.Child = contentGrid;

            TextBlock titleTextBlock = new TextBlock()
            {
                Text = task.Title,
                Margin = new Thickness(5, 0, 0, 0),
                Foreground = ForegroundBrush,
                FontSize = ((double)Height / (double)TaskCount) * 0.7,
                VerticalAlignment = VerticalAlignment.Bottom,
            };
            Grid.SetColumn(titleTextBlock, 0);
            contentGrid.Children.Add(titleTextBlock);

            TextBlock dueTextBlock = new TextBlock()
            {
                Text = DateTimeExtensions.ToRelativeString(task.DueDate, "", false),
                Margin = new Thickness(20, 0, 5, 0),
                Foreground = ForegroundBrush,
                FontSize = ((double)Height / (double)TaskCount) * 0.7,
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            Grid.SetColumn(dueTextBlock, 1);
            contentGrid.Children.Add(dueTextBlock);

            border.Child = innerBorder;
            return border;
        }
    }
}
