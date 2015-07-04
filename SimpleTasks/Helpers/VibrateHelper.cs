using System;
using Microsoft.Devices;
using SimpleTasks.Core.Models;

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
            if (Settings.Current.Vibrate)
            {
                VibrateController.Default.Start(TimeSpan.FromSeconds(seconds));
            }
        }
    }
}
