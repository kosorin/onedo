using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SimpleTasks.Controls
{
    public class ListPicker : Microsoft.Phone.Controls.ListPicker
    {
        public ListPicker()
        {
            DefaultStyleKey = typeof(ListPicker);
            SetValue(ItemCountThresholdProperty, 9);
        }


        public object LabelConverter
        {
            get { return (object)GetValue(LabelConverterProperty); }
            set { SetValue(LabelConverterProperty, value); }
        }
        public static readonly DependencyProperty LabelConverterProperty =
            DependencyProperty.Register("LabelConverter", typeof(object), typeof(ListPicker), new PropertyMetadata(null));


        public string LabelConverterParameter
        {
            get { return (string)GetValue(LabelConverterParameterProperty); }
            set { SetValue(LabelConverterParameterProperty, value); }
        }
        public static readonly DependencyProperty LabelConverterParameterProperty =
            DependencyProperty.Register("LabelConverterParameter", typeof(string), typeof(ListPicker), new PropertyMetadata(""));
    }
}
