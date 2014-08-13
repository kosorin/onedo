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
using System.IO;

namespace SimpleTasks.Core.Tiles
{
    public partial class MediumTaskTile : TileControl
    {
        public MediumTaskTile(TaskModel task)
            : base(task as object)
        {
            InitializeComponent();
        }

        public override void Refresh()
        {
            TaskModel task = Data as TaskModel;
            if (task == null)
                return;

            DataContext = task;
            TileSettings settings = task.TileSettings ?? TileSettings.Default;

            // Title
            Title.FontSize = settings.LineHeight * 0.75;
            Title.Text = task.Title;

            // Detail
            Detail.Visibility = !string.IsNullOrWhiteSpace(task.Detail) ? Visibility.Visible : Visibility.Collapsed;
            if (!string.IsNullOrWhiteSpace(task.Detail))
            {
                Detail.FontSize = settings.LineHeight * 0.65;
                Detail.Text = task.Detail;
            }

            // Info
            InfoWrapper.Visibility = task.HasDueDate ? Visibility.Visible : Visibility.Collapsed;
            if (task.HasDueDate)
            {
                Info.Height = settings.LineHeight;
                Date.Text = task.DueDate.Value.ToShortDateString();
            }

            // Podúkoly
            Subtasks.Children.Clear();
            Subtasks.Visibility = task.Subtasks.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            foreach (Subtask subtask in task.Subtasks)
            {
                Viewbox vb = new Viewbox();
                vb.Height = settings.LineHeight;
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
            LayoutRoot.Background = new SolidColorBrush(task.Color) { Opacity = settings.BackgroundOpacity };
        }
    }
}
