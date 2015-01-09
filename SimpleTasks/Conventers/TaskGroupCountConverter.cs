using System;
using System.Globalization;
using System.Windows.Data;
using Microsoft.Phone.Controls.LocalizedResources;
using SimpleTasks.Resources;

namespace SimpleTasks.Conventers
{
    public class TaskGroupCountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                int count = (int)value;
                string str = "";
                if (count == 1) str = string.Format(AppResources.Tasks1, count);
                else if (count >= 2 && count <= 4) str = string.Format(AppResources.Tasks2To4, count);
                else str = string.Format(AppResources.Tasks5AndMore, count);
                return str.ToUpper();
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}