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

        protected abstract IEnumerable<TaskGroup> GetAllGroups();

        public TaskGroup AddTask(TaskModel task)
        {
            task.Wrapper = new TaskWrapper(task);
            TaskGroup group = GetGroup(task);
            group.Add(task);
            return group;
        }

        public TaskGroup AddSortedTask(TaskModel task)
        {
            task.Wrapper = new TaskWrapper(task);
            TaskGroup group = GetGroup(task);
            group.AddSorted(task);
            return group;
        }

        public void RemoveTask(TaskModel task)
        {
            foreach (TaskGroup group in this)
            {
                group.Remove(task);
            }
        }

        public void Update()
        {
            List<TaskModel> tasksToAdd = new List<TaskModel>();

            foreach (TaskGroup group in GetAllGroups())
            {
                for (int i = group.Count - 1; i >= 0; i--)
                {
                    TaskModel task = group[i];
                    if (GetGroup(task) != group)
                    {
                        tasksToAdd.Add(task);
                        group.RemoveAt(i);
                    }
                }
            }

            foreach (TaskModel task in tasksToAdd)
            {
                AddSortedTask(task);
            }

            foreach (TaskGroup group in GetAllGroups())
            {
                group.Sort();
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

        Comparer<TaskModel> _completedComparer;
        Comparer<TaskModel> _somedayComparer;
        Comparer<TaskModel> _dateComparer;

        public DateTaskGroupCollection(TaskCollection tasks)
            : base(tasks)
        {
            _completedComparer = Comparer<TaskModel>.Create((t1, t2) => t2.Completed.Value.CompareTo(t1.Completed.Value));
            _somedayComparer = Comparer<TaskModel>.Create((t1, t2) => t2.Priority.CompareTo(t1.Priority));
            _dateComparer = Comparer<TaskModel>.Create((t1, t2) =>
            {
                int cmp = t1.ActualDueDate.Value.CompareTo(t2.ActualDueDate.Value);
                if (cmp == 0)
                {
                    return t2.Priority.CompareTo(t1.Priority);
                }
                return cmp;
            });

            _completedGroup = new TaskGroup(AppResources.DateCompleted, _completedComparer);
            _somedayGroup = new TaskGroup(AppResources.DateSomeday, _somedayComparer);
            _overdueGroup = new TaskGroup(AppResources.DateOverdue, _dateComparer);
            _todayGroup = new TaskGroup(AppResources.DateToday, _dateComparer);
            _tomorrowGroup = new TaskGroup(AppResources.DateTomorrow, _dateComparer);
            _upcomingGroup = new TaskGroup(AppResources.DateUpcoming, _dateComparer);

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
            else if (task.ActualDueDate.Value >= DateTimeExtensions.Tomorrow.AddDays(1))
            {
                return _upcomingGroup;
            }
            else if (task.ActualDueDate.Value >= DateTimeExtensions.Tomorrow)
            {
                return _tomorrowGroup;
            }
            else if (task.ActualDueDate.Value >= DateTimeExtensions.Today)
            {
                return _todayGroup;
            }
            else
            {
                return _overdueGroup;
            }
        }

        protected override IEnumerable<TaskGroup> GetAllGroups()
        {
            yield return _overdueGroup;
            yield return _todayGroup;
            yield return _tomorrowGroup;
            yield return _upcomingGroup;
            yield return _somedayGroup;
            yield return _completedGroup;
        }
    }
}
