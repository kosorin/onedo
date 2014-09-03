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

            CountText.Text = tasks.Count.ToString();

            // Pozadí
            LayoutRoot.Background = new SolidColorBrush(Colors.Transparent);
            //LayoutRoot.Background = new SolidColorBrush(task.Color) { Opacity = settings.BackgroundOpacity };
        }
    }
}
