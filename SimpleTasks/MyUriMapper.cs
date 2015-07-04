using System;
using System.Windows.Navigation;

namespace SimpleTasks
{
    public class MyUriMapper: UriMapperBase
    {
        public override Uri MapUri(Uri uri)
        {
            if (uri.OriginalString == "/Views/EntryPage.xaml")
            {
                uri = new Uri("/Views/MainPage.xaml", UriKind.Relative);
            }
            return uri;
        } 
    }
}
