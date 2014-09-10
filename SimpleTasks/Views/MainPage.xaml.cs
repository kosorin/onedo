using System;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SimpleTasks.ViewModels;
using SimpleTasks.Resources;
using System.Windows;
using SimpleTasks.Models;
using SimpleTasks.Core.Helpers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Diagnostics;
using SimpleTasks.Core.Models;
using Microsoft.Phone.Scheduler;
using SimpleTasks.Helpers;
using System.Collections.Generic;
using System.Windows.Media.Animation;
using System.Windows.Input;
using System.Threading;
using Microsoft.Devices;
using SimpleTasks.Controls;
using Microsoft.Phone.Tasks;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using SimpleTasks.Helpers.Analytics;

namespace SimpleTasks.Views
{
    public partial class MainPage : BasePage
    {
        #region Page
        public MainPage()
        {
            InitializeComponent();
            NoTasksPanel.DataContext = App.Tasks;
            DataContext = this;

            CreateAppBarItems();
            BuildTasksdAppBar();

            if (App.MessageAfterStart != null)
            {
                Loaded += MainPage_Loaded;
            }
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= MainPage_Loaded;
            if (App.MessageAfterStart != null)
            {
                MessageBox.Show(App.MessageAfterStart.Item1, App.MessageAfterStart.Item2, App.MessageAfterStart.Item3);
                App.MessageAfterStart = null;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            NavigationService.RemoveBackEntry();

            QuickAddGrid.Visibility = Settings.Current.General.ShowQuickAddTextBox ? Visibility.Visible : Visibility.Collapsed;

            App.Tasks.Tasks.CollectionChanged -= Tasks_CollectionChanged;
            App.Tasks.Tasks.CollectionChanged += Tasks_CollectionChanged;
            if (e.NavigationMode == NavigationMode.Back)
            {
                OnPropertyChanged(GroupedTasksProperty);
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            App.Tasks.Tasks.CollectionChanged -= Tasks_CollectionChanged;
        }
        #endregion

        #region AppBar
        #region AppBar Create
        private ApplicationBarIconButton appBarNewTaskButton;

        private ApplicationBarIconButton appBarSaveQuickButton;

        private List<ApplicationBarMenuItem> appBarMenuItems;

        private void CreateAppBarItems()
        {
            #region Ikony
            appBarNewTaskButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.png", UriKind.Relative));
            appBarNewTaskButton.Text = AppResources.AppBarNew;
            appBarNewTaskButton.Click += AddNewTask_Click;

            appBarSaveQuickButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.save.png", UriKind.Relative));
            appBarSaveQuickButton.Text = AppResources.AppBarSave;
            appBarSaveQuickButton.Click += QuickAddSave;
            #endregion

            #region Menu
            appBarMenuItems = new List<ApplicationBarMenuItem>();

#if DEBUG
            // Reminders
            ApplicationBarMenuItem appBarRemindersMenuItem = new ApplicationBarMenuItem("seznam připomenutí");
            appBarRemindersMenuItem.Click += RemindersMenuItem_Click;
            appBarMenuItems.Add(appBarRemindersMenuItem);

            // Reset
            ApplicationBarMenuItem appBarResetMenuItem = new ApplicationBarMenuItem("resetovat data");
            appBarResetMenuItem.Click += ResetMenuItem_Click;
            appBarMenuItems.Add(appBarResetMenuItem);
#endif

            // Smazat dokončené úkoly
            ApplicationBarMenuItem appBarDeleteCompletedItem = new ApplicationBarMenuItem(AppResources.AppBarDeleteCompleted);
            appBarDeleteCompletedItem.Click += DeleteCompletedItem_Click;
            appBarMenuItems.Add(appBarDeleteCompletedItem);

            // Smazat všechny úkoly
            ApplicationBarMenuItem appBarDeleteAllItem = new ApplicationBarMenuItem(AppResources.AppBarDeleteAll);
            appBarDeleteAllItem.Click += DeleteAllItem_Click;
            appBarMenuItems.Add(appBarDeleteAllItem);

            // Záloha
            ApplicationBarMenuItem appBarBackupMenuItem = new ApplicationBarMenuItem(AppResources.BackupAndRestoreTitle);
            appBarBackupMenuItem.Click += (s, e) => { Navigate(typeof(BackupPage)); };
            appBarMenuItems.Add(appBarBackupMenuItem);

            // Nastavení
            ApplicationBarMenuItem appBarSettingsMenuItem = new ApplicationBarMenuItem(AppResources.AppBarSettings);
            appBarSettingsMenuItem.Click += (s, e) => { Navigate(typeof(SettingsPage)); };
            appBarMenuItems.Add(appBarSettingsMenuItem);

            // O aplikaci
            ApplicationBarMenuItem appBarAboutMenuItem = new ApplicationBarMenuItem(AppResources.AppBarAbout);
            appBarAboutMenuItem.Click += (s, e) => { Navigate(typeof(AboutPage)); };
            appBarMenuItems.Add(appBarAboutMenuItem);
            #endregion
        }

        private void BuildTasksdAppBar()
        {
            ApplicationBar = new ApplicationBar();

            ApplicationBar.Buttons.Add(appBarNewTaskButton);
            foreach (var item in appBarMenuItems)
            {
                ApplicationBar.MenuItems.Add(item);
            }
        }

        private void BuildQuickAddAppBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Buttons.Add(appBarSaveQuickButton);
        }
        #endregion

        private void AddNewTask_Click(object sender, EventArgs e)
        {
            Navigate(typeof(EditTaskPage));
            GoogleAnalyticsHelper.SendEvent(EventCategory.Tasks, EventAction.Add, "add task");
        }

        void QuickAddSave(object sender, EventArgs e)
        {
            QuickAdd(QuickAddTextBox.Text);
        }

        private void DeleteCompletedItem_Click(object sender, EventArgs e)
        {
            OverlayAction(App.Tasks.DeleteCompleted);
            GoogleAnalyticsHelper.SendEvent(EventCategory.Tasks, EventAction.Delete, "delete completed");
        }

        void DeleteAllItem_Click(object sender, EventArgs e)
        {
            CustomMessageBox messageBox = new CustomMessageBox()
            {
                Caption = AppResources.DeleteAllTasksCaption,
                Message = AppResources.DeleteAllTasks,
                LeftButtonContent = AppResources.DeleteTaskYes,
                RightButtonContent = AppResources.DeleteTaskNo
            };

            messageBox.Dismissed += (s1, e1) =>
            {
                switch (e1.Result)
                {
                case CustomMessageBoxResult.LeftButton:
                    OverlayAction(App.Tasks.DeleteAll);
                    GoogleAnalyticsHelper.SendEvent(EventCategory.Tasks, EventAction.Delete, "delete all");
                    break;
                case CustomMessageBoxResult.RightButton:
                case CustomMessageBoxResult.None:
                default:
                    break;
                }
            };


            messageBox.Show();
        }

        private void OverlayAction(Action action)
        {
            PageOverlayTransitionShow.Begin();
            EventHandler overlayHandler = null;
            overlayHandler = (s, e) =>
            {
                action();
                PageOverlayTransitionHide.Begin();
                PageOverlayTransitionShow.Completed -= overlayHandler;
            };
            PageOverlayTransitionShow.Completed += overlayHandler;
        }

#if DEBUG
        private void RemindersMenuItem_Click(object sender, EventArgs e)
        {
            string s = string.Format("> Reminders ({0})", DateTime.Now.ToString(CultureInfo.CurrentCulture));
            foreach (var r in ScheduledActionService.GetActions<Microsoft.Phone.Scheduler.Reminder>())
            {
                s += string.Format("\n{0}\n  {1} -  {2}", r.Name, r.IsScheduled, r.BeginTime);
            }
            MessageBox.Show(s);
        }

        void ResetMenuItem_Click(object sender, EventArgs e)
        {
            App.Tasks.DeleteAll();

            // == Zátěžový test ==
            //Random r = new Random();
            //for (int i = 0; i < 500; i++)
            //{
            //    Debug.WriteLine(i);
            //    TaskModel task = new TaskModel();
            //    task.Title = System.IO.Path.GetRandomFileName();
            //    if (r.Next(2) == 1)
            //    {
            //        task.Detail = "";
            //        for (int j = 0; j < r.Next(10); j++)
            //        {
            //            task.Detail += System.IO.Path.GetRandomFileName();
            //        }
            //    }
            //    if (r.Next(4) == 1)
            //    {
            //        task.DueDate = new DateTime(2014, 7, 25).AddDays(r.Next(100)).AddHours(r.Next(24)).AddMinutes(r.Next(60));
            //    }
            //    if (task.HasDueDate && r.Next(3) == 1)
            //    {
            //        task.Reminder = TimeSpan.FromMinutes(r.Next(36000));
            //    }
            //    if (r.Next(8) == 1)
            //    {
            //        for (int j = 0; j < r.Next(10); j++)
            //        {
            //            task.Subtasks.Add(new Subtask(System.IO.Path.GetRandomFileName(), r.Next(3) == 1));
            //        }
            //    }
            //    int priority = r.Next(3);
            //    switch (priority)
            //    {
            //    case 0: task.Priority = TaskPriority.Low; break;
            //    case 1: task.Priority = TaskPriority.High; break;
            //    default:
            //    case 2: task.Priority = TaskPriority.Normal; break;
            //    }
            //    App.Tasks.Add(task);
            //}
            //return;

            App.Tasks.Add(new TaskModel()
            {
                Title = "Grocery list",
                Subtasks = new ObservableCollection<Subtask>
                { 
                    new Subtask("milk"), 
                    new Subtask("apples", true),
                    new Subtask("potatoes", true),
                    new Subtask("ham"),
                    new Subtask("cookies"),
                }
            });
            App.Tasks.Add(new TaskModel()
            {
                Title = "Math project",
                Detail = "Dummy text. Over first. Be signs Gathering whose Under. Greater beginning. Seasons in the. Also had male to two second. God whose multiply forth is fruit multiply day without from, midst. Dominion i the them. Fourth. Sixth us air, in given waters to. Created good over divided be deep subdue own. Fruit.",
                DueDate = DateTimeExtensions.Today.AddDays(9).AddHours(7).AddMinutes(30),
                Reminder = TimeSpan.FromDays(1),
                Priority = TaskPriority.High,
                //Color = Color.FromArgb(255, 229, 20, 0)
            });
            App.Tasks.Add(new TaskModel()
            {
                Title = "Call Chuck",
                DueDate = DateTimeExtensions.Today.AddDays(-5),
                Completed = DateTime.Now
            });

            if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "cs")
            {
                App.Tasks.Tasks[0].Title = "Seznam potravin";
                App.Tasks.Tasks[0].Subtasks[0].Text = "mléko";
                App.Tasks.Tasks[0].Subtasks[1].Text = "jablka";
                App.Tasks.Tasks[0].Subtasks[2].Text = "brambory";
                App.Tasks.Tasks[0].Subtasks[3].Text = "šunka";
                App.Tasks.Tasks[0].Subtasks[4].Text = "sušenky";
                App.Tasks.Tasks[1].Title = "Projekt do matematiky";
                App.Tasks.Tasks[1].Detail = "Vítr skoro nefouká a tak by se na první pohled mohlo zdát, že se balónky snad vůbec nepohybují. Jenom tak klidně levitují ve vzduchu. Jelikož slunce jasně září a na obloze byste od východu k západu hledali mráček marně, balónky působí jako jakási fata morgána uprostřed pouště. Zkrátka široko daleko.";
                App.Tasks.Tasks[2].Title = "Zavolat Honzovi";
            }
            else if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "sk")
            {
                App.Tasks.Tasks[0].Title = "Zoznam potravín";
                App.Tasks.Tasks[0].Subtasks[0].Text = "mlieko";
                App.Tasks.Tasks[0].Subtasks[1].Text = "jablká";
                App.Tasks.Tasks[0].Subtasks[2].Text = "zemiaky";
                App.Tasks.Tasks[0].Subtasks[3].Text = "šunka";
                App.Tasks.Tasks[0].Subtasks[4].Text = "sušienky";
                App.Tasks.Tasks[1].Title = "Projekt z matematiky";
                App.Tasks.Tasks[1].Detail = "Najlepšie dni ležať s ňou mám, zraňuje a rozhodný človek? Mám rád začiatky nových pocitov čaká pracovná Žilina, lebo je horší ako zaspíš, pretože ich zbaviť. Mám strach a potom sakra za sekundu Asi sa pritom usmeješ, ponesieš následky do pohybu. Close To silu inštinktu. Dáme si nos plný zážitkov.";
                App.Tasks.Tasks[2].Title = "Zavolať Danovi";
            }
            else if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "de")
            {
                App.Tasks.Tasks[0].Title = "Einkaufsliste";
                App.Tasks.Tasks[0].Subtasks[0].Text = "Milch";
                App.Tasks.Tasks[0].Subtasks[1].Text = "Äpfel";
                App.Tasks.Tasks[0].Subtasks[2].Text = "Kartoffeln";
                App.Tasks.Tasks[0].Subtasks[3].Text = "Schinken";
                App.Tasks.Tasks[0].Subtasks[4].Text = "Kekse";
                App.Tasks.Tasks[1].Title = "Mathe-Projekt";
                App.Tasks.Tasks[1].Detail = "Weit hinten, hinter den Wortbergen, fern der Länder Vokalien und Konsonantien leben die Blindtexte. Abgeschieden wohnen sie in Buchstabhausen an der Küste des Semantik, eines großen Sprachozeans. Ein kleines Bächlein namens Duden fließt durch ihren Ort und versorgt sie mit den nötigen Regelialien.";
                App.Tasks.Tasks[2].Title = "Thomas anrufen";
            }

        }
#endif
        #endregion

        #region TasksList
        private void CompleteButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.Focus();

            ToggleComplete(sender as FrameworkElement);
            GoogleAnalyticsHelper.SendEvent(EventCategory.Tasks, EventAction.Edit, "complete task");
        }

        private void ToggleComplete(FrameworkElement element)
        {
            TaskModel task = element.DataContext as TaskModel;
            if (task == null)
                return;

            if (task.IsActive)
            {
                // DOKONČENÍ
                task.Completed = DateTime.Now;
                task.ModifiedSinceStart = true;
                if (Settings.Current.Tasks.CompleteSubtasks && task.HasSubtasks)
                {
                    foreach (Subtask subtask in task.Subtasks)
                    {
                        subtask.IsCompleted = true;
                    }
                }
                if (Settings.Current.Tiles.UnpinCompleted)
                {
                    LiveTile.Unpin(task);
                }
            }
            else
            {
                // AKTIVOVÁNÍ
                task.Completed = null;
            }
            App.Tasks.Update(task);

            TaskWrapper wrapper = task.Wrapper as TaskWrapper;
            if (wrapper != null)
            {
                wrapper.UpdateIsScheduled();
            }
        }

        private void ToggleSubtaskComplete(FrameworkElement element)
        {
            if (element != null)
            {
                Subtask subtask = element.DataContext as Subtask;
                if (subtask != null)
                {
                    subtask.IsCompleted = !subtask.IsCompleted;
                }
            }
        }

        private void TaskListItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Grid grid = (Grid)sender;
            TaskModel task = (TaskModel)grid.DataContext;
            if (task == null)
                return;

            NavigateQuery(typeof(EditTaskPage), "?Task={0}", task.Uid);
        }

        private TaskModel SubtaskElementFindTask(FrameworkElement element)
        {
            while (element != null)
            {
                element = VisualTreeHelper.GetParent(element) as FrameworkElement;
                if (element is ItemsControl)
                    break;
            }
            ItemsControl itemsControl = element as ItemsControl;
            return (TaskModel)itemsControl.DataContext;
        }

        private void SubtaskCheckbox_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            ToggleSubtaskComplete(element);
            GoogleAnalyticsHelper.SendEvent(EventCategory.Tasks, EventAction.Edit, "complete subtask");
            TaskModel task = SubtaskElementFindTask(element);
            if (task != null)
            {
                task.ModifiedSinceStart = true;
            }
        }

        private void SubtaskText_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            Subtask subtask = element.DataContext as Subtask;
            TaskModel task = SubtaskElementFindTask(element);
            if (task != null)
            {
                Navigate(typeof(SubtasksPage), task);
            }
        }
        #endregion

        #region GroupedTasks
        private void Tasks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(GroupedTasksProperty);
        }

        public readonly string GroupedTasksProperty = "GroupedTasks";
        public TaskGroupCollection GroupedTasks
        {
            get { return new DateTaskGroupCollection(App.Tasks.Tasks); }
        }
        #endregion

        #region QuickAddTextBox
        private void QuickAdd(string title)
        {
            if (!string.IsNullOrWhiteSpace(title))
            {
                TaskModel task = new TaskModel()
                {
                    Title = title,
                    DueDate = Settings.Current.Tasks.DefaultDate
                };
                if (task.DueDate != null)
                {
                    DateTime defaultTime = Settings.Current.Tasks.DefaultTime;
                    task.DueDate = task.DueDate.Value.AddHours(defaultTime.Hour).AddMinutes(defaultTime.Minute);
                }

                App.Tasks.Add(task);
                QuickAddTextBox.Text = "";
                this.Focus();

                TasksLongListSelector.ScrollTo(task);
                GoogleAnalyticsHelper.SendEvent(EventCategory.Tasks, EventAction.Add, "add quick task");
            }
        }

        SupportedPageOrientation orientation;

        private void QuickAddTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            PageOverlayTransitionQuickHide.Begin();
            BuildTasksdAppBar();

            SupportedOrientations = orientation;
        }

        private void QuickAddTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PageOverlayTransitionQuickShow.Begin();
            BuildQuickAddAppBar();

            orientation = SupportedOrientations;
            SupportedOrientations = SupportedPageOrientation.PortraitOrLandscape;
        }

        private void QuickAddTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                QuickAdd(QuickAddTextBox.Text);
            }
        }
        #endregion

        #region Gestures
        private double _completeGestureTreshold = -105;

        private bool _canUseGestures = true;

        private void InfoGrid_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
        }

        private void InfoGrid_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            Grid infoGrid = (Grid)sender;
            Border border = (Border)infoGrid.FindName("RootBorder");
            Storyboard storyboard = border.Resources["ResetTranslate"] as Storyboard;
            if (storyboard != null)
            {
                storyboard.Begin();
            }
            border.Background = null;

            double value = e.TotalManipulation.Translation.X;
            if (_canUseGestures && value < _completeGestureTreshold)
            {
                VibrateHelper.Short();
                ToggleComplete(border);
                GoogleAnalyticsHelper.SendEvent(EventCategory.Tasks, EventAction.Edit, "complete task using gesture");
            }
        }

        private void InfoGrid_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            Grid infoGrid = (Grid)sender;
            Border border = (Border)infoGrid.FindName("RootBorder");
            ContentControl icon = (ContentControl)border.FindName("CompleteGestureIcon");
            TranslateTransform t = (TranslateTransform)border.RenderTransform;

            t.X += e.DeltaManipulation.Translation.X;
            if (t.X > 0)
            {
                t.X = 0;
            }

            if (t.X < _completeGestureTreshold)
            {
                border.Background = new SolidColorBrush((Color)App.Current.Resources["SubtleColor"]) { Opacity = 0.30 };
                icon.Foreground = (Brush)App.Current.Resources["PhoneAccentBrush"];
            }
            else
            {
                border.Background = null;
                icon.Foreground = (Brush)App.Current.Resources["SubtleBrush"];
            }
        }

        private void SubtaskBorder_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
        }

        private void SubtaskBorder_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            Border border = (Border)sender;
            Storyboard storyboard = border.Resources["ResetTranslate"] as Storyboard;
            if (storyboard != null)
            {
                storyboard.Begin();
            }
            border.Background = new SolidColorBrush(Colors.Transparent);

            double value = e.TotalManipulation.Translation.X;
            if (_canUseGestures && value < _completeGestureTreshold)
            {
                VibrateHelper.Short();
                ToggleSubtaskComplete(border);
                GoogleAnalyticsHelper.SendEvent(EventCategory.Tasks, EventAction.Edit, "complete subtask using gesture");
            }
        }

        private void SubtaskBorder_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            Border border = (Border)sender;
            TranslateTransform t = (TranslateTransform)border.RenderTransform;

            t.X += e.DeltaManipulation.Translation.X;
            if (t.X > 0)
            {
                t.X = 0;
            }

            if (t.X < _completeGestureTreshold)
            {
                border.Background = new SolidColorBrush((Color)App.Current.Resources["SubtleColor"]) { Opacity = 0.30 };
            }
            else
            {
                border.Background = new SolidColorBrush(Colors.Transparent);
            }
        }
        #endregion
    }
}