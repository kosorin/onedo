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
        public MediumTaskTile()
        {
            InitializeComponent();
            DataContext = null;
        }

        public TaskModel Task
        {
            get { return (TaskModel)GetValue(TaskProperty); }
            set
            {
                SetValue(TaskProperty, value);
                DataContext = value;
            }
        }
        public static readonly DependencyProperty TaskProperty =
            DependencyProperty.Register("Task", typeof(TaskModel), typeof(MediumTaskTile), null);
    }
}
