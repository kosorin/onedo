using SimpleTasks.Core.Helpers;
using SimpleTasks.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTasks.Models
{
    public class TaskWrapper : BindableBase
    {
        public TaskModel Task { get; set; }

        private double _height = -1;
        public double Height
        {
            get { return _height; }
            set { SetProperty(ref _height, value); }
        }

        public TaskWrapper(TaskModel task)
        {
            Task = task;
        }
    }
}
