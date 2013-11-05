﻿using SimpleTasks.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;


namespace SimpleTasks.Core.Models
{
    [DataContract(Name = "Task", Namespace = "")]
    public class TaskModel : BindableBase
    {
        private string _uid = string.Empty;
        [DataMember(Order = 0)]
        public string Uid
        {
            get
            {
                return _uid;
            }
            set
            {
                SetProperty(ref _uid, value);
            }
        }

        private string _title = string.Empty;
        [DataMember(Order = 1)]
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                SetProperty(ref _title, value);
            }
        }

        private DateTime? _date = null;
        [DataMember(Order = 2)]
        public DateTime? Date
        {
            get
            {
                return _date;
            }
            set
            {
                SetProperty(ref _date, value);
            }
        }

        public bool IsOverdue
        {
            get
            {
                if (Date == null)
                    return false;
                else
                    return (Date < DateTimeExtensions.Today);
            }
        }

        private bool _isImportant = false;
        [DataMember(Order = 3)]
        public bool IsImportant
        {
            get
            {
                return _isImportant;
            }
            set
            {
                SetProperty(ref _isImportant, value);
            }
        }

        public bool IsComplete { get { return CompletedDate != null; } }

        public bool IsActive { get { return CompletedDate == null; } }

        private DateTime? _completedDate = null;
        [DataMember(Order = 4)]
        public DateTime? CompletedDate
        {
            get
            {
                return _completedDate;
            }
            set
            {
                SetProperty(ref _completedDate, value);
            }
        }

        private DateTime? _reminderDate = null;
        [DataMember(Order = 5)]
        public DateTime? ReminderDate
        {
            get
            {
                return _reminderDate;
            }
            set
            {
                SetProperty(ref _reminderDate, value);
            }
        }

        public bool HasReminder { get { return ReminderDate != null; } }

        public TaskModel()
        {
            Uid = Guid.NewGuid().ToString();
        }

        public TaskModel Clone()
        {
            TaskModel task = new TaskModel();
            task.Uid = Uid;
            task.Title = Title;
            task.Date = Date;
            task.IsImportant = IsImportant;
            task.CompletedDate = CompletedDate;
            task.ReminderDate = ReminderDate;
            return task;
        }
    }

    [CollectionDataContract(Name = "Tasks", Namespace = "")]
    public class TaskModelCollection : ObservableCollection<TaskModel>
    {
        public TaskModelCollection() { }

        public TaskModelCollection(IEnumerable<TaskModel> tasks)
            : base(tasks)
        { }

        private const string TasksDataFileName = "TasksData.xml";

        public int ActiveTaskCount
        {
            get
            {
                return this.Where(t => { return t.IsActive; }).Count();
            }
        }

        public List<TaskModel> SortedActiveTasks
        {
            get
            {
                // Vybere aktivní (nedokončené) úkoly a úkoly s termínem dokončení.
                // Uspořádá je podle termínu. Důležité úkoly ve stejném dnu mají přednost.
                List<TaskModel> tasks = this
                    .Where((t) => { return t.IsActive && t.Date != null; })
                    .OrderBy(t => t.Date.Value)
                    .ThenByDescending(t => t.IsImportant)
                    .ToList();

                // Přidá úkoly bez termínu na konec seznamu (opět uspořádané podle důležitosti).
                tasks.AddRange(this.Where((t) => { return t.IsActive; }).OrderByDescending(t => t.IsImportant));

                return tasks;
            }
        }

        static public TaskModelCollection LoadTasksFromXmlFile()
        {
            return LoadTasksFromXmlFile(TasksDataFileName);
        }

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

        static public void SaveTasksToXmlFile(TaskModelCollection tasks)
        {
            SaveTasksToXmlFile(tasks, TasksDataFileName);
        }

        static public void SaveTasksToXmlFile(TaskModelCollection tasks, string fileName)
        {
            Debug.WriteLine(string.Format("> Ukládám data do souboru {0}...", fileName));

            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream rawStream = isf.OpenFile(fileName, FileMode.Create, FileAccess.Write))
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(TaskModelCollection));
                    XmlWriter xmlWriter = XmlWriter.Create(rawStream, new XmlWriterSettings() { Indent = true });
                    serializer.WriteObject(xmlWriter, tasks);
                    xmlWriter.Flush();
                    xmlWriter.Close();
                    Debug.WriteLine("> Ukládání dat dokončeno.");
                }
            }
        }
    }
}
