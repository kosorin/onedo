using Microsoft.Phone.Scheduler;
using SimpleTasks.Core.Helpers;
using SimpleTasks.Core.Models;
using SimpleTasks.Models;
using System;
using System.Diagnostics;
using System.Linq;

namespace SimpleTasks.ViewModels
{
    public class MainViewModel : BindableBase
    {
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
        public TaskKeyGroupCollection GroupedTasks
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
            Tasks = TaskCollection.LoadFromXmlFile(TaskCollection.DefaultDataFileName);
            if (Tasks == null)
            {
                Tasks = new TaskCollection();
            }
            IsDataLoaded = true;

            if (App.Settings.DeleteCompletedTasksSetting)
            {
                DeleteCompletedTasks(App.Settings.DeleteCompletedTasksDaysSetting);
            }

            //foreach (TaskModel task in Tasks)
            //{
            //    UpdateTaskReminder(task);
            //}

#if DEBUG
            Debug.WriteLine("> Úkoly ({0}):", Tasks.Count);
            foreach (TaskModel task in Tasks)
            {
                Debug.WriteLine("  {0}", task.Title);
            }

            Debug.WriteLine("> Připomenutí:");
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
            Tasks.SaveToXmlFile(TaskCollection.DefaultDataFileName);
        }

        public void AddTask(TaskModel task)
        {
            if (task == null)
                throw new ArgumentNullException();

            Tasks.Add(task);
            if (task.ReminderDate != null)
            {
                ReminderHelper.Add(task.Uid, "Připomínka úkolu", task.Title, task.ReminderDate.Value);
            }
            LiveTile.Update(Tasks);
        }

        public void UpdateTask(TaskModel oldTask, TaskModel newTask)
        {
            if (oldTask == null || newTask == null)
                throw new ArgumentNullException();

            Tasks.Remove(oldTask);
            Tasks.Add(newTask);
            if (newTask.ReminderDate != null)
            {
                ReminderHelper.Add(newTask.Uid, "Připomínka úkolu", newTask.Title, newTask.ReminderDate.Value);
            }
            LiveTile.Update(Tasks);
        }

        //public void UpdateTaskReminder(TaskModel task)
        //{
        //    if (task.HasReminder && task.ReminderDate <= DateTime.Now)
        //    {
        //        TaskReminder.Remove(task);
        //        task.ReminderDate = null;
        //    }
        //}

        public void RemoveTask(TaskModel task)
        {
            if (task == null)
                throw new ArgumentNullException();

            Tasks.Remove(task);
            ReminderHelper.Remove(task.Uid);
            LiveTile.Update(Tasks);
        }

        public void DeleteCompletedTasks(int days)
        {
            if (IsDataLoaded)
            {
                // Odstranění úkolů, které byly odstarněny před více jak 'days' dny.
                var completedTasks = Tasks.Where((t) =>
                {
                    return t.IsComplete && (DateTime.Today.Date - t.CompletedDate.Value.Date < TimeSpan.FromDays(days));
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
    }
}
