using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SimpleTasks.Core.Helpers;
using SimpleTasks.Core.Models;
using SimpleTasks.Resources;
using SimpleTasks.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
        public static Version Version
        {
            get
            {
                AssemblyName nameHelper = new AssemblyName(Assembly.GetExecutingAssembly().FullName);
                return nameHelper.Version;
            }
        }

        public static bool IsFirstStart = false;

        public static string ForceDebugCulture = "cs-CZ";

        public static readonly string SettingsFileName = "Settings.json";
        public static Settings Settings { get; private set; }

        public static readonly string TasksFileName = "Tasks.json";
        public static TasksViewModel Tasks { get; private set; }

        static App()
        {
            Debug.WriteLine("===== Application Constructor =====");

            Settings = new Settings();
            Tasks = new TasksViewModel();
        }

        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            Debug.WriteLine("===== Application Launching =====");
            Debug.WriteLine("> OS VERSION {0}", Environment.OSVersion.Version);
            Debug.WriteLine("> APP VERSION {0}", Version);

            Settings = Settings.LoadFromFile(SettingsFileName);

            if (Settings.Version == null || Settings.Version != Version.ToString())
            {
                Debug.WriteLine("==== INSTALACE/AKTUALIZACE ====");

                #region Načtení dat se starým formátem
                Tasks.Load();
                foreach (TaskModel task in Tasks.Tasks)
                {
                    if (task.ReminderDateObsoleteGet != null && task.Reminder == null)
                    {
                        task.DueDate = task.ReminderDateObsoleteGet;
                        task.Reminder = TimeSpan.Zero;
                    }

                    if (task.CompletedDateObsoleteGet != null && task.Completed == null)
                    {
                        task.Completed = task.CompletedDateObsoleteGet;
                    }
                }
                Tasks.Save();
                #endregion

                Settings.Version = Version.ToString();
                Debug.WriteLine("==== ===== Installed ===== ====");
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
                Settings = Settings.LoadFromFile(SettingsFileName);
                Tasks.Load();

                RootFrame.UriMapper = new MyUriMapper();
            }
            Debug.WriteLine("===== ===== ACTIVATED ===== =====");
        }

        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            Debug.WriteLine("===== Application Deactivated =====");
            Settings.SaveToFile(SettingsFileName, Settings);
            Tasks.Save();
            Debug.WriteLine("===== ===== DEACTIVATED ===== =====");
        }

        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            Debug.WriteLine("===== Application Closing =====");
            Settings.SaveToFile(SettingsFileName, Settings);
            Tasks.Save();
            Debug.WriteLine("===== ===== CLOSED ===== =====");
        }

        public static void UpdateAllLiveTiles()
        {
            LiveTile.UpdateOrReset(Settings.Tiles.Enable, Tasks.Tasks);
            foreach (TaskModel task in Tasks.Tasks)
            {
                if (task.ModifiedSinceStart)
                {
                    task.ModifiedSinceStart = false;
                    LiveTile.Update(task);
                }
            }
        }

        #region Phone application initialization

        public static PhoneApplicationFrame RootFrame { get; private set; }

        public App()
        {
            // Global handler for uncaught exceptions.
            UnhandledException += Application_UnhandledException;

            // Standard XAML initialization
            InitializeComponent();

            InitializeTheme();

            // Phone-specific initialization
            InitializePhoneApplication();

            // Language display initialization
            InitializeLanguage();

            // Show graphics profiling information while debugging.
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
    }
}