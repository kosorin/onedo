using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SimpleTasks.Core.Conventers
{
    public class NumberMultConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((double)value) * double.Parse((string)parameter, CultureInfo.InvariantCulture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
