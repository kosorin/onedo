using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTasks.Models
{
    public class ValueToString<T>
    {
        public T Value { get; private set; }

        private string _toString;

        public ValueToString(T value, string toString)
        {
            Value = value;
            _toString = toString;
        }

        public override string ToString()
        {
            return _toString;
        }
    }
}
