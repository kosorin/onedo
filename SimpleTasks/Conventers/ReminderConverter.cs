using SimpleTasks.Resources;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Data;
namespace SimpleTasks.Conventers
{
    public class ReminderConverter : IValueConverter
    {
        private string FormattedText(int value, string format1, string format2To4, string format5AndMore)
        {
            if (value == 1)
            {
                return string.Format(format1, value);
            }
            else if (value >= 2 && value <= 4)
            {
                return string.Format(format2To4, value);
            }
            else
            {
                return string.Format(format5AndMore, value);
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string format = parameter as string ?? "{0}";// +AppResources.ReminderBeforeDueDateText;
            TimeSpan ts = value as TimeSpan? ?? TimeSpan.Zero;

            if (ts.Days != 0)
            {
                int days = ts.Days % 7;
                int weeks = ts.Days / 7;
                if (weeks > 0 && weeks < 5)
                {
                    string text = FormattedText(weeks, AppResources.Weeks1, AppResources.Weeks2To4, "");
                    if (days > 0)
                    {
                        text += " + " + FormattedText(days, AppResources.Days1, AppResources.Days2To4, AppResources.Days5AndMore);
                    }
                    return string.Format(format, text);
                }
                else
                {
                    return string.Format(format, FormattedText(ts.Days, AppResources.Days1, AppResources.Days2To4, AppResources.Days5AndMore));
                }
            }
            else if (ts.Hours != 0)
            {
                return string.Format(format, FormattedText(ts.Hours, AppResources.Hours1, AppResources.Hours2To4, AppResources.Hours5AndMore));
            }
            else if (ts.Minutes != 0)
            {
                return string.Format(format, FormattedText(ts.Minutes, AppResources.Minutes1, AppResources.Minutes2To4, AppResources.Minutes5AndMore));
            }
            else
            {
                return AppResources.TimeZero;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
