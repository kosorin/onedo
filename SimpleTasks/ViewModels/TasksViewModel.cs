using Microsoft.Phone.Scheduler;
using SimpleTasks.Core.Helpers;
using SimpleTasks.Core.Models;
using SimpleTasks.Helpers;
using SimpleTasks.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace SimpleTasks.ViewModels
{
    public class TasksViewModel : BindableBase
    {
        public readonly string DataFileName = "Tasks.json";

        private TaskCollection _tasks = new TaskCollection();
        public TaskCollection Tasks
        {
            get
            {
                return _tasks;
            }
            private set
            {
                SetProperty(ref _tasks, value);
                OnPropertyChanged(GroupedTasksPropertyString);

                if (_tasks != null)
                {
                    _tasks.CollectionChanged += (s, e) => { OnPropertyChanged(GroupedTasksPropertyString); };
                }
            }
        }

        public string GroupedTasksPropertyString = "GroupedTasks";
        public List<TaskGroup> GroupedTasks
        {
            get { return TaskGroup.CreateGroups(Tasks); }
        }

        public TasksViewModel()
        {
        }

        public void Load()
        {
            Tasks = TaskCollection.LoadFromFile(DataFileName);
#if DEBUG
            Debug.WriteLine("> Nahrané úkoly ({0}):", Tasks.Count);
            foreach (TaskModel task in Tasks)
            {
                Reminder reminder = ReminderHelper.Get(task);
                Debug.WriteLine(": {0} [připomenutí: {1}]", task.Title, reminder != null ? reminder.Name : "<false>");
            }
#endif
        }

        public void Save()
        {
            TaskCollection.SaveToFile(DataFileName, Tasks);
        }

        public void Add(TaskModel task)
        {
            Tasks.Add(task);
            if (task.IsActive && task.HasReminder)
            {
                ReminderHelper.Add(task);
            }
        }

        public void Update(TaskModel task)
        {
            Reminder reminder = ReminderHelper.Get(task);
            if (reminder != null)
            {
                if (task.IsComplete || !task.HasReminder || !reminder.IsScheduled || reminder.BeginTime != task.ReminderDate)
                {
                    ReminderHelper.Remove(task);
                    reminder = null;
                }
            }

            if (reminder == null && task.IsActive && task.HasReminder)
            {
                ReminderHelper.Add(task);
            }
        }

        public void Delete(TaskModel task)
        {
            Tasks.Remove(task);
            ReminderHelper.Remove(task);
            LiveTile.Unpin(task);
        }

        public void DeleteCompleted(int days)
        {
            // Odstranění úkolů, které byly odstarněny před více jak 'days' dny.
            var completedTasks = Tasks.Where((t) =>
            {
                if (t.IsComplete)
                {
                    return (DateTime.Now - t.CompletedDate.Value) >= TimeSpan.FromDays(days);
                }
                else
                {
                    return false;
                }
            }).ToList();
            foreach (TaskModel task in completedTasks)
            {
                Delete(task);
            }
        }

        public void DeleteCompleted()
        {
            // Odstranění dokončených úkolů
            foreach (TaskModel task in Tasks.Where(t => t.IsComplete).ToList())
            {
                Delete(task);
            }
        }

        public void DeleteAll()
        {
            foreach (TaskModel task in Tasks)
            {
                ReminderHelper.Remove(task);
                LiveTile.Unpin(task);
            }
            Tasks.Clear();
        }
    }
}
