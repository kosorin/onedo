using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SimpleTasks.Conventers
{
    public class BoolToNumberConverter : IValueConverter
    {
        /// <summary>
        /// Převede bool na double.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter">formát: (double-true) nebo (double-false);(double-true)</param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double falseNumber = 0d;
            double trueNumber = 1;

            string param = parameter as string;
            if (param == null)
            {
                return falseNumber;
            }

            if (param.Contains(';'))
            {
                string[] prms = param.Split(';');
                falseNumber = System.Convert.ToDouble(prms[0], CultureInfo.InvariantCulture);
                trueNumber = System.Convert.ToDouble(prms[1], CultureInfo.InvariantCulture);
            }
            else
            {
                trueNumber = System.Convert.ToDouble(param, CultureInfo.InvariantCulture);
            }
            return (value is bool && (bool)value) ? trueNumber : falseNumber;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
