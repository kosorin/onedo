using SimpleTasks.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTasks.Controls
{
    public class ListPickerItem : BindableBase
    {
        private string _label;
        public string Label
        {
            get { return _label; }
            set { SetProperty(ref _label, value); }
        }

        private double _opacity = 0;
        public double Opacity
        {
            get { return _opacity; }
            set { SetProperty(ref _opacity, value); }
        }

        public ListPickerItem(string label, double opacity = 0)
        {
            Label = label;
            Opacity = opacity;
        }

        public override string ToString()
        {
            return Label;
        }
    }

    public class ListPickerItem<T> : ListPickerItem
    {
        private T _value;
        public T Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }

        public ListPickerItem(T value, string label, double opacity = 0) :
            base(label, opacity)
        {
            Value = value;
        }
    }

    public class ListPickerItem<T1, T2> : ListPickerItem
    {
        private T1 _value1;
        public T1 Value1
        {
            get { return _value1; }
            set { SetProperty(ref _value1, value); }
        }

        private T2 _value2;
        public T2 Value2
        {
            get { return _value2; }
            set { SetProperty(ref _value2, value); }
        }

        public ListPickerItem(T1 value1, T2 value2, string label, double opacity = 0) :
            base(label, opacity)
        {
            Value1 = value1;
            Value2 = value2;
        }
    }
}
