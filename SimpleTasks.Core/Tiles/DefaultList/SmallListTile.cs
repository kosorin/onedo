using SimpleTasks.Core.Models;
using SimpleTasks.Core.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SimpleTasks.Core.Tiles.DefaultList
{
    public class SmallListTile : TileTemplate
    {
        public SolidColorBrush BackgroundBrush = new SolidColorBrush(Colors.Black) { Opacity = 0.15 }; // (SolidColorBrush)Application.Current.Resources["PhoneAccentBrush"];

        public SolidColorBrush ForegroundBrush = new SolidColorBrush(Colors.White);

        public SolidColorBrush BorderBrush = new SolidColorBrush(Colors.Black) { Opacity = 0.5 };

        public SolidColorBrush ImportantBackgroundBrush = new SolidColorBrush(Colors.Black) { Opacity = 0.4 };

        public SolidColorBrush NormalBackgroundBrush = new SolidColorBrush(Colors.Black) { Opacity = 0.2 };

        public int TaskCount { get; set; }

        public SmallListTile()
        {
            TaskCount = 4;

            Width = SmallSize;
            Height = SmallSize;
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

        public override WriteableBitmap Render(TaskModel task)
        {
            throw new NotImplementedException();
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
                Text = task.Title,
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
