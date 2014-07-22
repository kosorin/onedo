using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTasks.Models
{
    public class ChangelogList : List<ChangelogCategory>
    {
        public ChangelogList() { }

        public void AddCategory(ChangelogCategory category)
        {
            base.Add(category);
        }
    }
}
