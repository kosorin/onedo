using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Newtonsoft.Json.Linq;
using SimpleTasks.Core.Helpers;
using SimpleTasks.Core.Models;
using SimpleTasks.Models;
using SimpleTasks.Resources;
using SimpleTasks.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Navigation;

namespace SimpleTasks
{
    public partial class App : Application
    {
        public static string ForceDebugCulture = "en-US";

        #region Properties
        public static Version Version
        {
            get
            {
                AssemblyName nameHelper = new AssemblyName(Assembly.GetExecutingAssembly().FullName);
                return nameHelper.Version;
            }
        }

        public static string VersionString
        {
            get { return Version.ToString(3); }
        }

        public static Tuple<string, string, MessageBoxButton> MessageAfterStart = null;

        public static readonly string SettingsFileName = "Settings.json";

        public static readonly string TasksFileName = "Tasks.json";
        public static TasksViewModel Tasks { get; private set; }
        #endregion

        static App()
        {
            Debug.WriteLine("===== Application Constructor =====");

            Tasks = new TasksViewModel();
        }

        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            Debug.WriteLine("===== Application Launching =====");
            Debug.WriteLine("> OS VERSION {0}", Environment.OSVersion.Version);
            Debug.WriteLine("> APP VERSION {0}", Version);

            Settings.Current = Settings.LoadFromFile(SettingsFileName);

            if (Settings.Current.Version == null)
            {
                Debug.WriteLine("==== INSTALACE ====");
                Settings.Current.Version = VersionString;
                Debug.WriteLine("==== ===== Installed ===== ====");
            }
            else if (Settings.Current.Version != VersionString)
            {
                Debug.WriteLine("==== AKTUALIZACE ====");
                Settings.Current.Version = VersionString;

                SimpleTasks.Models.ChangelogCategory changelog = App.LoadWhatsNew();
                if (changelog != null)
                {
                    string text = string.Format("{0} ({1})\n\n", string.Format(AppResources.AboutVersion, changelog.Version), changelog.Date.ToShortDateString());
                    foreach (SimpleTasks.Models.ChangelogItem item in changelog)
                    {
                        text += "  • " + item.Text + System.Environment.NewLine;
                    }
                    MessageAfterStart = new Tuple<string, string, MessageBoxButton>(text, AppResources.WhatsNew, MessageBoxButton.OK);
                }
                Debug.WriteLine("==== ===== Actualized ===== ====");
            }

            Tasks.Load();

            RootFrame.UriMapper = new MyUriMapper();
            Debug.WriteLine("===== ===== LAUNCHED ===== =====");
        }

        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            Debug.WriteLine("===== Application Activated =====");
            if (!e.IsApplicationInstancePreserved)
            {
                Settings.Current = Settings.LoadFromFile(SettingsFileName);
                Tasks.Load();

                RootFrame.UriMapper = new MyUriMapper();
            }
            Debug.WriteLine("===== ===== ACTIVATED ===== =====");
        }

        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            Debug.WriteLine("===== Application Deactivated =====");
            Settings.SaveToFile(SettingsFileName, Settings.Current);
            Tasks.Save();
            Debug.WriteLine("===== ===== DEACTIVATED ===== =====");
        }

        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            Debug.WriteLine("===== Application Closing =====");
            Settings.SaveToFile(SettingsFileName, Settings.Current);
            Tasks.Save();
            Debug.WriteLine("===== ===== CLOSED ===== =====");
        }

        public static ChangelogCategory LoadWhatsNew()
        {
            foreach (var version in JObject.Parse(AppResources.ChangelogFile))
            {
                JObject categoryData = (JObject)version.Value;

                ChangelogCategory category = new ChangelogCategory(version.Key, Convert.ToDateTime(categoryData["date"].ToString()));
                foreach (JToken item in (JArray)categoryData["items"])
                {
                    category.AddItem(item.ToString());
                }
                return category;
            }
            return null;
        }

        public static ChangelogList LoadChangelog()
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

            // První záznam je pro zobrazení zprávy "What's new" po aktualizaci/instalaci.
            changelog.RemoveAt(0);

            return changelog;
        }

        public static void FeedbackEmail()
        {
            EmailComposeTask task = new EmailComposeTask
            {
                To = AppInfo.Email,
                Subject = string.Format(AppResources.AboutEmailSubject, AppInfo.Name, App.VersionString)
            };
            task.Show();
        }

        #region Phone application initialization

        public static PhoneApplicationFrame RootFrame { get; private set; }

        public App()
        {
            UnhandledException += Application_UnhandledException;

            InitializeComponent();
            InitializePhoneApplication();
            InitializeLanguage();
            InitializeTheme();

            if (Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                //Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode,
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }
        }

        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                Debugger.Break();
            }
        }

        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new TransitionFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Handle reset requests for clearing the backstack
            RootFrame.Navigated += CheckForResetNavigation;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        private void CheckForResetNavigation(object sender, NavigationEventArgs e)
        {
            // If the app has received a 'reset' navigation, then we need to check
            // on the next navigation to see if the page stack should be reset
            if (e.NavigationMode == NavigationMode.Reset)
                RootFrame.Navigated += ClearBackStackAfterReset;
        }

        private void ClearBackStackAfterReset(object sender, NavigationEventArgs e)
        {
            // Unregister the event so it doesn't get called again
            RootFrame.Navigated -= ClearBackStackAfterReset;

            // Only clear the stack for 'new' (forward) and 'refresh' navigations
            if (e.NavigationMode != NavigationMode.New && e.NavigationMode != NavigationMode.Refresh)
                return;

            // For UI consistency, clear the entire page stack
            while (RootFrame.RemoveBackEntry() != null)
            {
                ; // do nothing
            }
        }

        #endregion

        private void InitializeLanguage()
        {
            try
            {
#if DEBUG
                // Force CurrentUICulture to locale defined by appForceCulture.
                // An empty string allows the user's Phone Language setting to
                // determine the locale.

                if (String.IsNullOrWhiteSpace(ForceDebugCulture) == false)
                {
                    Thread.CurrentThread.CurrentCulture =
                        new CultureInfo(ForceDebugCulture);
                    Thread.CurrentThread.CurrentUICulture =
                        new CultureInfo(ForceDebugCulture);
                }
#endif

                // Set the font to match the display language defined by the
                // ResourceLanguage resource string for each supported language.
                //
                // Fall back to the font of the neutral language if the Display
                // language of the phone is not supported.
                //
                // If a compiler error is hit then ResourceLanguage is missing from
                // the resource file.
                RootFrame.Language = XmlLanguage.GetLanguage(AppResources.ResourceLanguage);

                // Set the FlowDirection of all elements under the root frame based
                // on the ResourceFlowDirection resource string for each
                // supported language.
                //
                // If a compiler error is hit then ResourceFlowDirection is missing from
                // the resource file.
                FlowDirection flow = (FlowDirection)Enum.Parse(typeof(FlowDirection), AppResources.ResourceFlowDirection);
                RootFrame.FlowDirection = flow;
            }
            catch
            {
                // If an exception is caught here it is most likely due to either
                // ResourceLangauge not being correctly set to a supported language
                // code or ResourceFlowDirection is set to a value other than LeftToRight
                // or RightToLeft.

                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }

                throw;
            }
        }

        public void InitializeTheme()
        {

            //Debug.WriteLine("> Theme start: {0}", themeType);
            //Stopwatch sw = Stopwatch.StartNew();
            //if (themeType == Theme.Light)
            //{
            //    Debug.WriteLine(": to light theme");
            //    ThemeManager.ToLightTheme();
            //}
            //else if (themeType == Theme.Dark)
            //{
            //    Debug.WriteLine(": to dark theme");
            //    ThemeManager.ToDarkTheme();
            //}
            //sw.Stop();
            //Debug.WriteLine("Theme stop {0}", sw.ElapsedMilliseconds);

            string source = (Visibility)Resources["PhoneDarkThemeVisibility"] == Visibility.Visible ?
                "Dark" :
                "Light";
            string currentSource = (Visibility)Resources["DarkThemeVisibility"] == Visibility.Visible ?
                "Dark" :
                "Light";

            if (source == currentSource)
            {
                // Téma už je nastavené
                return;
            }
            else
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                // ResourceDictionary
                Uri sourceUri = new Uri(string.Format("/SimpleTasks;component/Themes/{0}.xaml", source), UriKind.Relative);
                ResourceDictionary app = Resources.MergedDictionaries[0];
                ResourceDictionary theme = new ResourceDictionary
                {
                    Source = sourceUri
                };

                // Barvy
                foreach (var ck in theme.Where(x => x.Value is Color))
                {
                    app.Remove(ck.Key);
                    app.Add(ck.Key, (Color)ck.Value);
                }

                // Brushe
                foreach (var ck in theme.Where(x => x.Value is SolidColorBrush))
                {
                    SolidColorBrush brush = (SolidColorBrush)ck.Value;
                    SolidColorBrush appBrush = (SolidColorBrush)app[ck.Key];

                    appBrush.Color = brush.Color;
                    appBrush.Opacity = brush.Opacity;
                }

                sw.Stop();
                Debug.WriteLine("## CHANGE THEME RESOURCE: ELAPSED TOTAL = {0}", sw.Elapsed);
            }
        }
        #endregion

        #region Theme
        private const string _themeSettingsKey = "Theme";

        public Theme GetTheme()
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            Theme theme;
            if (settings.Contains(_themeSettingsKey))
            {
                theme = (Theme)settings[_themeSettingsKey];
            }
            else
            {
                theme = Theme.System;
            }
            return theme;
        }

        public void SetTheme(Theme theme)
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains(_themeSettingsKey))
            {
                if (!settings[_themeSettingsKey].Equals(theme))
                {
                    settings[_themeSettingsKey] = theme;
                }
            }
            else
            {
                settings.Add(_themeSettingsKey, theme);
            }
            settings.Save();
        }
        #endregion // end of Theme
    }
}