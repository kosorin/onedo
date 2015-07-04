using SimpleTasks.Controls;
using SimpleTasks.Models;

namespace SimpleTasks.Views
{
    public partial class ChangelogPage : BasePage
    {
        public ChangelogList ChangelogList { get; set; }

        public ChangelogPage()
        {
            InitializeComponent();

            ChangelogList = App.LoadChangelog();
            DataContext = this;
        }
    }
}