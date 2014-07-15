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
    public class BooleanToNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string param = parameter as string ?? "1";
            bool negate = param.StartsWith("!");

            double number = System.Convert.ToDouble(negate ? param.Substring(1) : param);
            return (value is bool && (bool)value) ? number : 0d;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
