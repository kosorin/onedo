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
    public partial class MediumListTile : ListTileControl
    {
        public MediumListTile()
        {
            InitializeComponent();
        }

        public MediumListTile(IList<TaskModel> tasks)
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
                Style taskItemStyle = (Style)Resources["TaskItemStyle"];
                SolidColorBrush lowPriorityBrush = new SolidColorBrush(Colors.White) { Opacity = 0.75 }; ;

                int maxTasks = (int)Math.Round(336 / lineHeight) + (Settings.Current.Tiles.ShowTaskCount ? -2 : 0);
                foreach (TaskModel task in tasks.Take(maxTasks))
                {
                    ContentControl cc = new ContentControl();
                    cc.Style = taskItemStyle;
                    if (task.IsLowPriority)
                    {
                        cc.Foreground = lowPriorityBrush;
                    }
                    cc.Content = task.Title;

                    cc.FontSize = lineHeight * 0.72;
                    cc.Height = lineHeight;
                    TasksStackPanel.Children.Add(cc);
                }
            }

            // Pozadí
            LayoutRoot.Background = new SolidColorBrush(Colors.Transparent);
            //LayoutRoot.Background = new SolidColorBrush(task.Color) { Opacity = settings.BackgroundOpacity };
        }
    }
}
