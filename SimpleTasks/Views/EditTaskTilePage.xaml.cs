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
using SimpleTasks.Helpers.Analytics;

namespace SimpleTasks.Views
{
    public partial class EditTaskTilePage : BasePage
    {
        private TaskModel _task = null;

        /// <summary>
        /// 0 - small
        /// 1 - medium
        /// 2 - wide
        /// </summary>
        private int _currentSize = 0;

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

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            GoogleAnalyticsHelper.SetDimension(CustomDimension.TaskTileLineHeight, ((int)_task.TileSettings.LineHeight).ToString());
            GoogleAnalyticsHelper.SendEvent(EventCategory.Tasks, EventAction.Edit, "edit task tile line height");
        }

        private void SetTask(TaskModel task)
        {
            _task = task;
            DataContext = _task.TileSettings ?? (_task.TileSettings = new TaskTileSettings());

            SmallTile.Task = _task;
            MediumTile.Task = _task;
            WideTile.Task = _task;

            ChangeTileSize();
        }

        private void Refresh()
        {
            switch (_currentSize)
            {
            case 2: WideTile.Refresh(); break;
            case 1: MediumTile.Refresh(); break;
            case 0:
            default: SmallTile.Refresh(); break;
            }
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

        private void ChangeTileSize()
        {
            switch (_currentSize)
            {
            case 2: _currentSize = 1; ToMedium.Begin(); break;
            case 1: _currentSize = 0; ToSmall.Begin(); break;
            case 0:
            default: _currentSize = 2; ToWide.Begin(); break;
            }
            Refresh();
        }

        private void ChangeSizeButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ChangeTileSize();
        }
    }
}