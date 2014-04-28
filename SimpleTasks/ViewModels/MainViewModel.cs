using Microsoft.Phone.Scheduler;
using SimpleTasks.Core.Helpers;
using SimpleTasks.Helpers;
using SimpleTasks.Core.Models;
using SimpleTasks.Models;
using SimpleTasks.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SimpleTasks.ViewModels
{
    public class MainViewModel : BindableBase
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

        private const string GroupedTasksPropertyString = "GroupedTasks";
        public List<TaskKeyGroup> GroupedTasks
        {
            get
            {
                return TaskKeyGroup.CreateGroups(Tasks);
            }
        }

        public MainViewModel()
        {
            IsDataLoaded = false;
        }

        public bool IsDataLoaded { get; private set; }

        public void LoadTasks()
        {
            Tasks = TaskCollection.LoadFromFile(DataFileName);
            IsDataLoaded = true;

            if (App.Settings.DeleteCompletedTasksSetting)
            {
                DeleteCompletedTasks(App.Settings.DeleteCompletedTasksDaysSetting);
            }

#if DEBUG
            Debug.WriteLine("> Nahrané úkoly ({0}):", Tasks.Count);
            foreach (TaskModel task in Tasks)
            {
                Debug.WriteLine(": {0}", task.Title);
            }

            Debug.WriteLine("> Nahrané připomenutí ({0}):", Tasks.Count((t) =>
            {
                ScheduledAction reminder = ScheduledActionService.Find(t.Uid);
                if (reminder != null)
                {
                    return true;

                } return false;
            }));
            foreach (TaskModel task in Tasks)
            {
                ScheduledAction reminder = ScheduledActionService.Find(task.Uid);
                if (reminder != null)
                {
                    Debug.WriteLine("  [{0}] {1}", reminder.Name, task.Title);
                }
            }
#endif
        }

        public void SaveTasks()
        {
            TaskCollection.SaveToFile(DataFileName, Tasks);
        }

        public void AddTask(TaskModel task)
        {
            if (task == null)
                throw new ArgumentNullException();

            Tasks.Add(task);
            if (task.ReminderDate != null)
            {
                ReminderHelper.Add(task);
            }
        }

        public void UpdateTask(TaskModel oldTask, TaskModel newTask)
        {
            if (oldTask == null || newTask == null)
                throw new ArgumentNullException();

            ReminderHelper.Remove(oldTask.Uid);
            Tasks.Remove(oldTask);
            Tasks.Add(newTask);
            if (newTask.ReminderDate != null)
            {
                ReminderHelper.Add(newTask);
            }
        }

        public void RemoveTask(TaskModel task)
        {
            if (task == null)
                throw new ArgumentNullException();

            Tasks.Remove(task);
            ReminderHelper.Remove(task.Uid);
        }

        public void DeleteCompletedTasks(int days)
        {
            if (IsDataLoaded)
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
                    RemoveTask(task);
                }
            }
        }

        public void DeleteCompletedTasks()
        {
            if (IsDataLoaded)
            {
                // Odstranění dokončených úkolů
                var completedTasks = Tasks.Where((t) => { return t.IsComplete; }).ToList();
                foreach (TaskModel task in completedTasks)
                {
                    RemoveTask(task);
                }
            }
        }

        public void DeleteAllTasks()
        {
            if (IsDataLoaded)
            {
                foreach (TaskModel task in Tasks)
                {
                    ReminderHelper.Remove(task.Uid);
                }
                Tasks.Clear();
            }
        }
    }
}
