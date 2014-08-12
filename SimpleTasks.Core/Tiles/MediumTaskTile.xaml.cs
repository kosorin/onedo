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
using System.Windows.Media.Imaging;

namespace SimpleTasks.Core.Tiles
{
    public partial class MediumTaskTile : TileControl
    {
        public MediumTaskTile()
        {
            InitializeComponent();
            DataContext = null;
        }

        public void Refresh(TaskModel task, double size, SolidColorBrush background = null)
        {
            DataContext = task;

            // Title
            Title.FontSize = size * 0.75;
            Title.Text = task.Title;

            // Detail
            Detail.Visibility = !string.IsNullOrWhiteSpace(task.Detail) ? Visibility.Visible : Visibility.Collapsed;
            if (!string.IsNullOrWhiteSpace(task.Detail))
            {
                Detail.FontSize = size * 0.65;
                Detail.Text = task.Detail;
            }

            // Info
            bool showInfo = task.HasReminder || task.Priority != TaskPriority.Normal;
            InfoWrapper.Visibility = showInfo ? Visibility.Visible : Visibility.Collapsed;
            if (showInfo)
            {
                Info.Height = size;

                HighPriority.Visibility = task.IsHighPriority ? Visibility.Visible : Visibility.Collapsed;
                LowPriority.Visibility = task.IsLowPriority ? Visibility.Visible : Visibility.Collapsed;
                Reminder.Visibility = task.HasReminder ? Visibility.Visible : Visibility.Collapsed;
                Date.Text = task.DueDate.Value.ToShortDateString();
            }

            // Podúkoly
            Subtasks.Children.Clear();
            Subtasks.Visibility = task.Subtasks.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            foreach (Subtask subtask in task.Subtasks)
            {
                Viewbox vb = new Viewbox();
                vb.Height = size;
                vb.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;

                SubtaskControl sc = new SubtaskControl();

                sc.Icon.Visibility = subtask.IsCompleted ? Visibility.Visible : Visibility.Collapsed;
                sc.Text.Text = subtask.Text;
                sc.Strike.Visibility = subtask.IsCompleted ? Visibility.Visible : Visibility.Collapsed;
                if (subtask.IsCompleted)
                {
                    sc.Text.Opacity = 0.45;
                }

                vb.Child = sc;
                Subtasks.Children.Add(vb);
            }

            // Pozadí
            if (background != null)
            {
                LayoutRoot.Background = background;
            }
            else
            {
                LayoutRoot.Background = new SolidColorBrush(Colors.Transparent);
            }
        }

        public override WriteableBitmap Render()
        {
            int width = (int)Width;
            int height = (int)Height;
            WriteableBitmap wb = new WriteableBitmap(width, height);

            UpdateLayout();
            Measure(new Size(width, height));
            UpdateLayout();
            Arrange(new Rect(0, 0, width, height));

            UpdateLayout();
            wb.Render(this, null);
            wb.Invalidate();
            return wb;
        }
    }
}
