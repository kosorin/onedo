using System;
using System.Globalization;
using System.Windows.Data;
using SimpleTasks.Core.Helpers;
using SimpleTasks.Resources;
using SimpleTasks.Core.Models;

namespace SimpleTasks.Conventers
{
    public class TaskToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TaskModel)
            {
                TaskModel task = (TaskModel)value;
                if (task.IsComplete)
                {
                    return 0.65;
                }
                else if (task.DueDate.HasValue)
                {
                    DateTime date = task.DueDate.Value;
                    if (date.Date == DateTimeExtensions.Today || date.Date == DateTimeExtensions.Tomorrow)
                    {
                        return 1.0;
                    }
                    else
                    {
                        return 0.65;
                    }
                }
            }

            return 1.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
