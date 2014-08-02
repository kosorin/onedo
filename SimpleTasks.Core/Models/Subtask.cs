using SimpleTasks.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTasks.Core.Models
{
    [DataContract(Name = "Subtask", Namespace = "")]
    public class Subtask : BindableBase
    {
        public Subtask() { }

        public Subtask(string text)
        {
            Text = text;
        }

        #region Text
        private string _text = "";
        [DataMember(Order = 1)]
        public string Text
        {
            get { return _text; }
            set { SetProperty(ref _text, value); }
        }
        #endregion

        #region IsCompleted
        private bool _isCompleted = false;
        [DataMember(Order = 2)]
        public bool IsCompleted
        {
            get { return _isCompleted; }
            set { SetProperty(ref _isCompleted, value); }
        }
        #endregion
    }
}
