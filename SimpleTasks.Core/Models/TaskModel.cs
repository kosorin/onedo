using SimpleTasks.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SimpleTasks.Core.Models
{
    [DataContract(Name = "Task", Namespace = "")]
    public class TaskModel : BindableBase
    {
        public TaskModel()
        {
            Uid = Guid.NewGuid().ToString();
            Created = DateTime.Now;
        }

        #region Uid
        private string _uid = "";
        [DataMember(Order = 0)]
        public string Uid
        {
            get
            {
                return _uid;
            }
            private set
            {
                SetProperty(ref _uid, value);
            }
        }
        #endregion

        #region Title
        private string _title = "";
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
                Modified = DateTime.Now;
            }
        }
        #endregion

        #region Detail
        private string _detail = "";
        [DataMember(Order = 2)]
        public string Detail
        {
            get
            {
                return _detail;
            }
            set
            {
                SetProperty(ref _detail, value);
                Modified = DateTime.Now;
            }
        }
        #endregion

        #region Priority
        private TaskPriority _priority = TaskPriority.Normal;
        [DataMember(Order = 3)]
        public TaskPriority Priority
        {
            get
            {
                return _priority;
            }
            set
            {
                SetProperty(ref _priority, value);
                OnPropertyChanged("IsLowPriority");
                OnPropertyChanged("IsNormalPriority");
                OnPropertyChanged("IsHighPriority");
                Modified = DateTime.Now;
            }
        }

        public bool IsLowPriority
        {
            get { return Priority == TaskPriority.Low; }
        }

        public bool IsNormalPriority
        {
            get { return Priority == TaskPriority.Normal; }
        }

        public bool IsHighPriority
        {
            get { return Priority == TaskPriority.High; }
        }
        #endregion

        #region Due Date
        private DateTime? _dueDate = null;
        [DataMember(Order = 4)]
        public DateTime? DueDate
        {
            get
            {
                return _dueDate;
            }
            set
            {
                SetProperty(ref _dueDate, value);
                OnPropertyChanged("HasDueDate");
                OnPropertyChanged("IsOverdue");
                OnPropertyChanged("Reminder");
                OnPropertyChanged("ReminderDate");
                OnPropertyChanged("HasReminder");
                Modified = DateTime.Now;
            }
        }

        public bool HasDueDate { get { return DueDate != null; } }

        public bool IsOverdue
        {
            get
            {
                if (DueDate == null)
                    return false;
                else
                    return (DueDate < DateTime.Now);
            }
        }
        #endregion

        #region Reminder
        private TimeSpan? _reminder = null;
        [DataMember(Order = 5)]
        public TimeSpan? Reminder
        {
            get { return _reminder; }
            set
            {
                SetProperty(ref _reminder, value);
                OnPropertyChanged("HasReminder");
                Modified = DateTime.Now;
            }
        }

        public DateTime ReminderDate
        {
            get
            {
               if (DueDate == null || Reminder == null)
               {
                   throw new InvalidOperationException();
               }
               return DueDate.Value - Reminder.Value;
            }
        }

        public bool HasReminder
        {
            get { return DueDate != null && Reminder != null; }
        }

        #region ReminderDateObsolete
        private DateTime? _reminderDateObsolete = null;
        [Obsolete("Smazat po několika aktualizacích (dnes je 13.7.2014)")]
        [DataMember(Order = 5, Name = "ReminderDate")]
        public DateTime? ReminderDateObsolete
        {
            get { return null; }
            set { SetProperty(ref _reminderDateObsolete, value); }
        }

        [Obsolete("Smazat po několika aktualizacích (dnes je 13.7.2014)")]
        public DateTime? ReminderDateObsoleteGet
        {
            get { return _reminderDateObsolete; }
        }
        #endregion

        #endregion

        #region Complete
        private DateTime? _completedDate = null;
        [DataMember(Order = 6)]
        public DateTime? CompletedDate
        {
            get
            {
                return _completedDate;
            }
            set
            {
                SetProperty(ref _completedDate, value);
                OnPropertyChanged("IsComplete");
                OnPropertyChanged("IsActive");
                Modified = DateTime.Now;
            }
        }

        public bool IsComplete { get { return CompletedDate.HasValue; } }

        public bool IsActive { get { return !CompletedDate.HasValue; } }
        #endregion

        #region Tags
        private List<string> _tags = new List<string>();
        [DataMember(Order = 10)]
        public List<string> Tags
        {
            get
            {
                return _tags;
            }
            set
            {
                SetProperty(ref _tags, value);
                Modified = DateTime.Now;
            }
        }
        #endregion

        #region Created
        private DateTime? _created = null;
        [DataMember(Order = 7)]
        public DateTime? Created
        {
            get
            {
                return _created;
            }
            private set
            {
                SetProperty(ref _created, value);
            }
        }
        #endregion

        #region Modified
        private DateTime? _modified = null;
        [DataMember(Order = 8)]
        public DateTime? Modified
        {
            get
            {
                return _modified;
            }
            private set
            {
                SetProperty(ref _modified, value);
            }
        }

        private bool _modifiedSinceStart = false;
        public bool ModifiedSinceStart
        {
            get { return _modifiedSinceStart; }
            set { _modifiedSinceStart = value; }
        }
        #endregion
    }
}
