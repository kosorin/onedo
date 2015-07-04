using System.Collections.Generic;

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
