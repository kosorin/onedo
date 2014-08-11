using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SimpleTasks.Core.Conventers
{
    public sealed class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool negate = parameter is string && (string)parameter== "!";
            if (negate)
                return (value is bool && (bool)value) ? Visibility.Collapsed : Visibility.Visible;
            else
                return (value is bool && (bool)value) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool negate = parameter is string && (string)parameter == "!";
            if (negate)
                return value is Visibility && (Visibility)value != Visibility.Visible;
            else
                return value is Visibility && (Visibility)value == Visibility.Visible;
        }
    }
}
