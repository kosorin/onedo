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
            newSettings.HideTitle = HideTitle;
            newSettings.HideDate = HideDate;

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

        #region HideTitle
        private bool _hideTitle = false;
        [DataMember()]
        public bool HideTitle
        {
            get { return _hideTitle; }
            set { SetProperty(ref _hideTitle, value); }
        }
        #endregion

        #region HideDate
        private bool _hideDate = false;
        [DataMember()]
        public bool HideDate
        {
            get { return _hideDate; }
            set { SetProperty(ref _hideDate, value); }
        }
        #endregion
    }
}
