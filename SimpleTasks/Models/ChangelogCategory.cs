using System;
using System.Collections.Generic;

namespace SimpleTasks.Models
{
    public class ChangelogCategory : List<ChangelogItem>
    {
        public string Version { get; set; }

        public DateTime Date { get; set; }

        public ChangelogCategory(string version, DateTime date)
        {
            Version = version;
            Date = date;
        }

        public void AddItem(string text)
        {
            Add(new ChangelogItem(text));
        }
    }
}
