using GoogleAnalytics;
using GoogleAnalytics.Core;
using SimpleTasks.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTasks.Helpers.Analytics
{
    public static class GoogleAnalyticsHelper
    {
        private static Tracker _tracker = null;
        private static Tracker Tracker
        {
            get
            {
                if (_tracker == null)
                {
                    _tracker = EasyTracker.GetTracker();
                }
                return _tracker;
            }
        }

        public static void SendPage(BasePage page)
        {
            try
            {
                if (page != null)
                {
                    Tracker.SendView(page.GetType().Name);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("> GoogleAnalyticsHelper SendPage exception: {0}", e.Message);
            }
        }

        public static void SendEvent(EventCategory category, EventAction action, string label = "<Label>", long value = 0)
        {
            try
            {
                Tracker.SendEvent(category.ToString(), action.ToString(), label, value);
            }
            catch (Exception e)
            {
                Debug.WriteLine("> GoogleAnalyticsHelper SendEvent exception: {0}", e.Message);
            }
        }
    }
}
