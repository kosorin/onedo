﻿using System;
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
            if (IsSetParam("SubtaskToEdit"))
            {
                _subtaskToEdit = GetParam<Subtask>("SubtaskToEdit");
            }

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
        private Subtask _subtaskToEdit = null;

        private void AddSubtask()
        {
            SubtaskListBox.AnimateRearrange(TimeSpan.FromSeconds(0.22), delegate
            {
                _subtaskToEdit = new Subtask();
                Subtasks.Add(_subtaskToEdit);
            });
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
        }

        private void SubtaskTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
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

        protected override void BuildAppBar()
        {
            ApplicationBar = new ApplicationBar();

            if (_appBarAddSubtaskButton == null)
            {
                _appBarAddSubtaskButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.png", UriKind.Relative));
                _appBarAddSubtaskButton.Text = AppResources.AppBarAddSubtask;
                _appBarAddSubtaskButton.Click += AddSubtask_Click;
            }
            ApplicationBar.Buttons.Add(_appBarAddSubtaskButton);
            if (_appBarCompleteAllSubtasksButton == null)
            {
                _appBarCompleteAllSubtasksButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.list.check.png", UriKind.Relative));
                _appBarCompleteAllSubtasksButton.Text = AppResources.AppBarCompleteAllSubtasks;
                _appBarCompleteAllSubtasksButton.Click += CompleteAllSubtasks_Click;
            }
            ApplicationBar.Buttons.Add(_appBarCompleteAllSubtasksButton);
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
        #endregion
    }
}