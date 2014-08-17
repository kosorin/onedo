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
    public partial class SmallTaskTile : TaskTileControl
    {
        public SmallTaskTile()
        {
            InitializeComponent();
        }

        public SmallTaskTile(TaskModel task)
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
            Title.FontSize = settings.LineHeight * 0.72;
            Title.Text = task.Title;

            LayoutRoot.Background = new SolidColorBrush(Colors.Transparent);
            //LayoutRoot.Background = new SolidColorBrush(task.Color) { Opacity = settings.BackgroundOpacity };
        }
    }
}
