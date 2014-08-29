using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTasks.Controls
{
    public class ListPicker : Microsoft.Phone.Controls.ListPicker
    {
        public ListPicker()
        {
            DefaultStyleKey = typeof(ListPicker);
            SetValue(ItemCountThresholdProperty, 9);
        }
    }
}
