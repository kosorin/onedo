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
