using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SimpleTasks.Conventers
{
    public class AmPmToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool?)
            {
                bool? showAmPm = (bool?)value;
                return (showAmPm.HasValue ? (showAmPm.Value ? CultureInfo.CurrentCulture.DateTimeFormat.AMDesignator : CultureInfo.CurrentCulture.DateTimeFormat.PMDesignator).ToLower() : "");
            }
            else
                return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
