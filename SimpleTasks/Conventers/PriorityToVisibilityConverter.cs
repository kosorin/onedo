﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using SimpleTasks.Core.Models;

namespace SimpleTasks.Conventers
{
    public class PriorityToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TaskPriority)
            {
                switch ((TaskPriority)value)
                {
                case TaskPriority.High:
                case TaskPriority.Low: return Visibility.Visible;

                case TaskPriority.Normal:
                default: return Visibility.Collapsed;;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
