using System;
using System.Globalization;
using System.Windows.Data;
using SimpleTasks.Core.Helpers;
using SimpleTasks.Resources;
using System.Windows.Media;

namespace SimpleTasks.Conventers
{
    public class BrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = parameter as string ?? "AccentBrush:NormalBrush";

            if (value is bool && str.Contains(":"))
            {
                Brush brushTrue = App.Current.Resources[str.Substring(0, str.IndexOf(':'))] as Brush;
                Brush brushFalse = App.Current.Resources[str.Substring(str.IndexOf(':') + 1)] as Brush;
                return (bool)value ? brushTrue : brushFalse;
            }

            return new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
