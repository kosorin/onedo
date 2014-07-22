using SimpleTasks.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTasks.Models
{
    public class ReminderLengthType : BindableBase
    {
        private string _label;
        public string Label
        {
            get { return _label; }
            set { SetProperty(ref _label, value); }
        }

        private double _opacity = 1;
        public double Opacity
        {
            get { return _opacity; }
            set { SetProperty(ref _opacity, value); }
        }

        private int _maximum = 0;
        public int Maximum
        {
            get { return _maximum; }
            set { SetProperty(ref _maximum, value); }
        }

        public ReminderLengthType(int maximum, string label, double opacity)
        {
            Maximum = maximum;
            Label = label;
            Opacity = opacity;
        }

        public ReminderLengthType() { }
    }
}
