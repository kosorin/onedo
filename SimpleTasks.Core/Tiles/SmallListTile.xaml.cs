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
    public partial class SmallListTile : ListTileControl
    {
        public SmallListTile()
        {
            InitializeComponent();
        }

        public SmallListTile(IList<TaskModel> tasks)
            : base(tasks)
        {
            InitializeComponent();
        }

        public override void Refresh()
        {
            IList<TaskModel> tasks = Tasks;
            if (tasks == null)
                return;

            int count = tasks.Count;
            if (Settings.Current.ShowTaskCount)
            {
                CountWrapper.Visibility = Visibility.Visible;

                CountText.Text = count.ToString();
            }
            else
            {
                ListWrapper.Visibility = Visibility.Visible;

                // Podúkoly
                NoTasksIcon.Visibility = count == 0 ? Visibility.Visible : Visibility.Collapsed;
                TasksStackPanel.Visibility = count > 0 ? Visibility.Visible : Visibility.Collapsed;

                if (count > 0)
                {
                    TasksStackPanel.Children.Clear();
                    Style taskItemStyle = (Style)Resources["TaskItemStyle"];
                    SolidColorBrush lowPriorityBrush = new SolidColorBrush(Colors.White) { Opacity = 0.75 };

                    double lineHeight = 40;
                    int maxTasks = (int)Math.Round(159 / lineHeight);
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
            }

            // Pozadí
            LayoutRoot.Background = new SolidColorBrush(Colors.Transparent);
            //LayoutRoot.Background = new SolidColorBrush(task.Color) { Opacity = settings.BackgroundOpacity };
        }
    }
}
