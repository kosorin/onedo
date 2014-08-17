using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SimpleTasks.Controls;
using SimpleTasks.Core.Models;
using System.Diagnostics;

namespace SimpleTasks.Views
{
    public partial class EditTaskTilePage : BasePage
    {
        private TaskModel _task = null;

        public EditTaskTilePage()
        {
            InitializeComponent();

            SetTask(NavigationParameter<TaskModel>());
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (_task != null)
            {
                _task.ModifiedSinceStart = true;
            }
        }

        private void SetTask(TaskModel task)
        {
            _task = task;
            DataContext = _task.TileSettings ?? (_task.TileSettings = new TaskTileSettings());

            SmallTile.Task = _task;
            MediumTile.Task = _task;
            WideTile.Task = _task;

            Refresh();
        }

        private void Refresh()
        {
            SmallTile.Refresh();
            MediumTile.Refresh();
            WideTile.Refresh();
        }

        private void LineHeight_RoundValueChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
        {
            if (_task != null)
            {
                _task.TileSettings.LineHeight = e.NewValue;
                Refresh();
            }
        }

        private void SettingChanged(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Refresh();
        }

        private void ResetLineHeight_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            LineHeightSlider.SetSliderValue((int)TaskTileSettings.Default.LineHeight);
        }
    }
}