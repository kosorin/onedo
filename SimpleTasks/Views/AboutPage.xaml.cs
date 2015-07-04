using System;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using SimpleTasks.Controls;
using SimpleTasks.Core.Helpers;
using SimpleTasks.Helpers;
using SimpleTasks.Resources;

namespace SimpleTasks.Views
{
    public partial class AboutPage : BasePage
    {
        public AboutPage()
        {
            InitializeComponent();

            DataContext = this;

            ApplicationBar = ThemeHelper.CreateApplicationBar();

            ApplicationBarIconButton rateButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.star.png", UriKind.Relative));
            rateButton.Text = AppResources.AboutRateReview;
            rateButton.Click += Rate_Click;
            ApplicationBar.Buttons.Add(rateButton);

            ApplicationBarIconButton contactButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.email.hardedge.png", UriKind.Relative));
            contactButton.Text = AppResources.AboutContactUs;
            contactButton.Click += Contact_Click;
            ApplicationBar.Buttons.Add(contactButton);

            ApplicationBarMenuItem storeItem = new ApplicationBarMenuItem("store");
            storeItem.Click += StoreItem_Click;
            ApplicationBar.MenuItems.Add(storeItem);

            ApplicationBarMenuItem baItem = new ApplicationBarMenuItem("background agent");
            baItem.Click += (s, e) => { Navigate(typeof(AboutBAPage)); };
            ApplicationBar.MenuItems.Add(baItem);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            SystemTray.ForegroundColor = (Color)App.Current.Resources["SlightColor"];
        }

        #region Propertie
        public string VersionString { get { return App.VersionString; } }

        public string Author { get { return AppInfo.Author; } }

        public string AppName { get { return AppInfo.Name; } }
        #endregion

        #region Rate
        private void Rate_Click(object sender, EventArgs e)
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
            marketplaceReviewTask.Show();
        }
        #endregion

        #region Email
        private void Contact_Click(object sender, EventArgs e)
        {
            App.FeedbackEmail();
        }
        #endregion

        #region Store
        private void StoreItem_Click(object sender, EventArgs e)
        {
            MarketplaceDetailTask marketplaceDetailTask = new MarketplaceDetailTask();
            marketplaceDetailTask.ContentIdentifier = "e21e7ce7-e536-4788-86c1-b6f72625ce2b";
            marketplaceDetailTask.ContentType = MarketplaceContentType.Applications;
            marketplaceDetailTask.Show();
        }
        #endregion

        #region Changelog
        private void Changelog_Tap(object sender, GestureEventArgs e)
        {
            Navigate(typeof(ChangelogPage));
        }
        #endregion // end of Changelog
    }
}