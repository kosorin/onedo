using SimpleTasks.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Xml;

namespace SimpleTasks.Old.Core.Models
{
    [DataContract(Name = "Task", Namespace = "")]
    public class TaskModel : BindableBase
    {
        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public DateTime? Date { get; set; }

        [DataMember]
        public bool IsImportant { get; set; }

        [DataMember]
        public bool IsComplete { get; set; }

        [DataMember]
        public DateTime? CompletedDate { get; set; }
    }

    [CollectionDataContract(Name = "Tasks", Namespace = "")]
    public class TaskModelCollection : ObservableCollection<TaskModel>
    {
        public TaskModelCollection() { }

        public TaskModelCollection(IEnumerable<TaskModel> tasks)
            : base(tasks)
        { }

        private const string TasksDataFileName = "TasksData.xml";

        static public TaskModelCollection LoadTasksFromXmlFile(string fileName)
        {
            TaskModelCollection tasks = new TaskModelCollection();

            Debug.WriteLine(string.Format("> Nahrávám data ze souboru {0}...", fileName));

            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                try
                {
                    using (IsolatedStorageFileStream rawStream = isf.OpenFile(fileName, FileMode.Open, FileAccess.Read))
                    {
                        DataContractSerializer serializer = new DataContractSerializer(typeof(TaskModelCollection));
                        XmlReader xmlReader = XmlReader.Create(rawStream);

                        try
                        {
                            tasks = serializer.ReadObject(xmlReader) as TaskModelCollection;
                            Debug.WriteLine("> Nahrávání dat dokončeno.");
                        }
                        catch (Exception)
                        {
                            Debug.WriteLine("Chyba při nahrávání dat.");
                        }
                        xmlReader.Close();
                    }
                }
                catch (Exception)
                {
                }
            }

            return tasks;
        }
    }
}
