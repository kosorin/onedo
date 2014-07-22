using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SimpleTasks.Conventers
{
    public class NumberToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int parameterValue = 0;
            if (parameter != null && parameter is string)
            {
                int.TryParse((string)parameter, out parameterValue);
            }
            return (value is int && (int)value == parameterValue) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
