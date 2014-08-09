using SimpleTasks.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTasks.Controls
{
    public class ListPickerValueItem<T> : ListPickerItem
    {
        #region Value
        private T _value;
        public T Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }
        #endregion

        public ListPickerValueItem(T value, string label, double opacity = -1) :
            base(label, opacity)
        {
            Value = value;
        }
    }
}
