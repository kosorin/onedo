using SimpleTasks.Core.Helpers;
using SimpleTasks.Helpers;
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

        public TaskWrapper(TaskModel task)
        {
            Task = task;
        }
    }
}
