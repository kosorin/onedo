using SimpleTasks.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTasks.Core.Models
{
    [DataContract(Name = "TaskTileSettings", Namespace = "")]
    public class TaskTileSettings : BindableBase
    {
        public TaskTileSettings() { }

        public TaskTileSettings Clone()
        {
            TaskTileSettings newSettings = new TaskTileSettings();

            newSettings.LineHeight = LineHeight;
            newSettings.BackgroundOpacity = BackgroundOpacity;
            newSettings.TitleOnOneLine = TitleOnOneLine;
            newSettings.ShowCompletedSubtasks = ShowCompletedSubtasks;
            newSettings.ShowTitle = ShowTitle;
            newSettings.ShowDate = ShowDate;

            return newSettings;
        }

        #region LineHeight
        public const double DefaultLineHeight = 48;

        private double _lineHeight = DefaultLineHeight;
        [DataMember()]
        public double LineHeight
        {
            get { return _lineHeight; }
            set { SetProperty(ref _lineHeight, value); }
        }
        #endregion

        #region BackgroundOpacity
        private double _backgroundOpacity = 0.45;
        [DataMember()]
        public double BackgroundOpacity
        {
            get { return _backgroundOpacity; }
            set { SetProperty(ref _backgroundOpacity, value); }
        }
        #endregion

        #region TitleOnOneLine
        private bool _titleOnOneLine = false;
        [DataMember()]
        public bool TitleOnOneLine
        {
            get { return _titleOnOneLine; }
            set { SetProperty(ref _titleOnOneLine, value); }
        }
        #endregion

        #region ShowCompletedSubtasks
        private bool _showCompletedSubtasks = true;
        [DataMember()]
        public bool ShowCompletedSubtasks
        {
            get { return _showCompletedSubtasks; }
            set { SetProperty(ref _showCompletedSubtasks, value); }
        }
        #endregion

        #region ShowTitle
        private bool _showTitle = true;
        [DataMember()]
        public bool ShowTitle
        {
            get { return _showTitle; }
            set { SetProperty(ref _showTitle, value); }
        }
        #endregion

        #region ShowDate
        private bool _showDate = true;
        [DataMember()]
        public bool ShowDate
        {
            get { return _showDate; }
            set { SetProperty(ref _showDate, value); }
        }
        #endregion
    }
}
