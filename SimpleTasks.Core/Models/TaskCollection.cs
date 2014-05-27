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
    }
}
