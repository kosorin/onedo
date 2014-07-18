using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using SimpleTasks.Resources;
using SimpleTasks.Models;
using System.Reflection;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using SimpleTasks.Controls;

namespace SimpleTasks.Views
{
    public partial class AboutPage : BasePage
    {
        public AboutPage()
        {
            InitializeComponent();

            ChangelogList = CreateChangelog();
            DataContext = this;
        }

        #region Properties
        private ChangelogList CreateChangelog()
        {
            ChangelogList changelog = new ChangelogList();

            foreach (var version in JObject.Parse(AppResources.ChangelogFile))
            {
                JObject categoryData = (JObject)version.Value;

                ChangelogCategory category = new ChangelogCategory(version.Key, Convert.ToDateTime(categoryData["date"].ToString()));
                foreach (JToken item in (JArray)categoryData["items"])
                {
                    category.AddItem(item.ToString());
                }
                changelog.AddCategory(category);
            }

            return changelog;
        }

        public string VersionString { get { return string.Format(AppResources.AboutVersion, App.Version.ToString()); } }

        public ChangelogList ChangelogList { get; set; }
        #endregion

        #region Rate
        private void Rate_Click(object sender, RoutedEventArgs e)
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
            marketplaceReviewTask.Show();
        }
        #endregion

        #region Email
        private void Contact_Click(object sender, RoutedEventArgs e)
        {
            EmailComposeTask task = new EmailComposeTask
            {
                To = "kosorin@outlook.com",
                Subject = string.Format(AppResources.AboutEmailSubject, "Simple Tasks", App.Version.ToString())
            };
            task.Show();
        }
        #endregion
    }
}