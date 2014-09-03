using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SimpleTasks.Core.Models;
using System.Windows.Media;

namespace SimpleTasks.Core.Tiles
{
    public partial class WideListTile : ListTileControl
    {
        public WideListTile()
        {
            InitializeComponent();
        }

        public WideListTile(IList<TaskModel> tasks)
            : base(tasks)
        {
            InitializeComponent();
        }

        public override void Refresh()
        {
            IList<TaskModel> tasks = Tasks;
            if (tasks == null)
                return;

            double lineHeight = 48;
            //DataContext = task;
            //TaskTileSettings settings = task.TileSettings ?? Settings.Current.Tiles.DefaultTaskTileSettings;

            int count = tasks.Count;

            // Info
            if (Settings.Current.Tiles.ShowTaskCount)
            {
                InfoWrapper.Visibility = Visibility.Visible;
                InfoWrapper.Height = lineHeight * 2;

                // Icon
                IconBorder.Width = lineHeight * 2;
                Icon.Height = lineHeight * 1.667;

                // Count
                CountBorder.Width = lineHeight * 2;
                CountText.FontSize = lineHeight;
                CountText.Text = count.ToString();
            }
            else
            {
                InfoWrapper.Visibility = Visibility.Collapsed;
            }

            // Podúkoly
            NoTasksIcon.Visibility = count == 0 ? Visibility.Visible : Visibility.Collapsed;
            TasksStackPanel.Visibility = count > 0 ? Visibility.Visible : Visibility.Collapsed;
            if (count > 0)
            {
                TasksStackPanel.Children.Clear();

                SolidColorBrush lowPriorityBrush = new SolidColorBrush(Colors.White) { Opacity = 0.5 };

                Thickness borderThickness = new Thickness(0, 0, 0, 1);
                SolidColorBrush borderBrush = new SolidColorBrush(Color.FromArgb(89, 0, 0, 0));
                SolidColorBrush background = new SolidColorBrush(Color.FromArgb(30, 0, 0, 0));

                foreach (TaskModel task in tasks.Take((int)(336 / lineHeight) + 1))
                {
                    Border border = new Border()
                    {
                        BorderThickness = borderThickness,
                        BorderBrush = borderBrush,
                        Background = background,
                        Height = lineHeight
                    };

                    TextBlock dateTextBlock = null;
                    if (task.DueDate != null)
                    {
                        dateTextBlock = new TextBlock()
                        {
                            Text = task.DueDate.Value.ToShortDateString(),
                            Style = (Style)Resources["LineTextStyle"],
                            FontSize = lineHeight * 0.72,
                            LineHeight = lineHeight,
                            Opacity = 0.5
                        };
                    }
                    TextBlock titleTextBlock = new TextBlock()
                    {
                        Text = task.Title,
                        Style = (Style)Resources["LineTextStyle"],
                        FontSize = lineHeight * 0.72,
                        LineHeight = lineHeight
                    };

                    if (task.IsLowPriority)
                    {
                        if (dateTextBlock != null)
                        {
                            dateTextBlock.Opacity = 0.25;
                        }
                        titleTextBlock.Opacity = 0.5;
                    }

                    if (Settings.Current.Tiles.SwapDateAndTitleOnWide)
                    {
                        StackPanel content = new StackPanel() { Orientation = Orientation.Horizontal };
                        if (dateTextBlock != null)
                        {
                            content.Children.Add(dateTextBlock);
                        }
                        content.Children.Add(titleTextBlock);

                        border.Child = content;
                    }
                    else
                    {
                        Grid content = new Grid();
                        if (dateTextBlock != null)
                        {
                            content.ColumnDefinitions.Add(new ColumnDefinition());
                            content.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });

                            dateTextBlock.HorizontalAlignment = HorizontalAlignment.Right;
                            content.Children.Add(dateTextBlock);

                            Grid.SetColumn(dateTextBlock, 1);
                        }
                        content.Children.Add(titleTextBlock);

                        border.Child = content;

                    }

                    TasksStackPanel.Children.Add(border);
                }
            }

            // Pozadí
            LayoutRoot.Background = new SolidColorBrush(Colors.Transparent);
            //LayoutRoot.Background = new SolidColorBrush(task.Color) { Opacity = settings.BackgroundOpacity };
        }
    }
}
