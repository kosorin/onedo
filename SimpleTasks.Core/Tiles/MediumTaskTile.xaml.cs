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

namespace SimpleTasks.Core.Tiles
{
    public partial class MediumTaskTile : TileControl
    {
        public double LineSize
        {
            get { return (double)GetValue(LineSizeProperty); }
            set { SetValue(LineSizeProperty, value); }
        }
        public static readonly DependencyProperty LineSizeProperty =
            DependencyProperty.Register("LineSize", typeof(double), typeof(MediumTaskTile), new PropertyMetadata(48D));

        public MediumTaskTile()
        {
            InitializeComponent();
            DataContext = null;
        }

        public MediumTaskTile(TaskModel task, double lineSize)
        {
            InitializeComponent();
            Refresh(task, lineSize);
        }

        public void Refresh(TaskModel task, double lineSize)
        {
            DataContext = task;
            LineSize = lineSize;
        }
    }
}
