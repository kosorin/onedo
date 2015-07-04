using System.Collections.Generic;
using System.Windows;
using SimpleTasks.Core.Models;

namespace SimpleTasks.Core.Tiles
{
    public abstract class ListTileControl : TileControl
    {
        public IList<TaskModel> Tasks
        {
            get { return (IList<TaskModel>)GetValue(TasksProperty); }
            set { SetValue(TasksProperty, value); }
        }
        public static readonly DependencyProperty TasksProperty =
            DependencyProperty.Register("Tasks", typeof(IList<TaskModel>), typeof(ListTileControl), null);

        public ListTileControl() { }

        public ListTileControl(IList<TaskModel> tasks)
        {
            Tasks = tasks;
        }
    }
}
