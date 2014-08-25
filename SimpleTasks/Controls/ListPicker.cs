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

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (ItemsSource is List<ListPickerItem>)
            {
                List<ListPickerItem> list = (List<ListPickerItem>)ItemsSource;
                int size = list.Count;

                if (size > 0)
                {
                    double minimum = 0.4;
                    double step = (1d - minimum) / (double)size;

                    double opacity = 1;
                    foreach (ListPickerItem item in list)
                    {
                        if (item.Opacity < 0)
                        {
                            item.Opacity = opacity;
                        }
                        opacity -= step;
                    }
                }
            }
        }
    }
}
