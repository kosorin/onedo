using Microsoft.Devices;
using SimpleTasks.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTasks.Helpers
{
    public static class VibrateHelper
    {
        public static void Short()
        {
            Start(0.05);
        }

        public static void Start(double seconds)
        {
            if (Settings.Current.General.Vibrate)
            {
                VibrateController.Default.Start(TimeSpan.FromSeconds(seconds));
            }
        }
    }
}
