using System.Collections.Generic;
using System.Collections.ObjectModel;
using SimpleTasks.Core.Models;

namespace SimpleTasks.Models
{
    public class TaskGroup : ObservableCollection<TaskModel>
    {
        public string Title { get; private set; }

        private Comparer<TaskModel> _comparer;

        public TaskGroup(string title, Comparer<TaskModel> comparer)
        {
            Title = title;
            _comparer = comparer;
        }

        public void AddSorted(TaskModel task)
        {
            int i = 0;
            while (i < Count && _comparer.Compare(this[i], task) < 0)
            {
                i++;
            }
            Insert(i, task);
        }

        public void Sort()
        {
            List<TaskModel> list = new List<TaskModel>(this);
            list.Sort(_comparer);
            this.Clear();
            foreach (TaskModel task in list)
            {
                this.Add(task);
            }
        }
    }
}
