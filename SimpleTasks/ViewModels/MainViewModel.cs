using SimpleTasks.Helpers;
using SimpleTasks.Models;
using System;
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

        private TaskModel _taskToEdit = null;
        public TaskModel TaskToEdit
        {
            get
            {
                return _taskToEdit;
            }
            set
            {
                SetProperty(ref _taskToEdit, value);
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
        }

        public void SaveData()
        {
            TaskModelCollection.SaveTasksToXmlFile(Tasks);
        }

        public void DeleteOldCompletedTasks(int days)
        {
            if (IsDataLoaded)
            {
                int before = Tasks.Count;

                // Odstranění dokončených úkolů starých 3 a více dnů
                var updatedTasks = Tasks.Where((t) =>
                {
                    if (t.IsComplete)
                    {
                        if (t.CompletedDate.HasValue)
                        {
                            //return (DateTime.Now - t.CompletedDate.Value < TimeSpan.FromSeconds(30));
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
                int before = Tasks.Count;

                // Odstranění dokončených úkolů
                Tasks = new TaskModelCollection(Tasks.Where((t) => { return !t.IsComplete; }));
            }
        }
    }
}
