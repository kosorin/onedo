using SimpleTasks.Core.Helpers;
using SimpleTasks.Helpers;
using SimpleTasks.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace SimpleTasks
{
    public class MyUriMapper: UriMapperBase
    {
        public override Uri MapUri(Uri uri)
        {
            if (uri.OriginalString == "/Views/EntryPage.xaml")
            {
                if (App.Settings.FirstStartSetting)
                {
                    App.Settings.FirstStartSetting = false;
                    App.Settings.EnableLiveTileSetting = true;

                    uri = new Uri("/Views/MainPage.xaml", UriKind.Relative);
                }
                else
                {
                    uri = new Uri("/Views/MainPage.xaml", UriKind.Relative);
                }
            }
            return uri;
        } 
    }
}
