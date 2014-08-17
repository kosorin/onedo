using SimpleTasks.Core.Helpers;
using SimpleTasks.Core.Models;
using SimpleTasks.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SimpleTasks.Models
{
    public class TaskGroup : List<TaskModel>
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

        public new void Sort()
        {
            base.Sort(_comparer);
        }
    }
}
