using System;
using System.Globalization;
using System.Windows.Data;
using SimpleTasks.Core.Helpers;

namespace SimpleTasks.Conventers
{
    public class StringFunctionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = value as string;
            string function = parameter as string;
            if (str == null || string.IsNullOrWhiteSpace(function))
            {
                return "";
            }

            if (function == "ToUpper")
                return str.ToUpper();
            if (function == "ToLower")
                return str.ToLower();
            if (function == "Trim")
                return str.Trim();
            if (function == "TrimStart")
                return str.TrimStart();
            if (function == "TrimEnd")
                return str.TrimEnd();
            if (function == "FirstUpper")
                return str.FirstUpper();

            return str;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
