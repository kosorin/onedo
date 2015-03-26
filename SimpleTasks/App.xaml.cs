using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Tasks;
using Newtonsoft.Json.Linq;
using SimpleTasks.Core.Helpers;
using SimpleTasks.Core.Models;
using SimpleTasks.Helpers;
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
        public static readonly string BackgroundAgentName = "PeriodicBackgroundAgent";

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

        public static bool ShowChangelog = false;

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

            Settings.Current = Settings.LoadFromFile(AppInfo.SettingsFileName);

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
                ShowChangelog = true;
                Debug.WriteLine("==== ===== Actualized ===== ====");
            }

            Tasks.Load();

            if (Settings.Current.EnableTile)
            {
                try
                {
                    bool add = true;
                    PeriodicTask task = ScheduledActionService.Find(BackgroundAgentName) as PeriodicTask;
                    if (task != null)
                    {
                        ScheduledActionService.Remove(BackgroundAgentName);
                        //if (task.ExpirationTime - DateTime.Now < TimeSpan.FromDays(7))
                        //{
                        //    ScheduledActionService.Remove(BackgroundAgentName);
                        //}
                        //else
                        //{
                        //    add = false;
                        //}
                    }

                    if (add)
                    {
                        ScheduledActionService.Add(new PeriodicTask(BackgroundAgentName)
                        {
                            ExpirationTime = DateTime.Today.AddDays(14),
                            Description = AppResources.PeriodicTaskDescription
                        });

#if DEBUG
                        if (Debugger.IsAttached)
                        {
                            ScheduledActionService.LaunchForTest(BackgroundAgentName, TimeSpan.FromSeconds(65));
                            Debug.WriteLine("> LAUNCH FOR TEST");
                        }
#endif
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("ERR> PeriodicTask: {0}", ex.Message);
                }
            }

            RootFrame.UriMapper = new MyUriMapper();
            Debug.WriteLine("===== ===== LAUNCHED ===== =====");
        }

        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            Debug.WriteLine("===== Application Activated =====");
            if (!e.IsApplicationInstancePreserved)
            {
                Settings.Current = Settings.LoadFromFile(AppInfo.SettingsFileName);
                Tasks.Load();

                RootFrame.UriMapper = new MyUriMapper();
            }
            Debug.WriteLine("===== ===== ACTIVATED ===== =====");
        }

        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            Debug.WriteLine("===== Application Deactivated =====");
            Settings.SaveToFile(AppInfo.SettingsFileName, Settings.Current);
            Tasks.Save();
            Debug.WriteLine("===== ===== DEACTIVATED ===== =====");
        }

        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            Debug.WriteLine("===== Application Closing =====");
            Settings.SaveToFile(AppInfo.SettingsFileName, Settings.Current);
            Tasks.Save();
            Debug.WriteLine("===== ===== CLOSED ===== =====");
        }

        public static ChangelogList LoadChangelog()
        {
            ChangelogList changelog = new ChangelogList();

            try
            {
                string path;
                if (AppResources.ResourceLanguage == "cs-CZ" || AppResources.ResourceLanguage == "sk-SK")
                {
                    path = @"Resources\Changelog\Changelog.cs.json";
                }
                else
                {
                    path = @"Resources\Changelog\Changelog.json";
                }
                string jsonText = ResourcesHelper.ReadTextFile(path);
                if (jsonText != null)
                {
                    foreach (var version in JObject.Parse(jsonText))
                    {
                        JObject categoryData = (JObject)version.Value;

                        ChangelogCategory category = new ChangelogCategory(version.Key,
                            Convert.ToDateTime(categoryData["date"].ToString()));
                        foreach (JToken item in (JArray)categoryData["items"])
                        {
                            category.AddItem(item.ToString());
                        }
                        changelog.AddCategory(category);
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                if (Debugger.IsAttached)
                    Debug.WriteLine(ex.StackTrace);
#endif
            }

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

        public static Style IconStyle(string name)
        {
            return App.Current.Resources[name + "IconStyle"] as Style;
        }

        #region Phone application initialization

        public static PhoneApplicationFrame RootFrame { get; private set; }

        public App()
        {
            UnhandledException += Application_UnhandledException;

            InitializeComponent();
            InitializePhoneApplication();
            InitializeLanguage();
            ThemeHelper.InitializeTheme();

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
                if (!String.IsNullOrWhiteSpace(ForceDebugCulture) && Microsoft.Devices.Environment.DeviceType == Microsoft.Devices.DeviceType.Emulator)
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
        #endregion
    }
}