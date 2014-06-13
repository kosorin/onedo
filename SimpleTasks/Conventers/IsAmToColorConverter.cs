using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace SimpleTasks.Conventers
{
    public class IsAmToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isAm = (bool) value;
            var par = (string) parameter;

            if (isAm)
            {
                if (par == "AMText")
                    return Application.Current.Resources["PhoneAccentBrush"] as SolidColorBrush;
                if (par == "PMText")
                    return Application.Current.Resources["PhoneForegroundBrush05"] as SolidColorBrush;
                if (par == "HoursBg")
                    return Application.Current.Resources["PhoneAccentBrush03"] as SolidColorBrush;
                if (par == "HoursFg")
                    return Application.Current.Resources["PhoneAccentBrush07"] as SolidColorBrush;
            }
            else
            {
                if (par == "AMText")
                    return Application.Current.Resources["PhoneForegroundBrush05"] as SolidColorBrush;
                if (par == "PMText")
                    return Application.Current.Resources["PhoneAccentBrush"] as SolidColorBrush;
                if (par == "HoursBg")
                    return Application.Current.Resources["PhoneAccentBrush08"] as SolidColorBrush;
                if (par == "HoursFg")
                    return Application.Current.Resources["PhoneAccentBrush"] as SolidColorBrush;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
