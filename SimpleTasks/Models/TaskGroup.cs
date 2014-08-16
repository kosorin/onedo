using SimpleTasks.Core.Helpers;
using SimpleTasks.Core.Models;
using SimpleTasks.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SimpleTasks.Models
{
    public class TaskGroup : ObservableCollection<TaskModel>
    {
        public string Title { get; private set; }

        public TaskGroup(string title)
        {
            Title = title;
        }

        public void AddSorted<T>(TaskModel task, Func<TaskModel, T> getValue, Func<int, bool> compare = null)
        {
            IComparer<T> comparer = Comparer<T>.Default;
            T value = getValue(task);
            if (compare == null)
                compare = v => v < 0;

            int i = 0;
            while (i < Count && compare(comparer.Compare(getValue(this[i]), value)))
            {
                i++;
            }
            Insert(i, task);
        }

        public void AddSorted<T, U>(TaskModel task, Func<TaskModel, T> getValue1, Func<TaskModel, U> getValue2, Func<int, bool> compare1 = null, Func<int, bool> compare2 = null)
        {
            IComparer<T> comparer1 = Comparer<T>.Default;
            IComparer<U> comparer2 = Comparer<U>.Default;
            if (compare1 == null)
                compare1 = v => v < 0;
            if (compare2 == null)
                compare2 = v => v < 0;

            T value1 = getValue1(task);
            U value2 = getValue2(task);

            int i = 0;
            while (i < Count)
            {
                int result = comparer1.Compare(getValue1(this[i]), value1);
                if (compare1(result))
                {
                    i++;
                }
                else if (result == 0 && compare2(comparer2.Compare(getValue2(this[i]), value2)))
                {
                    i++;
                }
                else
                {
                    break;
                }
            }
            Insert(i, task);
        }
    }
}
