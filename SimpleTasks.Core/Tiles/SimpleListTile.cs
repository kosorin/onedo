using SimpleTasks.Core.Models;
using SimpleTasks.Core.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace SimpleTasks.Core.Tiles
{
    public class SimpleListTile : TileTemplate
    {
        public int TaskCount { get; set; }

        public SimpleListTile(int taskCount, int width, int height)
        {
            TaskCount = taskCount;

            Width = width;
            Height = height;
        }

        public override WriteableBitmap Render(List<TaskModel> tasks)
        {
            WriteableBitmap wb = new WriteableBitmap(Width, Height);

            Grid grid = new Grid()
            {
                Background = BackgroundBrush
            };

            if (tasks.Count > 0)
            {
                StackPanel stackPanel = new StackPanel()
                {
                    Orientation = Orientation.Vertical
                };
                foreach (TaskModel task in tasks.Take(TaskCount))
                {
                    Border border = GetTaskItemBorder(task);
                    stackPanel.Children.Add(border);
                }

                if (Debugger.IsAttached)
                {
                    if (stackPanel.Children.Contains(stackPanel.Children.Last()))
                    {
                        stackPanel.Children.Remove(stackPanel.Children.Last());
                    }
                    stackPanel.Children.Add(GetTaskItemBorder(new TaskModel
                    {
                        Title = DateTime.Now.ToString("HH:mm:ss")
                    }));
                }

                grid.Children.Add(stackPanel);
            }
            else
            {
                Grid emptyListGrid = GetEmptyListGrid();
                grid.Children.Add(emptyListGrid);
            }

            grid.UpdateLayout();
            grid.Measure(new Size(Width, Height));
            grid.Arrange(new Rect(0, 0, Width, Height));

            wb.Render(grid, null);
            wb.Invalidate();
            return wb;
        }

        protected virtual Border GetTaskItemBorder(TaskModel task)
        {
            Border border = new Border()
            {
                Height = (double)Height / (double)TaskCount,
                BorderThickness = new Thickness(0, 0, 0, 1),
                BorderBrush = BorderBrush,
            };
            Border innerBorder = new Border();
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
                Text = task.TitleFirstLine,
                Margin = new Thickness(5, 0, 2, 0),
                Foreground = ForegroundBrush,
                FontSize = ((double)Height / (double)TaskCount) * 0.7,
                VerticalAlignment = VerticalAlignment.Bottom
            };
            innerBorder.Child = textBlock;

            border.Child = innerBorder;
            return border;
        }

        protected virtual Grid GetEmptyListGrid()
        {
            Grid grid = new Grid()
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };

            TextBlock textBlock = new TextBlock()
            {
                Text = AppResources.TileNoTasks,
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Center,
                Foreground = ForegroundBrush,
                FontSize = ((double)Height / 6d),
            };
            textBlock.MaxWidth = Width * 0.9;
            grid.Children.Add(textBlock);

            return grid;
        }
    }
}
