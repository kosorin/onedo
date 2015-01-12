using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SimpleTasks.Controls;
using SimpleTasks.Models;
using System.Windows.Media;

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