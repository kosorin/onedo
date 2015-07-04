using System.Windows;
using SimpleTasks.Core.Models;

namespace SimpleTasks.Core.Tiles
{
    public abstract class TaskTileControl : TileControl
    {
        public TaskModel Task
        {
            get { return (TaskModel)GetValue(TaskProperty); }
            set { SetValue(TaskProperty, value); }
        }
        public static readonly DependencyProperty TaskProperty =
            DependencyProperty.Register("Task", typeof(TaskModel), typeof(TaskTileControl), null);

        public TaskTileControl() { }

        public TaskTileControl(TaskModel task)
        {
            Task = task;
        }
    }
}
