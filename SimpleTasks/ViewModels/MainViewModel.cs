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
        private TaskModelCollection _tasks = new TaskModelCollection();
        public TaskModelCollection Tasks
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
                    _tasks.CollectionChanged += (s, e) => { OnPropertyChanged(GroupedTasksPropertyString); };
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

        public void LoadData()
        {
            Tasks = TaskModelCollection.LoadTasksFromXmlFile();
            if (Tasks == null)
            {
                Tasks = new TaskModelCollection();
            }
            IsDataLoaded = true;

            if (App.Settings.DeleteCompletedTasksSetting)
                DeleteOldCompletedTasks(App.Settings.DeleteCompletedTasksDaysSetting);


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
        }

        public void SaveData()
        {
            TaskModelCollection.SaveTasksToXmlFile(Tasks);
        }

        public void DeleteOldCompletedTasks(int days)
        {
            if (IsDataLoaded)
            {
                // Odstranění dokončených úkolů starších více jak days dnů
                var updatedTasks = Tasks.Where((t) =>
                {
                    if (t.IsComplete)
                    {
                        if (t.CompletedDate != null)
                        {
                            return (DateTime.Today.Date - t.CompletedDate.Value.Date < TimeSpan.FromDays(days));
                        }
                    }
                    return true;
                });
                Tasks = new TaskModelCollection(updatedTasks);
            }
        }

        public void DeleteCompletedTasks()
        {
            if (IsDataLoaded)
            {
                // Odstranění dokončených úkolů
                Tasks = new TaskModelCollection(Tasks.Where((t) => { return t.IsActive; }));
            }
        }
    }
}
