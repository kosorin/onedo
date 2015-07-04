using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using SimpleTasks.Core.Helpers;

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
