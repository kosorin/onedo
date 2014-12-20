﻿using System;
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
            ApplicationBar = ThemeHelper.CreateApplicationBar();

            ApplicationBar.Buttons.Add(appBarNewTaskButton);
            foreach (var item in appBarMenuItems)
            {
                ApplicationBar.MenuItems.Add(item);
            }
        }

        private void BuildQuickAddAppBar()
        {
            ApplicationBar = ThemeHelper.CreateApplicationBar();
            ApplicationBar.Buttons.Add(appBarSaveQuickButton);
        }
        #endregion

        private void AddNewTask_Click(object sender, EventArgs e)
        {
            Navigate(typeof(EditTaskPage));
        }

        void QuickAddSave(object sender, EventArgs e)
        {
            QuickAdd(QuickAddTextBox.Text);
        }

        private void DeleteCompletedItem_Click(object sender, EventArgs e)
        {
            OverlayAction(App.Tasks.DeleteCompleted);
            Toast.Show(AppResources.ToastCompletedTasksDeleted, App.IconStyle("Delete"));
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
            else if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "it")
            {
                App.Tasks.Tasks[0].Title = "Lista della spesa";
                App.Tasks.Tasks[0].Subtasks[0].Text = "latte";
                App.Tasks.Tasks[0].Subtasks[1].Text = "mele";
                App.Tasks.Tasks[0].Subtasks[2].Text = "patate";
                App.Tasks.Tasks[0].Subtasks[3].Text = "prosciutto";
                App.Tasks.Tasks[0].Subtasks[4].Text = "biscotti";
                App.Tasks.Tasks[1].Title = "Progetto di matematica";
                App.Tasks.Tasks[1].Detail = "In nec massa semper dolor sodales molestie. Curabitur sagittis consequat imperdiet. Aliquam nec mattis enim. Nam vitae ligula quis ligula viverra pharetra ac in arcu. Pellentesque luctus, urna sit amet porta commodo, quam dui maximus massa, in vestibulum lacus elit eu orci. Phasellus lacinia lobortis feugiat.";
                App.Tasks.Tasks[2].Title = "Chiamare Francesco";
            }
            else if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "ru")
            {
                App.Tasks.Tasks[0].Title = "Список покупок";
                App.Tasks.Tasks[0].Subtasks[0].Text = "молоко";
                App.Tasks.Tasks[0].Subtasks[1].Text = "яблоки";
                App.Tasks.Tasks[0].Subtasks[2].Text = "помидоры";
                App.Tasks.Tasks[0].Subtasks[3].Text = "колбаса";
                App.Tasks.Tasks[0].Subtasks[4].Text = "выпечка";
                App.Tasks.Tasks[1].Title = "Математический проект";
                App.Tasks.Tasks[1].Detail = "Не Весельем не ли отчаяньи То жаждущим снислала исчезает Он прошенье. Процветут Кто ров страстьми маловерах достигать отвращуся Без Праведник очи. Во та бы ту. Укреплять псалтирям благоволи правитель множества. Из из Се от НА. Как дать мое ведя тон Числ Нивы шаг Чтя скал пущу.";
                App.Tasks.Tasks[2].Title = "Позвонить Иван";
            }
        }
#endif
        #endregion

        #region TasksList
        private void TaskItem_Check(object sender, Controls.TaskEventArgs e)
        {
            ToggleComplete(e.Task);
        }

        private void ToggleComplete(TaskModel task)
        {
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
                ToggleSubtaskComplete(element.DataContext as Subtask);
            }
        }

        private void ToggleSubtaskComplete(Subtask subtask)
        {
            if (subtask != null)
            {
                subtask.IsCompleted = !subtask.IsCompleted;
            }
        }

        private void TaskItem_Click(object sender, Controls.TaskEventArgs e)
        {
            if (e.Task == null)
                return;

            NavigateQuery(typeof(EditTaskPage), "?Task={0}", e.Task.Uid);
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
                    task.DueDate = task.DueDate.Value.SetTime(Settings.Current.Tasks.DefaultTime);
                }

                App.Tasks.Add(task);
                QuickAddTextBox.Text = "";
                this.Focus();

                TasksLongListSelector.ScrollTo(task);
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
        private void SetDueDate(TaskModel task, DateTime? due, GestureAction action)
        {
            //bool hadReminder = task.ExistsSystemReminder();

            task.DueDate = due;
            App.Tasks.Update(task);
            OnPropertyChanged(GroupedTasksProperty);

            //if (hadReminder && !task.ExistsSystemReminder())
            //{
            //    Toast.Show(AppResources.ToastReminderOff, GestureActionHelper.IconStyle(action), AppResources.ToastNotice);
            //}
        }

        private void Postpone(TaskModel task, DateTime? due, GestureAction action)
        {
            task.DueDate = due;
            App.Tasks.Update(task);
            TaskWrapper wrapper = task.Wrapper as TaskWrapper;
            if (wrapper != null)
            {
                wrapper.UpdateIsScheduled();
            }
            OnPropertyChanged(GroupedTasksProperty);

            Toast.Show(string.Format(AppResources.ToastPostponedUntil, task.DueDate), GestureActionHelper.IconStyle(action));
        }

        private void ExecuteGesture(GestureAction action, TaskModel task)
        {
            if (task == null)
            {
                return;
            }

            switch (action)
            {
            case GestureAction.Complete:
                VibrateHelper.Short();
                ToggleComplete(task);
                break;

            case GestureAction.Delete:
                VibrateHelper.Short();
                OverlayAction(App.Tasks.Delete, task);
                Toast.Show(AppResources.ToastTaskDeleted, App.IconStyle("Delete"));
                break;

            case GestureAction.DueToday:
            case GestureAction.DueTomorrow:
                VibrateHelper.Short();
                {
                    DateTime? oldDue = task.DueDate;
                    DateTime newDue = (action == GestureAction.DueToday ? DateTimeExtensions.Today : DateTimeExtensions.Tomorrow);
                    newDue = newDue.SetTime(oldDue ?? Settings.Current.Tasks.DefaultTime);

                    OverlayAction(SetDueDate, task, newDue, action);
                }
                break;

            case GestureAction.PostponeDay:
            case GestureAction.PostponeWeek:
                VibrateHelper.Short();
                if (task.HasDueDate)
                {
                    OverlayAction(Postpone, task, task.DueDate.Value.AddDays((action == GestureAction.PostponeDay ? 1 : 7)), action);
                }
                else
                {
                    OverlayAction(Postpone, task, DateTimeExtensions.Today.AddDays((action == GestureAction.PostponeDay ? 1 : 7)).SetTime(Settings.Current.Tasks.DefaultTime), action);
                }
                break;

            case GestureAction.None:
            default:
                break;
            }
        }

        private void ExecuteSubtaskGesture(GestureAction action, TaskModel task, Subtask subtask)
        {
            if (task == null || subtask == null)
            {
                return;
            }

            switch (action)
            {
            case GestureAction.Complete:
                VibrateHelper.Short();
                ToggleSubtaskComplete(subtask);
                break;

            case GestureAction.Delete:
                VibrateHelper.Short();
                App.Tasks.DeleteSubtask(subtask);
                Toast.Show(AppResources.ToastSubtaskDeleted, App.IconStyle("Delete"));
                break;

            default:
                break;
            }
        }

        private void TaskItem_SwipeLeft(object sender, Controls.TaskEventArgs e)
        {
            ExecuteGesture(Settings.Current.Tasks.SwipeLeftAction, e.Task);
        }

        private void TaskItem_SwipeRight(object sender, Controls.TaskEventArgs e)
        {
            ExecuteGesture(Settings.Current.Tasks.SwipeRightAction, e.Task);
        }
        #endregion
    }
}