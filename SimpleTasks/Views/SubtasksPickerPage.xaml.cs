using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.LocalizedResources;
using SimpleTasks.ViewModels;
using Microsoft.Phone.Shell;
using SimpleTasks.Resources;
using SimpleTasks.Controls;
using System.Diagnostics;
using SimpleTasks.Models;
using System.Collections.Generic;
using System.Windows.Media.Animation;
using SimpleTasks.Core.Models;
using System.Collections.ObjectModel;
using SubtaskCollection = System.Collections.ObjectModel.ObservableCollection<SimpleTasks.Core.Models.Subtask>;

namespace SimpleTasks.Views
{
    public partial class SubtasksPickerPage : BasePickerPage
    {
        private SubtaskCollection _subtasks = new SubtaskCollection();
        public SubtaskCollection Subtasks
        {
            get { return _subtasks; }
            set { SetProperty(ref _subtasks, value); }
        }

        public SubtasksPickerPage()
        {
            if (CanRetrieve("Subtasks"))
                Subtasks = RetrieveAndConfigure<SubtaskCollection>("Subtasks") ?? Subtasks;

            InitializeComponent();
            DataContext = this;
        }

        protected override void Save()
        {
            SetValueToSave(Subtasks);
        }

        #region Subtasks
        private VerticalAlignment _subtasksAlignment = VerticalAlignment.Stretch;
        public VerticalAlignment SubtasksAlignment
        {
            get { return _subtasksAlignment; }
            set { SetProperty(ref _subtasksAlignment, value); }
        }

        private void AddSubtask()
        {
            if (!string.IsNullOrWhiteSpace(SubtaskTextBox.Text))
            {
                SubtaskListBox.AnimateRearrange(TimeSpan.FromSeconds(0.22), delegate
                {
                    Subtasks.Add(new Subtask(SubtaskTextBox.Text));
                    SubtaskTextBox.Text = "";
                });
            }
        }

        private void DeleteSubtask(Subtask subtask)
        {
            if (subtask != null)
            {
                SubtaskListBox.AnimateRearrange(TimeSpan.FromSeconds(0.22), delegate
                {
                    Subtasks.Remove(subtask);
                });
            }
            this.Focus();
        }

        private void SubtaskListBox_Delete_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ContentControl cc = sender as ContentControl;
            if (cc != null)
            {
                DeleteSubtask(cc.DataContext as Subtask);
            }
        }

        private void SubtaskTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SubtasksAlignment = VerticalAlignment.Bottom;
            BuildSubtasksAppBar();
        }

        private void SubtaskTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            SubtasksAlignment = VerticalAlignment.Stretch;
            BuildAppBar();
        }

        private void SubtaskTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddSubtask();
            }
        }
        #endregion

        #region AppBar
        private ApplicationBarIconButton appBarAddSubtaskButton = null;

        private ApplicationBarIconButton appBarCompleteAllSubtasksButton = null;

        protected override void BuildAppBar()
        {
            base.BuildAppBar();


            if (appBarCompleteAllSubtasksButton == null)
            {
                appBarCompleteAllSubtasksButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.list.check.png", UriKind.Relative));
                appBarCompleteAllSubtasksButton.Text = AppResources.AppBarCompleteAllSubtasks;
                appBarCompleteAllSubtasksButton.Click += CompleteAllSubtasks;
            }
            ApplicationBar.Buttons.Insert(1, appBarCompleteAllSubtasksButton);

        }

        private void BuildSubtasksAppBar()
        {
            ApplicationBar = new ApplicationBar();

            if (appBarAddSubtaskButton == null)
            {
                appBarAddSubtaskButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.png", UriKind.Relative));
                appBarAddSubtaskButton.Text = AppResources.AppBarAddSubtask;
                appBarAddSubtaskButton.Click += AddSubtask;
            }
            ApplicationBar.Buttons.Add(appBarAddSubtaskButton);
        }

        private void AddSubtask(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SubtaskTextBox.Text))
            {
                SubtaskTextBox.Focus();
            }
            else
            {
                AddSubtask();
            }
        }

        private void CompleteAllSubtasks(object sender, EventArgs e)
        {
            foreach (Subtask subtask in Subtasks)
            {
                subtask.IsCompleted = true;
            }
        }
        #endregion
    }
}