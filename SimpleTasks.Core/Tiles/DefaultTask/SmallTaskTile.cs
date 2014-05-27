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

namespace SimpleTasks.Core.Tiles.DefaultTask
{
    public class SmallTaskTile : TileTemplate
    {
        public SolidColorBrush ForegroundBrush = new SolidColorBrush(Colors.White);

        public SolidColorBrush TitleBackgroundBrush = new SolidColorBrush(Colors.Black) { Opacity = 0.35 };

        public SolidColorBrush DetailBackgroundBrush = new SolidColorBrush(Colors.Black) { Opacity = 0.15 };

        public SolidColorBrush InfoBackgroundBrush = new SolidColorBrush(Colors.Black) { Opacity = 0.35 };

        public Thickness BorderThickness = new Thickness(0, 1, 0, 1);

        public SolidColorBrush BorderBrush = new SolidColorBrush(Colors.Black) { Opacity = 0.5 };

        public double InfoOpacity = 0.86;

        public SmallTaskTile()
        {
            Width = SmallSize;
            Height = SmallSize;
        }

        public override WriteableBitmap Render(List<TaskModel> tasks)
        {
            throw new NotImplementedException();
        }

        public override WriteableBitmap Render(TaskModel task)
        {
            WriteableBitmap wb = new WriteableBitmap(Width, Height);

            Grid grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });

            // Title
            Border titleBorder = new Border()
            {
                Background = TitleBackgroundBrush,
                Padding = new Thickness(10, 10, 5, 5)
            };
            Grid.SetRow(titleBorder, 0);
            grid.Children.Add(titleBorder);
            titleBorder.Child = new TextBlock()
            {
                Text = task.Title,
                Foreground = ForegroundBrush,
                TextWrapping = TextWrapping.Wrap,
                FontWeight = FontWeights.Light,
                FontSize = 40,
            };

            // Detail
            Border detailBorder = new Border()
            {
                Background = DetailBackgroundBrush,
                BorderThickness = BorderThickness,
                BorderBrush = BorderBrush,
                Padding = new Thickness(10, 5, 5, 0)
            };
            Grid.SetRow(detailBorder, 1);
            grid.Children.Add(detailBorder);
            detailBorder.Child = new TextBlock()
            {
                Text = task.Detail,
                Opacity = 0.9,
                Foreground = ForegroundBrush,
                TextWrapping = TextWrapping.Wrap,
                FontWeight = FontWeights.Light,
                FontSize = 32,
            };

            // Info
            Border infoBorder = new Border()
            {
                Background = InfoBackgroundBrush,
                Padding = new Thickness(5, 5, 5, 0)
            };
            Grid.SetRow(infoBorder, 2);
            grid.Children.Add(infoBorder);

            Grid infoGrid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
            infoBorder.Child = infoGrid;

            if (task.HasDueDate)
            {
                StackPanel dueDateStackPanel = new StackPanel() { Orientation = Orientation.Horizontal };
                dueDateStackPanel.Children.Add(new TextBlock()
                {
                    Text = "\uE163",
                    Foreground = ForegroundBrush,
                    FontFamily = new FontFamily("Segoe UI Symbol"),
                    Margin = new Thickness(0, -5, 0, 0),
                    FontSize = 32,
                    Opacity = InfoOpacity
                });
                dueDateStackPanel.Children.Add(new TextBlock()
                {
                    Text = task.DueDate.Value.ToShortDateString(),
                    Foreground = ForegroundBrush,
                    FontWeight = FontWeights.Light,
                    Margin = new Thickness(5, 0, 0, 0),
                    FontSize = 32,
                    Opacity = InfoOpacity
                });
                Grid.SetColumn(dueDateStackPanel, 0);
                infoGrid.Children.Add(dueDateStackPanel);
            }

            StackPanel moreInfoStackPanel = new StackPanel() {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            if (task.HasReminder)
            {
                moreInfoStackPanel.Children.Add(new TextBlock()
                {
                    Text = "\uE1FA",
                    Foreground = ForegroundBrush,
                    FontFamily = new FontFamily("Segoe UI Symbol"),
                    Margin = new Thickness(0, -5, 0, 0),
                    FontSize = 32,
                    Opacity = InfoOpacity
                });
            }
            if (task.Priority==TaskPriority.High)
            {
                moreInfoStackPanel.Children.Add(new TextBlock()
                {
                    Text = "\uE171",
                    Foreground = ForegroundBrush,
                    FontFamily = new FontFamily("Segoe UI Symbol"),
                    Margin = new Thickness(0, -7, 0, 0),
                    FontSize = 32,
                    Opacity = InfoOpacity
                });
            }
            else if (task.Priority == TaskPriority.Low)
            {
                moreInfoStackPanel.Children.Add(new TextBlock()
                {
                    Text = "\ue1fd",
                    Foreground = ForegroundBrush,
                    FontFamily = new FontFamily("Segoe UI Symbol"),
                    Margin = new Thickness(0, 0, 0, 0),
                    FontSize = 28,
                    Opacity = InfoOpacity
                });
            }
            Grid.SetColumn(moreInfoStackPanel, 1);
            infoGrid.Children.Add(moreInfoStackPanel);

            // Konec
            grid.UpdateLayout();
            grid.Measure(new Size(Width, Height));
            grid.Arrange(new Rect(0, 0, Width, Height));

            wb.Render(grid, null);
            wb.Invalidate();
            return wb;
        }
    }
}
