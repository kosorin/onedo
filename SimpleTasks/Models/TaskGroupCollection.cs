using SimpleTasks.Core.Helpers;
using SimpleTasks.Core.Models;
using SimpleTasks.Resources;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SimpleTasks.Models
{
    public abstract class TaskGroupCollection : ObservableCollection<TaskGroup>
    {
        public TaskCollection Tasks { get; private set; }

        public TaskGroupCollection(TaskCollection tasks)
        {
            Tasks = tasks;
        }

        protected abstract TaskGroup GetGroup(TaskModel task);

        public void AddTask(TaskModel task)
        {
            task.Wrapper = new TaskWrapper(task);
            GetGroup(task).Add(task);
        }

        public void AddSortedTask(TaskModel task)
        {
            task.Wrapper = new TaskWrapper(task);
            GetGroup(task).AddSorted(task);
        }

        public void RemoveTask(TaskModel task)
        {
            foreach (TaskGroup group in this)
            {
                group.Remove(task);
            }
        }
    }

    public class DateTaskGroupCollection : TaskGroupCollection
    {
        private TaskGroup _overdueGroup = null;
        private TaskGroup _todayGroup = null;
        private TaskGroup _tomorrowGroup = null;
        private TaskGroup _upcomingGroup = null;
        private TaskGroup _somedayGroup = null;
        private TaskGroup _completedGroup = null;

        public DateTaskGroupCollection(TaskCollection tasks)
            : base(tasks)
        {
            Comparer<TaskModel> completedComparer = Comparer<TaskModel>.Create((t1, t2) => t2.Completed.Value.CompareTo(t1.Completed.Value));
            Comparer<TaskModel> somedayComparer = Comparer<TaskModel>.Create((t1, t2) => t2.Priority.CompareTo(t1.Priority));
            Comparer<TaskModel> dateComparer = Comparer<TaskModel>.Create((t1, t2) =>
            {
                int cmp = t1.DueDate.Value.CompareTo(t2.DueDate.Value);
                if (cmp == 0)
                {
                    return t2.Priority.CompareTo(t1.Priority);
                }
                return cmp;
            });

            _completedGroup = new TaskGroup(AppResources.DateCompleted, completedComparer);
            _somedayGroup = new TaskGroup(AppResources.DateSomeday, somedayComparer);
            _overdueGroup = new TaskGroup(AppResources.DateOverdue, dateComparer);
            _todayGroup = new TaskGroup(AppResources.DateToday, dateComparer);
            _tomorrowGroup = new TaskGroup(AppResources.DateTomorrow, dateComparer);
            _upcomingGroup = new TaskGroup(AppResources.DateUpcoming, dateComparer);

            foreach (TaskModel task in tasks)
            {
                AddTask(task);
            }

            _completedGroup.Sort();
            _somedayGroup.Sort();
            _overdueGroup.Sort();
            _todayGroup.Sort();
            _tomorrowGroup.Sort();
            _upcomingGroup.Sort();

            Add(_overdueGroup);
            Add(_todayGroup);
            Add(_tomorrowGroup);
            Add(_upcomingGroup);
            Add(_somedayGroup);
            Add(_completedGroup);
        }

        protected override TaskGroup GetGroup(TaskModel task)
        {
            if (task.IsCompleted)
            {
                return _completedGroup;
            }
            else if (!task.HasDueDate)
            {
                return _somedayGroup;
            }
            else if (task.DueDate.Value >= DateTimeExtensions.Tomorrow.AddDays(1))
            {
                return _upcomingGroup;
            }
            else if (task.DueDate.Value >= DateTimeExtensions.Tomorrow)
            {
                return _tomorrowGroup;
            }
            else if (task.DueDate.Value >= DateTimeExtensions.Today)
            {
                return _todayGroup;
            }
            else
            {
                return _overdueGroup;
            }
        }
    }
}
