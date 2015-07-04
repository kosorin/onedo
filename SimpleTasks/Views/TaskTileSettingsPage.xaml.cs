using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using SimpleTasks.Controls;
using SimpleTasks.Core.Models;

namespace SimpleTasks.Views
{
    public partial class TaskTileSettingsPage : BasePage
    {
        private TaskModel _task = null;

        /// <summary>
        /// 0 - small
        /// 1 - medium
        /// 2 - wide
        /// </summary>
        private int _currentSize = 0;

        private bool _useDefaultLineHeight = false;

        public TaskTileSettingsPage()
        {
            InitializeComponent();

            SetTask(NavigationParameter<TaskModel>(DefaultParameterKey, new TaskModel()));
            try
            {
                _useDefaultLineHeight = (_task == null || _task.TileSettings == Settings.Current.DefaultTaskTileSettings);
            }
            catch { }
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
            TaskTileSettings settings = _task.TileSettings ?? (_task.TileSettings = Settings.Current.DefaultTaskTileSettings.Clone());

            DataContext = settings;
            LineHeightSlider.Value = (double)settings.LineHeight;

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

        private void LineHeight_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_task != null)
            {
                _task.TileSettings.LineHeight = Math.Round(e.NewValue);
                Refresh();
            }
        }

        private void SettingChanged(object sender, GestureEventArgs e)
        {
            Refresh();
        }

        //private void ResetLineHeight_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        //{
        //    double newLineHeight = _useDefaultLineHeight ? TaskTileSettings.DefaultLineHeight : Settings.Current.DefaultTaskTileSettings.LineHeight;
        //    LineHeightSlider.Value = newLineHeight;
        //}

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

        private void ChangeSizeButton_Tap(object sender, GestureEventArgs e)
        {
            ChangeTileSize();
        }
    }
}