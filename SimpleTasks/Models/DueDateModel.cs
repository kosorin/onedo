using SimpleTasks.Core.Helpers;
using System;

namespace SimpleTasks.Models
{
    public class DueDateModel : BindableBase
    {
        public enum DueDatePickerType
        {
            NoDueDate,
            Date,
            CustomDate
        }

        public DueDatePickerType Type { get; set; }

        public string Title { get; set; }

        private DateTime? _date = null;
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
    }
}
