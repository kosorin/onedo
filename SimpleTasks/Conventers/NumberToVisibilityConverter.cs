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
            bool negate = false;

            if (parameter != null && parameter is string)
            {
                string p = (string)parameter;
                if (p.StartsWith("!"))
                {
                    negate = true;
                    p = p.Substring(1);
                }
                int.TryParse(p, out parameterValue);
            }

            if (negate)
                return (value is int && (int)value == parameterValue) ? Visibility.Collapsed : Visibility.Visible;
            else
                return (value is int && (int)value == parameterValue) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
