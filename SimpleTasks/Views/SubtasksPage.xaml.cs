using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SimpleTasks.Controls;
using SimpleTasks.Core.Models;
using SimpleTasks.Helpers;
using SimpleTasks.Resources;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;
using SubtaskCollection = System.Collections.ObjectModel.ObservableCollection<SimpleTasks.Core.Models.Subtask>;

namespace SimpleTasks.Views
{
    public partial class SubtasksPage : BasePage
    {
        private SubtaskCollection _subtasks = new SubtaskCollection();
        public SubtaskCollection Subtasks
        {
            get { return _subtasks; }
            set { SetProperty(ref _subtasks, value); }
        }

        private readonly TaskModel _task = null;

        public SubtasksPage()
        {
            if (IsSetNavigationParameter())
            {
                _task = NavigationParameter<TaskModel>();
                Subtasks = _task.Subtasks;
            }

            if (IsSetNavigationParameter("AddSubtask"))
            {
                if (NavigationParameter<bool>("AddSubtask"))
                {
                    _subtaskToEdit = new Subtask();
                    Subtasks.Add(_subtaskToEdit);
                }
            }

            InitializeComponent();
            DataContext = this;
            BuildAppBar();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            TiltEffect.TiltableItems.Remove(typeof(ListBoxItem));
            base.OnNavigatedTo(e);

            if (_task != null)
            {
                _task.ModifiedSinceStart = true;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            TiltEffect.TiltableItems.Add(typeof(ListBoxItem));
            base.OnNavigatedFrom(e);

            if (Subtasks.All(s => s.IsCompleted))
            {
                App.Tasks.Complete(_task);
            }
        }

        #region Subtasks
        private Subtask _subtaskToEdit = null;

        private TextBox _currentTextBox = null;

        private const double _defaultAnimateDuration = 0.22;

        private void AddSubtask()
        {
            if (_currentTextBox != null && string.IsNullOrWhiteSpace(_currentTextBox.Text))
            {
                Subtask currentSubtask = _currentTextBox.DataContext as Subtask;
                if (currentSubtask.Text != null)
                {
                    currentSubtask.Text = "";
                    currentSubtask.IsCompleted = false;
                }
            }
            else
            {
                _subtaskToEdit = new Subtask();
                Subtasks.Add(_subtaskToEdit);
            }
        }

        private void DeleteSubtask(Subtask subtask, double duration = 0)
        {
            if (subtask != null)
            {
                if (duration > 0)
                {
                    SubtaskListBox.AnimateRearrange(TimeSpan.FromSeconds(duration), delegate
                    {
                        Subtasks.Remove(subtask);
                    });
                }
                else
                {
                    Subtasks.Remove(subtask);
                }
            }
        }

        private void SubtaskListBox_Delete_Tap(object sender, GestureEventArgs e)
        {
            ContentControl cc = sender as ContentControl;
            if (cc != null)
            {
                DeleteSubtask(cc.DataContext as Subtask, _defaultAnimateDuration);
            }
        }

        private void SubtaskTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SubtaskListBox.IsReorderEnabled = false;
            _currentTextBox = sender as TextBox;
        }

        private void SubtaskTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            _currentTextBox = null;
            TextBox tb = sender as TextBox;
            if (tb != null)
            {
                Subtask subtask = tb.DataContext as Subtask;
                if (subtask != null)
                {
                    if (string.IsNullOrWhiteSpace(tb.Text))
                    {
                        DeleteSubtask(subtask);
                    }
                }
            }
            SubtaskListBox.IsReorderEnabled = true;
        }

        private void SubtaskTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddSubtask();
            }
        }

        private void SubtaskTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb != null)
            {
                Subtask subtask = tb.DataContext as Subtask;
                if (subtask != null && _subtaskToEdit == subtask)
                {
                    _subtaskToEdit = null;
                    tb.Focus();
                }
            }
        }
        #endregion

        #region AppBar
        private ApplicationBarIconButton _appBarAddSubtaskButton = null;

        private ApplicationBarIconButton _appBarCompleteAllSubtasksButton = null;

        private ApplicationBarIconButton _appBarDeleteAllSubtasksButton = null;

        private void BuildAppBar()
        {
            ApplicationBar = ThemeHelper.CreateApplicationBar();

            if (_appBarAddSubtaskButton == null)
            {
                _appBarAddSubtaskButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.png", UriKind.Relative));
                _appBarAddSubtaskButton.Text = AppResources.AppBarAddSubtask;
                _appBarAddSubtaskButton.Click += AddSubtask_Click;
            }
            ApplicationBar.Buttons.Add(_appBarAddSubtaskButton);
            if (_appBarCompleteAllSubtasksButton == null)
            {
                _appBarCompleteAllSubtasksButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.check.all.png", UriKind.Relative));
                _appBarCompleteAllSubtasksButton.Text = AppResources.AppBarCompleteAllSubtasks;
                _appBarCompleteAllSubtasksButton.Click += CompleteAllSubtasks_Click;
            }
            ApplicationBar.Buttons.Add(_appBarCompleteAllSubtasksButton);
            if (_appBarDeleteAllSubtasksButton == null)
            {
                _appBarDeleteAllSubtasksButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.delete.all.png", UriKind.Relative));
                _appBarDeleteAllSubtasksButton.Text = AppResources.AppBarDeleteAllSubtasks;
                _appBarDeleteAllSubtasksButton.Click += DeleteAllSubtasks_Click;
            }
            ApplicationBar.Buttons.Add(_appBarDeleteAllSubtasksButton);
        }

        private void AddSubtask_Click(object sender, EventArgs e)
        {
            AddSubtask();
        }

        private void CompleteAllSubtasks_Click(object sender, EventArgs e)
        {
            foreach (Subtask subtask in Subtasks)
            {
                subtask.IsCompleted = true;
            }
        }

        private void DeleteAllSubtasks_Click(object sender, EventArgs e)
        {
            SubtaskListBox.AnimateRearrange(TimeSpan.FromSeconds(_defaultAnimateDuration), delegate
            {
                Subtasks.Clear();
            });
        }
        #endregion
    }
}