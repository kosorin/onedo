using SimpleTasks.Core.Models;
using System;
using System.Globalization;
using System.Windows.Data;

namespace SimpleTasks.Conventers
{
    public class TaskPriorityToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TaskPriority)
            {
                switch ((TaskPriority)value)
                {
                case TaskPriority.High: return true;
                case TaskPriority.Low: return false;
                case TaskPriority.Normal:
                default: return null;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? priority = value as bool?;

            if (priority == null)
            {
                return TaskPriority.Normal;
            }
            else if (priority.Value)
            {
                return TaskPriority.High;
            }
            else
            {
                return TaskPriority.Low;
            }
        }
    }
}
