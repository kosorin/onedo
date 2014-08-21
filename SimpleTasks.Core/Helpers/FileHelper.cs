using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;


namespace SimpleTasks.Core.Helpers
{
    public static class FileHelper
    {
        public static T LoadFromJson<T>(string fileName) where T : class, new()
        {
            Debug.WriteLine(string.Format("> Nahrávám '{1}' ze souboru '{0}'...", fileName, typeof(T).Name));

            T collection = new T();
            try
            {
                using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (isf.FileExists(fileName))
                    {
                        using (Stream stream = isf.OpenFile(fileName, FileMode.Open, FileAccess.Read))
                        {
                            StreamReader sr = new StreamReader(stream);
                            collection = JsonConvert.DeserializeObject<T>(sr.ReadToEnd());
                            sr.Close();
                            Debug.WriteLine(": Nahrávání '{0}' dokončeno.", typeof(T).Name);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(": Chyba při nahrávání složek: {0}", e.Message);
            }

            return collection;
        }

        public static void SaveToJson<T>(string fileName, T collection) where T : class
        {
            Debug.WriteLine(string.Format("> Ukládám '{1}' do souboru '{0}'...", fileName, typeof(T).Name));

            string data = JsonConvert.SerializeObject(collection, Formatting.Indented);
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (Stream stream = isf.OpenFile(fileName, FileMode.Create, FileAccess.Write))
                {
                    StreamWriter sw = new StreamWriter(stream);
                    sw.Write(data);
                    sw.Flush();
                    sw.Close();
                    Debug.WriteLine(": Ukládání '{0}' dokončeno.", typeof(T).Name);
                }
            }
        }
    }
}
