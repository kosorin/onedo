using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
namespace SimpleTasks.Conventers
{
    public class TimeSpanToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value !=null)
                Debug.WriteLine("KONV: " + value.GetType());
            else
                Debug.WriteLine("KONV: NIC");
            TimeSpan? ts = value as TimeSpan?;
            if (ts == null)
            {
                return "žádné";
            }
            return ts.Value.ToString("d'd 'hh'h 'mm'm 'ss's '");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
