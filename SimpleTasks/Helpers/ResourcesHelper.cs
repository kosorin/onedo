using System;
using System.IO;
using System.Windows;
using System.Windows.Resources;

namespace SimpleTasks.Helpers
{
    public static class ResourcesHelper
    {
        public static string ReadTextFile(string filePath)
        {
            //this verse is loaded for the first time so fill it from the text file
            StreamResourceInfo rsInfo = Application.GetResourceStream(new Uri(filePath, UriKind.Relative));
            if (rsInfo != null)
            {
                using (Stream stream = rsInfo.Stream)
                {
                    if (stream.CanRead)
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            //read the content here
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
            return null;
        }
    }
}
