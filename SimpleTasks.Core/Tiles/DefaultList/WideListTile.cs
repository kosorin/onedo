using SimpleTasks.Core.Helpers;
using SimpleTasks.Core.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SimpleTasks.Core.Tiles.DefaultList
{
    class WideListTile : SmallListTile
    {
        public WideListTile()
            : base()
        {
            TaskCount = 7;

            Width = WideSize;
            Height = MediumSize;
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

            if (task.Priority == TaskPriority.High)
            {
                innerBorder.Background = ImportantBackgroundBrush;
            }
            else
            {
                innerBorder.Background = NormalBackgroundBrush;
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

            StackPanel moreInfoStackPanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(20, 0, 0, 0),
                Opacity = 0.65
            };
            if (task.HasDueDate)
            {
                moreInfoStackPanel.Children.Add(new TextBlock()
                {
                    Text = task.DueDate.Value.ToShortDateString(),
                    Margin = new Thickness(0, 0, 5, 0),
                    Foreground = ForegroundBrush,
                    FontSize = ((double)Height / (double)TaskCount) * 0.7,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Right
                });
            }
            if (task.HasReminder)
            {
                moreInfoStackPanel.Children.Add(new TextBlock()
                {
                    Text = "\uE1FA",
                    Foreground = ForegroundBrush,
                    FontFamily = new FontFamily("Segoe UI Symbol"),
                    Margin = new Thickness(0, -3, -5, 0),
                    FontSize = 32,
                });
            }
            if (task.Priority == TaskPriority.High)
            {
                moreInfoStackPanel.Children.Add(new TextBlock()
                {
                    Text = "\uE171",
                    Foreground = ForegroundBrush,
                    FontFamily = new FontFamily("Segoe UI Symbol"),
                    Margin = new Thickness(-4, 0, 0, 0),
                    FontSize = 32,
                });
            }
            else if (task.Priority == TaskPriority.Low)
            {
                moreInfoStackPanel.Children.Add(new TextBlock()
                {
                    Text = "\ue1fd",
                    Foreground = ForegroundBrush,
                    FontFamily = new FontFamily("Segoe UI Symbol"),
                    Margin = new Thickness(-2, 4, 1, 0),
                    FontSize = 28,
                });
            }
            Grid.SetColumn(moreInfoStackPanel, 1);
            contentGrid.Children.Add(moreInfoStackPanel);

            border.Child = innerBorder;
            return border;
        }
    }
}
