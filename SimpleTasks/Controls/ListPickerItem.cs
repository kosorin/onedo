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

        public ListPickerItem(string label)
        {
            Label = label;
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

        public ListPickerItem(string label, T value) :
            base(label)
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

        public ListPickerItem(string label, T1 value1, T2 value2) :
            base(label)
        {
            Value1 = value1;
            Value2 = value2;
        }
    }
}
