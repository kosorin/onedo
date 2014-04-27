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
using System.Reflection;

namespace SimpleTasks.Views
{
    public partial class AboutPage : PhoneApplicationPage
    {
        public AboutPage()
        {
            InitializeComponent();

            DataContext = this;
        }

        public static string Version
        {
            get
            {
                return App.Version.ToString();
            }
        }

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