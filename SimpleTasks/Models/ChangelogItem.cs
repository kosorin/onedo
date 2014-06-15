using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTasks.Models
{
    public class ChangelogItem
    {
        public string Text { get; set; }

        public ChangelogItem(string text)
        {
            Text = text;
        }
    }
}
