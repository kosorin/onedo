using SimpleTasks.Core.Helpers;
using System;
using System.Runtime.Serialization;

namespace SimpleTasks.Core.Models
{
    [DataContract(Name = "Task", Namespace = "")]
    public class TaskModel : BindableBase
    {
        #region Uid
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
        #endregion

        #region Title
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
                SetProperty(ref _title, value.NormalizeNewLines());
                OnPropertyChanged("TitleFirstLine");
                OnPropertyChanged("TitleDescription");
            }
        }

        public string TitleFirstLine
        {
            get
            {
                int position = _title.IndexOf(Environment.NewLine);
                if (position == -1)
                    return _title;
                return _title.Substring(0, position);
            }
        }

        public string TitleDescription
        {
            get
            {
                int position = _title.IndexOf(Environment.NewLine);
                if (position == -1)
                    return null;
                return _title.Substring(position + 1);
            }
        }

        #endregion

        #region Due Date
        private DateTime? _dueDate = null;
        [DataMember(Order = 2)]
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
                    return (DueDate < DateTimeExtensions.Today);
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
        #endregion

        #region Complete
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
            }
        }

        public bool HasReminder { get { return ReminderDate != null; } }
        #endregion

        public TaskModel()
        {
            Uid = Guid.NewGuid().ToString();
        }

        public void Update(TaskModel newTask)
        {
            Title = newTask.Title;
            DueDate = newTask.DueDate;
            Priority = newTask.Priority;
            CompletedDate = newTask.CompletedDate;
            ReminderDate = newTask.ReminderDate;
        }

        public TaskModel Clone()
        {
            TaskModel task = new TaskModel()
            {
                Uid = this.Uid,
                Title = this.Title,
                DueDate = this.DueDate,
                Priority = this.Priority,
                CompletedDate = this.CompletedDate,
                ReminderDate = this.ReminderDate,
            };
            return task;
        }
    }
}
