using System;
using System.Globalization;
using System.Windows.Data;
using SimpleTasks.Core.Helpers;
using System.Diagnostics;

namespace SimpleTasks.Conventers
{
    public class DateToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime)
            {
                DateTime date = (DateTime)value;
                return date.Date >= DateTime.Today;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
