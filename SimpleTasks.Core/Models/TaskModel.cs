using SimpleTasks.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SimpleTasks.Core.Models
{
    [DataContract(Name = "Task", Namespace = "")]
    public class TaskModel : BindableBase
    {
        #region Uid
        private string _uid = "";
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
                OnPropertyChanged("TitleFirstLine");
                OnPropertyChanged("TitleDescription");
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
                    return (DueDate < DateTime.Today);
            }
        }
        #endregion

        #region Reminder
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
                OnPropertyChanged(HasReminderName);
            }
        }

        private string HasReminderName = "HasReminder";
        public bool HasReminder { get { return ReminderDate != null; } }
        #endregion

        #region Complete
        private string IsCompleteName = "IsComplete";
        public bool IsComplete { get { return CompletedDate.HasValue; } }

        private string IsActiveName = "IsActive";
        public bool IsActive { get { return !CompletedDate.HasValue; } }

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
                OnPropertyChanged(IsCompleteName);
                OnPropertyChanged(IsActiveName);
            }
        }
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
            set
            {
                SetProperty(ref _created, value);
            }
        }
        #endregion

        #region Modified
        private DateTime? _modified = null;
        [DataMember(Order = 7)]
        public DateTime? Modified
        {
            get
            {
                return _modified;
            }
            set
            {
                SetProperty(ref _modified, value);
            }
        }
        #endregion

        public TaskModel()
        {
            Uid = Guid.NewGuid().ToString();
        }

        public TaskModel Clone()
        {
            TaskModel task = new TaskModel()
            {
                Uid = this.Uid,
                Title = this.Title,
                Detail = this.Detail,
                Priority = this.Priority,
                DueDate = this.DueDate,
                ReminderDate = this.ReminderDate,
                CompletedDate = this.CompletedDate,
            };
            return task;
        }
    }
}
