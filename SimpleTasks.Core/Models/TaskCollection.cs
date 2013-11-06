using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SimpleTasks.Core.Models
{
    [CollectionDataContract(Name = "Tasks", Namespace = "")]
    public class TaskCollection : ObservableCollection<TaskModel>
    {
        public TaskCollection() { }

        public TaskCollection(IEnumerable<TaskModel> tasks)
            : base(tasks)
        { }

        public List<TaskModel> SortedActiveTasks
        {
            get
            {
                // Vybere aktivní (nedokončené) úkoly a úkoly s termínem dokončení.
                // Uspořádá je podle termínu. Důležité úkoly ve stejném dnu mají přednost.
                List<TaskModel> tasks = this
                    .Where((t) => { return t.IsActive && t.DueDate != null; })
                    .OrderBy(t => t.DueDate.Value)
                    .ThenByDescending(t => t.IsImportant)
                    .ToList();

                // Přidá úkoly bez termínu na konec seznamu (opět uspořádané podle důležitosti).
                tasks.AddRange(this
                    .Where((t) => { return t.IsActive && t.DueDate == null; })
                    .OrderByDescending(t => t.IsImportant));

                return tasks;
            }
        }
        
        public const string DefaultDataFileName = "TasksData.xml";

        public static TaskCollection LoadFromXmlFile(string fileName)
        {
            TaskCollection tasks = new TaskCollection();

            Debug.WriteLine(string.Format("> Nahrávám data ze souboru {0}...", fileName));

            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                try
                {
                    using (IsolatedStorageFileStream rawStream = isf.OpenFile(fileName, FileMode.Open, FileAccess.Read))
                    {
                        DataContractSerializer serializer = new DataContractSerializer(typeof(TaskCollection));
                        XmlReader xmlReader = XmlReader.Create(rawStream);

                        try
                        {
                            tasks = serializer.ReadObject(xmlReader) as TaskCollection;
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

        public void SaveToXmlFile(string fileName)
        {
            Debug.WriteLine(string.Format("> Ukládám data do souboru {0}...", fileName));

            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream rawStream = isf.OpenFile(fileName, FileMode.Create, FileAccess.Write))
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(TaskCollection));
                    XmlWriter xmlWriter = XmlWriter.Create(rawStream, new XmlWriterSettings() { Indent = true });
                    serializer.WriteObject(xmlWriter, this);
                    xmlWriter.Flush();
                    xmlWriter.Close();
                    Debug.WriteLine("> Ukládání dat dokončeno.");
                }
            }
        }
    }
}
