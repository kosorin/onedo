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
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;

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

            Loaded += MainPage_Loaded;

            //if (false)
            //{
            //    _rlb = new ReorderListBox.ReorderListBox();
            //    BindingOperations.SetBinding(_rlb, ReorderListBox.ReorderListBox.ItemsSourceProperty, new Binding("Tasks") { Source = this });
            //    _rlb.ItemTemplate = (DataTemplate)Resources["TasksLongListSelectorItemTemplate"];
            //    _rlb.IsReorderEnabled = true;
            //    ContentPanel.Children.Insert(0, _rlb);
            //}
            //else
            //{
            //    _lls = new LongListSelector();
            //    BindingOperations.SetBinding(_lls, LongListSelector.ItemsSourceProperty, new Binding("GroupedTasks") { Source = this });
            //    _lls.Template = (ControlTemplate)Resources["TasksLongListSelectorTemplate"];
            //    _lls.GroupHeaderTemplate = (DataTemplate)Resources["TasksLongListSelectorGroupHeaderTemmplate"];
            //    _lls.GroupFooterTemplate = (DataTemplate)Resources["TasksLongListSelectorGroupFooterTemmplate"];
            //    _lls.ItemTemplate = (DataTemplate)Resources["TasksLongListSelectorItemTemplate"];
            //    _lls.LayoutMode = LongListSelectorLayoutMode.List;
            //    _lls.IsGroupingEnabled = true;
            //    _lls.HideEmptyGroups = true;
            //    ContentPanel.Children.Insert(0, _lls);
            //}

#if DEBUG
            TestButton.Click += RemindersMenuItem_Click;
#else
            TestButton.Visibility = System.Windows.Visibility.Collapsed;
#endif
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= MainPage_Loaded;

            if (App.ShowChangelog)
            {
                ShowWhatsNew();
                App.ShowChangelog = false;
            }
        }

        private static void ShowWhatsNew()
        {
            ChangelogCategory changelog = App.LoadChangelog().FirstOrDefault();
            if (changelog != null && changelog.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat(AppResources.VersionFormat, changelog.Version);
                sb.AppendFormat("\n{0:d}\n\n", changelog.Date);
                foreach (ChangelogItem item in changelog)
                {
                    sb.AppendFormat(" \u2022 {0}\n", item.Text);
                }
                MessageBox.Show(sb.ToString(), AppResources.WhatsNew, MessageBoxButton.OK);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            SystemTray.BackgroundColor = (Color)App.Current.Resources["AccentColor"];
            SystemTray.Opacity = 1;

            NavigationService.RemoveBackEntry();

            App.Tasks.Tasks.CollectionChanged -= Tasks_CollectionChanged;
            App.Tasks.Tasks.CollectionChanged += Tasks_CollectionChanged;

            UpdateGroupedTasks();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
        }
        #endregion

        #region AppBar

        #region AppBar Create
        private List<ApplicationBarIconButton> appBarButtons;

        private List<ApplicationBarMenuItem> appBarMenuItems;

        private void CreateAppBarItems()
        {
            #region Ikony
            appBarButtons = new List<ApplicationBarIconButton>();

            // Přidat úkol
            ApplicationBarIconButton appBarNewTaskButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.png", UriKind.Relative));
            appBarNewTaskButton.Text = AppResources.AppBarNew;
            appBarNewTaskButton.Click += (s, e) => { Navigate(typeof(EditTaskPage)); };
            appBarButtons.Add(appBarNewTaskButton);

            // Smazat dokončené úkoly
            ApplicationBarIconButton appBarDeleteCompletedButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.clean.png", UriKind.Relative));
            appBarDeleteCompletedButton.Text = AppResources.AppBarDeleteCompleted;
            appBarDeleteCompletedButton.Click += DeleteCompletedMenuItem_Click;
            appBarButtons.Add(appBarDeleteCompletedButton);
            #endregion

            #region Menu
            appBarMenuItems = new List<ApplicationBarMenuItem>();

#if DEBUG
            // Reset
            ApplicationBarMenuItem appBarResetMenuItem = new ApplicationBarMenuItem("resetovat data");
            appBarResetMenuItem.Click += ResetMenuItem_Click;
            appBarMenuItems.Add(appBarResetMenuItem);
#endif

            // Smazat všechny úkoly
            ApplicationBarMenuItem appBarDeleteAllItem = new ApplicationBarMenuItem(AppResources.AppBarDeleteAll);
            appBarDeleteAllItem.Click += DeleteAllMenuItem_Click;
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

#if DEBUG
            // Reminders
            ApplicationBarMenuItem appBarRemindersMenuItem = new ApplicationBarMenuItem("seznam připomenutí");
            appBarRemindersMenuItem.Click += RemindersMenuItem_Click;
            appBarMenuItems.Add(appBarRemindersMenuItem);
#endif
            #endregion
        }

        private void BuildTasksdAppBar()
        {
            ApplicationBar = ThemeHelper.CreateApplicationBar();

            foreach (var item in appBarButtons)
            {
                ApplicationBar.Buttons.Add(item);
            }
            foreach (var item in appBarMenuItems)
            {
                ApplicationBar.MenuItems.Add(item);
            }
        }
        #endregion

        private void DeleteCompletedMenuItem_Click(object sender, EventArgs e)
        {
            OverlayAction(App.Tasks.DeleteCompleted);
            Toast.Show(AppResources.ToastCompletedTasksDeleted, App.IconStyle("Delete"));
        }

        private void DeleteAllMenuItem_Click(object sender, EventArgs e)
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
                    Toast.Show(AppResources.ToastAllTasksDeleted, App.IconStyle("Delete"));
                    break;
                case CustomMessageBoxResult.RightButton:
                case CustomMessageBoxResult.None:
                default:
                    break;
                }
            };

            messageBox.Show();
        }

#if DEBUG
        private void RemindersMenuItem_Click(object sender, EventArgs e)
        {
            string s = string.Format("> Reminders ({0})", DateTime.Now.ToString(CultureInfo.CurrentCulture));
            foreach (var r in ScheduledActionService.GetActions<Microsoft.Phone.Scheduler.Reminder>())
            {
                string name = r.Name;
                if (name.Length > 12)
                {
                    name = string.Format("{0}...{1}", name.Substring(0, 3), name.Substring(name.Length - 3, 3));
                }
                string interval = "";
                switch (r.RecurrenceType)
                {
                case RecurrenceInterval.Daily:
                    interval = " [d]";
                    break;
                case RecurrenceInterval.Monthly:
                    interval = " [m]";
                    break;
                case RecurrenceInterval.Weekly:
                    interval = " [w]";
                    break;
                default:
                    break;
                }
                s += string.Format("\n{0}: {1} - {2:g}{3}", name, r.IsScheduled, r.BeginTime, interval);
            }
            MessageBox.Show(s);
        }

        private void ResetMenuItem_Click(object sender, EventArgs e)
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
                DueDate = DateTimeExtensions.Today.AddDays(9).AddHours(7).AddMinutes(30),
                Reminder = TimeSpan.FromDays(1),
                Priority = TaskPriority.High,
                Color = Color.FromArgb(255, 229, 20, 0)
            });
            App.Tasks.Add(new TaskModel()
            {
                Title = "Call Chuck",
                DueDate = DateTimeExtensions.Today.AddDays(-5),
                Completed = DateTime.Now
            });
        }
#endif
        #endregion

        #region Methods
        private void ToggleComplete(TaskModel task)
        {
            if (task == null)
                return;

            if (task.IsActive)
            {
                App.Tasks.Complete(task);
            }
            else
            {
                App.Tasks.Activate(task);
            }
        }

        private void ToggleComplete(TaskModel task, Subtask subtask)
        {
            App.Tasks.Complete(task, subtask);
        }

        private void SetDueDate(TaskModel task, DateTime? due, GestureAction action)
        {
            task.DueDate = due;
            App.Tasks.Update(task);
            UpdateGroupedTasks();
        }

        private void Postpone(TaskModel task, DateTime? due, GestureAction action)
        {
            task.DueDate = due;
            App.Tasks.Update(task);
            UpdateGroupedTasks();

            Toast.Show(string.Format(AppResources.ToastPostponedUntil, task.DueDate), GestureActionHelper.IconStyle(action));
        }
        #endregion // end of Methods

        #region Tasks
        private void Tasks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                ResetGroupedTasks();
            }
            else
            {
                if (e.OldItems != null)
                {
                    foreach (TaskModel task in e.OldItems)
                    {
                        GroupedTasks.RemoveTask(task);
                    }
                }
                if (e.NewItems != null)
                {
                    foreach (TaskModel task in e.NewItems)
                    {
                        GroupedTasks.AddSortedTask(task);
                    }
                }
            }
        }

        public readonly string GroupedTasksPropertyName = "GroupedTasks";
        private TaskGroupCollection _groupedTasks = null;
        public TaskGroupCollection GroupedTasks
        {
            get
            {
                if (_groupedTasks == null)
                {
                    _groupedTasks = new DateTaskGroupCollection(App.Tasks.Tasks);
                }
                return _groupedTasks;
            }
        }

        private void ResetGroupedTasks()
        {
            _groupedTasks = null;
            OnPropertyChanged(GroupedTasksPropertyName);
        }

        private void UpdateGroupedTasks()
        {
            GroupedTasks.Update();
            OnPropertyChanged(GroupedTasksPropertyName);
        }

        public TaskCollection Tasks
        {
            get { return App.Tasks.Tasks; }
        }

        private void TaskItem_Check(object sender, Controls.TaskEventArgs e)
        {
            ToggleComplete(e.Task);
        }

        private void TaskItem_Click(object sender, Controls.TaskEventArgs e)
        {
            if (e.Task == null)
                return;

            NavigateQuery(typeof(EditTaskPage), "?Task={0}", e.Task.Uid);
        }

        private void TaskItem_SubtaskCheck(object sender, TaskSubtaskEventArgs e)
        {
            ToggleComplete(e.Task, e.Subtask);
        }

        private void TaskItem_SubtaskClick(object sender, TaskSubtaskEventArgs e)
        {
            if (e.Task != null)
            {
                SetNavigationParameter(true, "AddSubtask");
                Navigate(typeof(SubtasksPage), e.Task);
            }
        }

        private void OverlayAction(Action action)
        {
            PageOverlayTransitionShow.BeginTime = TimeSpan.FromSeconds(0.25);
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

        private void OverlayAction(Action<TaskModel> action, TaskModel task)
        {
            PageOverlayTransitionShow.BeginTime = TimeSpan.Zero;
            PageOverlayTransitionShow.Begin();
            EventHandler overlayHandler = null;
            overlayHandler = (s, e) =>
            {
                action(task);
                PageOverlayTransitionHide.Begin();
                PageOverlayTransitionShow.Completed -= overlayHandler;
            };
            PageOverlayTransitionShow.Completed += overlayHandler;
        }

        private void OverlayAction(Action<TaskModel, DateTime?, GestureAction> action, TaskModel task, DateTime? date, GestureAction gestureAction)
        {
            PageOverlayTransitionShow.BeginTime = TimeSpan.Zero;
            PageOverlayTransitionShow.Begin();
            EventHandler overlayHandler = null;
            overlayHandler = (s, e) =>
            {
                action(task, date, gestureAction);
                PageOverlayTransitionHide.Begin();
                PageOverlayTransitionShow.Completed -= overlayHandler;
            };
            PageOverlayTransitionShow.Completed += overlayHandler;
        }
        #endregion

        #region Gestures
        private void TaskItem_SwipeLeft(object sender, Controls.TaskEventArgs e)
        {
            ExecuteGesture(Settings.Current.SwipeLeftAction, e);
        }

        private void TaskItem_SwipeRight(object sender, Controls.TaskEventArgs e)
        {
            ExecuteGesture(Settings.Current.SwipeRightAction, e);
        }

        private void TaskItem_SubtaskSwipeLeft(object sender, TaskSubtaskEventArgs e)
        {
            ExecuteGesture(Settings.Current.SwipeLeftAction, e);
        }

        private void TaskItem_SubtaskSwipeRight(object sender, TaskSubtaskEventArgs e)
        {
            ExecuteGesture(Settings.Current.SwipeRightAction, e);
        }

        private void ExecuteGesture(GestureAction action, Controls.TaskEventArgs e)
        {
            if (e.Task == null)
            {
                return;
            }
            TaskModel task = e.Task;

            switch (action)
            {
            case GestureAction.Complete:
                VibrateHelper.Short();
                ToggleComplete(task);
                break;

            case GestureAction.Delete:
                VibrateHelper.Short();
                OverlayAction(() =>
                {
                    e.Item.ResetDelete();
                    App.Tasks.Delete(e.Task);
                });
                e.Delete = true;
                Toast.Show(AppResources.ToastTaskDeleted, App.IconStyle("Delete"));
                break;

            case GestureAction.DueToday:
            case GestureAction.DueTomorrow:
                VibrateHelper.Short();
                if (!task.HasRepeats)
                {
                    DateTime? oldDue = task.DueDate;
                    DateTime newDue = (action == GestureAction.DueToday ? DateTimeExtensions.Today : DateTimeExtensions.Tomorrow);
                    newDue = newDue.SetTime(oldDue ?? Settings.Current.DefaultTime);

                    OverlayAction(SetDueDate, task, newDue, action);
                }
                else
                {
                    Toast.Show(AppResources.GestureCantWithRepeats, App.IconStyle("Warning"));
                }
                break;

            case GestureAction.PostponeDay:
            case GestureAction.PostponeWeek:
                VibrateHelper.Short();
                if (!task.HasRepeats)
                {
                    if (task.HasDueDate)
                    {
                        OverlayAction(Postpone, task, task.DueDate.Value.AddDays((action == GestureAction.PostponeDay ? 1 : 7)), action);
                    }
                    else
                    {
                        OverlayAction(Postpone, task, DateTimeExtensions.Today.AddDays((action == GestureAction.PostponeDay ? 1 : 7)).SetTime(Settings.Current.DefaultTime), action);
                    }
                }
                else
                {
                    Toast.Show(AppResources.GestureCantWithRepeats, App.IconStyle("Warning"));
                }
                break;

            case GestureAction.None:
            default:
                break;
            }
        }

        private void ExecuteGesture(GestureAction action, TaskSubtaskEventArgs e)
        {
            if (e.Task == null || e.Subtask == null)
            {
                return;
            }

            switch (action)
            {
            case GestureAction.Complete:
                VibrateHelper.Short();
                ToggleComplete(e.Task, e.Subtask);
                break;

            case GestureAction.Delete:
                VibrateHelper.Short();
                e.Delete = true;
                Toast.Show(AppResources.ToastSubtaskDeleted, App.IconStyle("Delete"));
                break;

            default:
                break;
            }
        }
        #endregion
    }
}