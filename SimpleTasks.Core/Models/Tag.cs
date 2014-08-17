using SimpleTasks.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTasks.Core.Models
{
    public class Tag : BindableBase
    {
        private string _name = "";
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private string _iconPath = "";
        public string IconPath
        {
            get { return _iconPath; }
            set { SetProperty(ref _iconPath, value); }
        }

        private int _count = 0;
        public int Count
        {
            get { return (new Random()).Next(100); }
            set { SetProperty(ref _count, value); }
        }

        public Func<TaskModel, bool> Predicate { get; set; }

        private ReadOnlyCollection<TaskModel> _tasks;

        public Tag()
        { 
        }

        public Tag(ReadOnlyCollection<TaskModel> tasks)
        {
            //_tasks = tasks;

            //Predicate = (task) =>
            //{
            //    return task.Tags.Contains(Name);
            //};
        }
    }
}
