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

        public const string ApplicationName = "Simple Tasks";

        public const string AuthorName = "David Kosorin";

        public const string AuthorEmail = "kosorin@outlook.com";

        public const string EmailSubject = "Feedback for Simple Task";

        public string ApplicationNameString { get { return ApplicationName.TrimStart(ApplicationName[0]); } }

        public string AuthorString { get { return string.Format(AppResources.AboutByAuthor, AuthorName); } }

        public string VersionString { get { return string.Format(AppResources.AboutVersion, GetVersion()); } }

        private string GetVersion()
        {
            AssemblyName nameHelper = new AssemblyName(Assembly.GetExecutingAssembly().FullName);
            return nameHelper.Version.ToString();
        }

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