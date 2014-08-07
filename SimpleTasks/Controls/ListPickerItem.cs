using SimpleTasks.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTasks.Controls
{
    public abstract class ListPickerItem : BindableBase
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

        public ListPickerItem(string label, double opacity)
        {
            Label = label;
            Opacity = opacity;
        }
    }
}
