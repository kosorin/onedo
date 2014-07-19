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

        #region Uid 0
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

        #region Title 1
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

        #region Detail 2
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

        #region DueDate 10
        private DateTime? _dueDate = null;
        [DataMember(Order = 10)]
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

        #region Reminder 11
        private TimeSpan? _reminder = null;
        [DataMember(Order = 11)]
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

        #region Priority 20
        private TaskPriority _priority = TaskPriority.Normal;
        [DataMember(Order = 20)]
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

        #region Tags 30
        private List<string> _tags = new List<string>();
        [DataMember(Name = "Tags", Order = 30)]
        public List<string> Tags
        {
            get { return _tags; }
            set { SetProperty(ref _tags, value); }
        }
        #endregion

        #region Created 100
        private DateTime? _created = null;
        [DataMember(Order = 100)]
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

        #region Modified 101
        private DateTime? _modified = null;
        [DataMember(Order = 101)]
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

        #region Completed 102
        private DateTime? _completed = null;
        [DataMember(Order = 102)]
        public DateTime? Completed
        {
            get
            {
                return _completed;
            }
            set
            {
                SetProperty(ref _completed, value);
                OnPropertyChanged("IsComplete");
                OnPropertyChanged("IsActive");
                Modified = DateTime.Now;
            }
        }

        public bool IsComplete { get { return Completed.HasValue; } }

        public bool IsActive { get { return !Completed.HasValue; } }

        #region CompletedDateObsolete
        private DateTime? _completedDateObsolete = null;
        [Obsolete("Smazat po několika aktualizacích (dnes je 19.7.2014)")]
        [DataMember(Order = 6, Name = "CompletedDate")]
        public DateTime? CompletedDateObsolete
        {
            get { return null; }
            set { SetProperty(ref _completedDateObsolete, value); }
        }

        [Obsolete("Smazat po několika aktualizacích (dnes je 19.7.2014)")]
        public DateTime? CompletedDateObsoleteGet
        {
            get { return _completedDateObsolete; }
        }
        #endregion

        #endregion
    }
}
