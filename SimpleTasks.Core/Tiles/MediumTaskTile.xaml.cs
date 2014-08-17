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
using System.Diagnostics;

namespace SimpleTasks.Core.Tiles
{
    public partial class MediumTaskTile : TaskTileControl
    {
        public MediumTaskTile()
        {
            InitializeComponent();
        }

        public MediumTaskTile(TaskModel task)
            : base(task)
        {
            InitializeComponent();
        }

        public override void Refresh()
        {
            TaskModel task = Task;
            if (task == null)
                return;

            DataContext = task;
            TaskTileSettings settings = task.TileSettings ?? TaskTileSettings.Default;

            // Název
            if (settings.HideTitle)
            {
                TitleWrapper.Visibility = Visibility.Collapsed;
            }
            else
            {
                TitleWrapper.Visibility = Visibility.Visible;
                Title.FontSize = settings.LineHeight * 0.72;
                Title.LineHeight = settings.LineHeight;
                Title.Text = task.Title;
                Title.TextWrapping = settings.TitleOnOneLine ? TextWrapping.NoWrap : TextWrapping.Wrap;
            }

            // Date
            bool hideDate = settings.HideDate || !task.HasDueDate;
            InfoWrapper.Visibility = hideDate ? Visibility.Collapsed : Visibility.Visible;
            if (!hideDate)
            {
                Info.Height = settings.LineHeight;
                Date.Text = task.DueDate.Value.ToShortDateString();
            }

            // Podúkoly
            Subtasks.Children.Clear();
            List<Subtask> subtasks = new List<Subtask>(task.Subtasks.Where(s => settings.ShowCompletedSubtasks || !s.IsCompleted));
            Subtasks.Visibility = subtasks.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            foreach (Subtask subtask in subtasks.Take((336 / (int)settings.LineHeight) + 1))
            {
                SubtaskControl sc = new SubtaskControl(subtask);
                sc.Refresh(settings.LineHeight);
                Subtasks.Children.Add(sc);
            }

            // Detail
            Detail.Visibility = !string.IsNullOrWhiteSpace(task.Detail) ? Visibility.Visible : Visibility.Collapsed;
            if (!string.IsNullOrWhiteSpace(task.Detail))
            {
                Detail.FontSize = settings.LineHeight * 0.65;
                Detail.Text = task.Detail;
            }

            // Pozadí
            LayoutRoot.Background = new SolidColorBrush(Colors.Transparent);
            //LayoutRoot.Background = new SolidColorBrush(task.Color) { Opacity = settings.BackgroundOpacity };
        }
    }
}
