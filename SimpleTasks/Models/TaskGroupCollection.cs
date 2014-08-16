using SimpleTasks.Core.Helpers;
using SimpleTasks.Core.Models;
using SimpleTasks.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTasks.Models
{
    public abstract class TaskGroupCollection : ObservableCollection<TaskGroup>, INotifyPropertyChanged
    {
        private TaskCollection _tasks = null;
        public TaskCollection Tasks
        {
            get { return _tasks; }
            set { SetProperty(ref _tasks, value); }
        }

        public TaskGroupCollection(TaskCollection tasks)
        {
            Tasks = tasks;
            CreateGroups();
            foreach (TaskModel task in Tasks)
            {
                AddTask(task);
            }
        }

        protected abstract void CreateGroups();

        public abstract void AddTask(TaskModel task);

        public void RemoveTask(TaskModel task)
        {
            foreach (TaskGroup group in this)
            {
                group.Remove(task);
            }
        }

        public void UpdateTask(TaskModel task)
        {
            RemoveTask(task);
            AddTask(task);
        }


        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChangedNew;

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChangedNew != null)
            {
                PropertyChangedNew(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }
            storage = value;

            OnPropertyChanged(propertyName);
            return true;
        }
        #endregion
    }

    public class DateTaskGroupCollection : TaskGroupCollection
    {
        public DateTaskGroupCollection(TaskCollection tasks)
            : base(tasks)
        {
        }

        private TaskGroup _overdueGroup = new TaskGroup(AppResources.DateOverdue);
        private TaskGroup _todayGroup = new TaskGroup(AppResources.DateToday);
        private TaskGroup _tomorrowGroup = new TaskGroup(AppResources.DateTomorrow);
        private TaskGroup _upcomingGroup = new TaskGroup(AppResources.DateUpcoming);
        private TaskGroup _somedayGroup = new TaskGroup(AppResources.DateSomeday);
        private TaskGroup _completedGroup = new TaskGroup(AppResources.DateCompleted);

        protected override void CreateGroups()
        {
            Add(_overdueGroup);
            Add(_todayGroup);
            Add(_tomorrowGroup);
            Add(_upcomingGroup);
            Add(_somedayGroup);
            Add(_completedGroup);
        }

        public override void AddTask(TaskModel task)
        {
            task.Wrapper = new TaskWrapper(task);
            if (task.IsCompleted)
            {
                _completedGroup.AddSorted<DateTime>(task, t => t.Completed.Value);
            }
            else if (!task.HasDueDate)
            {
                _somedayGroup.AddSorted<TaskPriority>(task, t => t.Priority);
            }
            else if (task.DueDate.Value.Date < DateTimeExtensions.Today)
            {
                _overdueGroup.AddSorted<DateTime>(task, t => t.DueDate.Value);
            }
            else if (task.DueDate.Value.Date == DateTimeExtensions.Today)
            {
                _todayGroup.AddSorted<DateTime, TaskPriority>(task, t => t.DueDate.Value, t => t.Priority, null, v => v > 0);
            }
            else if (task.DueDate.Value.Date == DateTimeExtensions.Tomorrow)
            {
                _tomorrowGroup.AddSorted<DateTime>(task, t => t.DueDate.Value);
            }
            else
            {
                _upcomingGroup.AddSorted<DateTime>(task, t => t.DueDate.Value);
            }
        }
    }
}
