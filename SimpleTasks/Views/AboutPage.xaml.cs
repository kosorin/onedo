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

namespace SimpleTasks.Views
{
    public partial class AboutPage : PhoneApplicationPage
    {
        public AboutPage()
        {
            InitializeComponent();

            ChangelogList = CreateChangelog();
            DataContext = this;
        }

        private List<ChangelogCategory> CreateChangelog()
        {
            List<ChangelogCategory> changelog = new List<ChangelogCategory>();

            foreach (var version in JObject.Parse(AppResources.ChangelogFile))
            {
                JObject categoryData = (JObject)version.Value;

                ChangelogCategory category = new ChangelogCategory(version.Key, Convert.ToDateTime(categoryData["date"].ToString()));
                foreach (JToken item in (JArray)categoryData["items"])
                {
                    category.AddItem(item.ToString());
                }
                changelog.Add(category);
            }

            return changelog;
        }

        public static string Version
        {
            get
            {
                return App.Version.ToString();
            }
        }

        public List<ChangelogCategory> ChangelogList { get; set; }

        public static string ApplicationName { get { return "Simple Tasks"; } }

        public static string AuthorName { get { return "David Kosorin"; } }

        public static string AuthorEmail { get { return "kosorin@outlook.com"; } }

        public static string EmailSubject { get { return string.Format(AppResources.AboutEmailSubject, ApplicationName, Version); } }

        public string ApplicationNameString { get { return ApplicationName.TrimStart(ApplicationName[0]); } }

        public string AuthorString { get { return string.Format("by {0}", AuthorName); } }

        public string VersionString { get { return string.Format(AppResources.AboutVersion, Version); } }

        private void Rate_Click(object sender, RoutedEventArgs e)
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
            marketplaceReviewTask.Show();
        }

        private void Contact_Click(object sender, RoutedEventArgs e)
        {
            EmailComposeTask task = new EmailComposeTask
            {
                To = AuthorEmail,
                Subject = EmailSubject
            };
            task.Show();
        }
    }
}