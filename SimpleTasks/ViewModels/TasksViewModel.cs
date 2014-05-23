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
                OnPropertyChanged(TagsPropertyString);

                if (_tasks != null)
                {
                    _tasks.CollectionChanged += (s, e) => { OnPropertyChanged(GroupedTasksPropertyString); };
                    _tasks.CollectionChanged += (s, e) => { OnPropertyChanged(TagsPropertyString); };
                }
            }
        }

        public ReadOnlyCollection<TaskModel> ReadOnlyTasks
        {
            get
            {
                return new ReadOnlyCollection<TaskModel>(_tasks);
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

        private const string TagsPropertyString = "Tags";
        public List<Tag> Tags
        {
            get
            {
                return new List<Tag>() 
                { 
                    new Tag() { Name = "dokončené" },
                    new Tag() { Name = "škola" },
                    new Tag() { Name = "všechny" }
                };
            }
        }

        public TasksViewModel()
        {
        }

        public void Load()
        {
            Tasks = TaskCollection.LoadFromFile(DataFileName);
#if DEBUG
            var reminders = new List<Reminder>(ScheduledActionService.GetActions<Reminder>());
            Debug.WriteLine("> Nahrané úkoly ({0}):", Tasks.Count);
            foreach (TaskModel task in Tasks)
            {
                if (ReminderHelper.Exists(task))
                    reminders.Remove(ReminderHelper.Get(task));
                Debug.WriteLine(": {0} [připomenutí: {1}]", task.Title, ReminderHelper.Exists(task));
            }

            Debug.WriteLine("> Přebývající připomenutí ({0}):", reminders.Count);
            foreach (var reminder in reminders)
            {
                Debug.WriteLine(": {1} ({0})", reminder.Name, reminder.Title);
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
            if (task.ReminderDate != null)
            {
                ReminderHelper.Add(task);
            }
        }

        public void Update(TaskModel oldTask, TaskModel newTask)
        {
            Delete(oldTask);
            Add(newTask);
        }

        public void Delete(TaskModel task)
        {
            Tasks.Remove(task);
            ReminderHelper.Remove(task);
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
            var completedTasks = Tasks.Where((t) => { return t.IsComplete; }).ToList();
            foreach (TaskModel task in completedTasks)
            {
                Delete(task);
            }
        }

        public void DeleteAll()
        {
            foreach (TaskModel task in Tasks)
            {
                ReminderHelper.Remove(task);
            }
            Tasks.Clear();
        }
    }
}
