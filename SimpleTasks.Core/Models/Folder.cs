using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows.Media;
using SimpleTasks.Core.Helpers;

namespace SimpleTasks.Core.Models
{
    [DataContract(Name = "Folder", Namespace = "")]
    public class Folder : BindableBase
    {
        #region IsVirtual
        private bool _isVirtual = false;
        [DataMember(Name = "IsVirtual")]
        public bool IsVirtual
        {
            get { return _isVirtual; }
            set { SetProperty(ref _isVirtual, value); }
        }
        #endregion

        #region Name
        private string _name = "";
        [DataMember(Name = "Name")]
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
        #endregion

        #region Color
        private Color _color = Colors.Transparent;
        [DataMember(Name = "Color")]
        public Color Color
        {
            get { return _color; }
            set { SetProperty(ref _color, value); }
        }
        #endregion

        #region Tasks
        private List<string> _tasks = new List<string>();
        [DataMember(Name = "Tasks")]
        public List<string> Tasks
        {
            get { return _tasks; }
            set { SetProperty(ref _tasks, value); }
        }
        #endregion
    }
}
