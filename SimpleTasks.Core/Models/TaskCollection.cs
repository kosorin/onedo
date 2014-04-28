using Newtonsoft.Json;
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
            Debug.WriteLine(string.Format("> Nahrávám data ze souboru {0}...", fileName));

            TaskCollection tasks = new TaskCollection();
            try
            {
                using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (isf.FileExists(fileName))
                    {
                        using (Stream stream = isf.OpenFile(fileName, FileMode.Open, FileAccess.Read))
                        {
                            StreamReader sr = new StreamReader(stream);
                            tasks = JsonConvert.DeserializeObject<TaskCollection>(sr.ReadToEnd());
                            sr.Close();
                            Debug.WriteLine(": Nahrávání dat dokončeno.");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(": Chyba při nahrávání dat: {0}", e.Message);
            }

            return tasks;
        }

        public static void SaveToFile(string fileName, TaskCollection tasks)
        {
            Debug.WriteLine(string.Format("> Ukládám data do souboru {0}...", fileName));

            string data = JsonConvert.SerializeObject(tasks, Newtonsoft.Json.Formatting.Indented);
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (Stream stream = isf.OpenFile(fileName, FileMode.Create, FileAccess.Write))
                {
                    StreamWriter sw = new StreamWriter(stream);
                    sw.Write(data);
                    sw.Flush();
                    sw.Close();
                    Debug.WriteLine(": Ukládání dat dokončeno.");
                }
            }
        }

        #region DEPRECATED asi tak za měsíc nebo až vyjde Wp 8.1 toto smazat
        public static void ConvertOldXmlFile(string oldFileName, string newFileName)
        {
            Debug.WriteLine(string.Format("> Konvertuji staré data ze souboru {0} do {1}...", oldFileName, newFileName));

            TaskCollection tasks = new TaskCollection();
            try
            {
                using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (isf.FileExists(oldFileName))
                    {
                        using (Stream stream = isf.OpenFile(oldFileName, FileMode.Open, FileAccess.Read))
                        {
                            XDocument xml = XDocument.Load(stream, LoadOptions.SetLineInfo);

                            XElement root = xml.Root;
                            if (root.Name == "Tasks")
                            {
                                foreach (XElement taskNode in root.Elements())
                                {
                                    if (taskNode.Name.LocalName == "Task")
                                    {
                                        TaskModel task = new TaskModel();
                                        tasks.Add(task);
                                        foreach (XElement attributeNode in taskNode.Elements())
                                        {
                                            string value = attributeNode.Value;
                                            try
                                            {
                                                switch (attributeNode.Name.LocalName)
                                                {
                                                case "Uid":
                                                    task.Uid = value;
                                                    break;

                                                case "Title":
                                                    int position = value.IndexOf('\n');
                                                    if (position == -1)
                                                    {
                                                        task.Title = value.Trim();
                                                    }
                                                    else
                                                    {
                                                        task.Title = value.Substring(0, position).Trim();
                                                        task.Detail = value.Substring(position + 1).Trim();
                                                    }
                                                    break;

                                                case "Priority":
                                                    switch (value)
                                                    {
                                                    case "Low":
                                                        task.Priority = TaskPriority.Low;
                                                        break;
                                                    case "High":
                                                        task.Priority = TaskPriority.High;
                                                        break;
                                                    case "Normal":
                                                    default:
                                                        task.Priority = TaskPriority.Normal;
                                                        break;
                                                    }
                                                    break;

                                                case "IsImportant":
                                                    if (value == "true")
                                                        task.Priority = TaskPriority.High;
                                                    break;

                                                case "DueDate":
                                                case "Date":
                                                    if (value != "")
                                                        task.DueDate = DateTime.Parse(value);
                                                    break;

                                                case "ReminderDate":
                                                    if (value != "")
                                                        task.ReminderDate = DateTime.Parse(value);
                                                    break;

                                                case "CompletedDate":
                                                    if (value != "")
                                                        task.CompletedDate = DateTime.Parse(value);
                                                    break;

                                                case "IsComplete":
                                                    if (value == "true")
                                                        task.CompletedDate = DateTime.Now;
                                                    break;

                                                default:
                                                    break;
                                                }
                                            }
                                            catch (FormatException)
                                            {
                                            }
                                        }
                                        if (task.Uid == "")
                                            task.Uid = Guid.NewGuid().ToString();
                                    }
                                }
                            }
                            Debug.WriteLine(": Hotovo");
                        }

                        isf.DeleteFile(oldFileName);
                    }
                    else
                    {
                        Debug.WriteLine(": Není co převádět");
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(": Chyba při převádění souboru: {0}", e.Message);
            }

            SaveToFile(newFileName, tasks);
        }
        #endregion
    }
}
