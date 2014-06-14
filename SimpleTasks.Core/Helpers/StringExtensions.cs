using System;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace SimpleTasks.Core.Helpers
{
    public static class StringExtensions
    {
        public static string NormalizeNewLines(this string value)
        {
            if (value == null)
                return value;
            return Regex.Replace(value, @"\r(?!\n)|(?<!\r)\n", Environment.NewLine);
        }

        public static long LineCount(this string s)
        {
            int newLineLength = Environment.NewLine.Length;
            long count = 1;
            int position = 0;
            while ((position = s.IndexOf(Environment.NewLine, position)) != -1)
            {
                count++;
                position += newLineLength;
            }
            return count;
        }

        public static string[] Lines(this string s)
        {
            return Regex.Split(s, "\r\n|\r|\n");
        }

        public static int LineNumberAtPosition(this string s, int lastPosition)
        {
            int count = 0;
            int length = s.Length;

            char f, n = '\0';
            for (int i = 0; i < lastPosition; ++i)
            {
                f = s[i];
                if ((i + 1) < length)
                    n = s[i + 1];
                else
                    n = '\0';

                if (f == '\r')
                {
                    if (n == '\n')
                    {
                        count++;
                        ++i;
                    }
                    else
                    {
                        count++;
                    }
                }
                else if (n == '\n')
                {
                    count++;
                }
            }

            return count;
        }

        //public static string ToTagName(this string value)
        //{
        //    string charsFrom = "ÀÁÂÃÄÅÇÈÉÊËÌÍÎÏÑÒÓÔÕÖÙÚÛÜÝßàáâãäåçèéêëìíîïñòóôõöùúûüýÿĀāĂăĄąĆćĈĉĊċČčĎďĐđĒēĔĕĖėĘęĚěĜĝĞğĠġĢģĤĥĦħĨĩĪīĬĭĮįİıĶķĸĹĺĻļĽľĿŀŁłŃńŅņŇňŉŊŋŌōŎŏŐőŔŕŖŗŘřŚśŜŝŞşŠšŢţŤťŦŧŨũŪūŬŭŮůŰűŲųŴŵŶŷŸŹźŻżŽžſ";
        //    string charsTo = "AAAAAACEEEEIIIINOOOOOUUUUYsaaaaaaceeeeiiiinooooouuuuyyAaAaAaCcCcCcCcDdDdEeEeEeEeEeGgGgGgGgHhHhIiIiIiIiIiKkkLlLlLlLlLlNnNnNnNnNOoOoOoRrRrRrSsSsSsSsTtTtTtUuUuUuUuUuUuWwYyYZzZzZzs";

        //    return value.Replace(
        //}
    }
}
