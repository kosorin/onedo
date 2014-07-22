using SimpleTasks.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTasks.Core.Models
{
    public abstract class SettingsCategory : BindableBase
    {
        public Settings Settings { get; protected set; }

        public SettingsCategory(Settings settings)
        {
            Settings = settings;
        }
    }
}
