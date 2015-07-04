using Microsoft.Phone.Controls.LocalizedResources;
using SimpleTasks.Resources;

namespace SimpleTasks
{
    /// <summary>
    /// Provides access to string resources.
    /// </summary>
    public class LocalizedStrings
    {
        private static AppResources _localizedResources = new AppResources();

        public AppResources LocalizedResources { get { return _localizedResources; } }

        public string On { get { return ControlResources.On; } }

        public string Off { get { return ControlResources.Off; } }
    }
}