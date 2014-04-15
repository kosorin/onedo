using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
    }
}
