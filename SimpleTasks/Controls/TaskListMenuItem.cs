using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SimpleTasks.Controls
{
    public class TaskListMenuItem : MenuItem
    {
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(string), typeof(TaskListMenuItem), new PropertyMetadata("\ue17e"));
        public string Icon
        {
            get { return (string)base.GetValue(IconProperty); }
            set { base.SetValue(IconProperty, value); }
        }
    }
}
