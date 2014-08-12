using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SimpleTasks.Core.Controls;
using System.Windows;
using System.Windows.Shapes;

namespace SimpleTasks.Core.Tiles
{
    [TemplatePart(Name = "Icon", Type = typeof(Canvas))]
    [TemplatePart(Name = "Text", Type = typeof(TextBlock))]
    [TemplatePart(Name = "Strike", Type = typeof(Border))]
    public partial class SubtaskControl : UserControl
    {
        public SubtaskControl()
        {
            InitializeComponent();
        }
    }
}
