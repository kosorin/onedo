using System;
using System.Globalization;
using System.Windows.Data;
using Microsoft.Phone.Controls.LocalizedResources;

namespace SimpleTasks.Conventers
{
    public class OffOnConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool? || value == null)
            {
                return (bool?)value == true ? ControlResources.On : ControlResources.Off;
            }
            return ControlResources.Off;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}