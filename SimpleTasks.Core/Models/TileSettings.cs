using SimpleTasks.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTasks.Core.Models
{
    [DataContract(Name = "TileSettings", Namespace = "")]
    public class TileSettings : BindableBase
    {
        static TileSettings()
        {
            Default = new TileSettings();
        }

        public static TileSettings Default { get; private set; }

        public TileSettings() { }

        #region LineHeight
        private double _lineHeight = 48D;
        [DataMember(Order = 1)]
        public double LineHeight
        {
            get { return _lineHeight; }
            set { SetProperty(ref _lineHeight, value); }
        }
        #endregion

        #region BackgroundOpacity
        private double _backgroundOpacity = 0.65;
        [DataMember(Order = 1)]
        public double BackgroundOpacity
        {
            get { return _backgroundOpacity; }
            set { SetProperty(ref _backgroundOpacity, value); }
        }
        #endregion
    }
}
