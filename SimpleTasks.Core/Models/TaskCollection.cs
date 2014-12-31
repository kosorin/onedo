using Newtonsoft.Json;
using SimpleTasks.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace SimpleTasks.Core.Models
{
    [CollectionDataContract(Name = "Tasks", Namespace = "")]
    public class TaskCollection : ObservableCollection<TaskModel>
    {
        public TaskCollection() : base() { }

        public TaskCollection(IEnumerable<TaskModel> tasks) : base(tasks) { }

        public static TaskCollection LoadFromFile(string fileName)
        {
            TaskCollection tasks = FileHelper.ReadFromJson<TaskCollection>(fileName);
            foreach (TaskModel task in tasks)
            {
                task.UpdateRepeatsDueDate();
            }
            return tasks;
        }

        public static void SaveToFile(string fileName, TaskCollection tasks)
        {
            FileHelper.WriteToJson(fileName, tasks);
        }
    }
}
