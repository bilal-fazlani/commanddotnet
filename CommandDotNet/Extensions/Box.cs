namespace CommandDotNet.Extensions
{
    internal class Box<T>
    {
        public T? Value { get; }

        public Box(T value)
        {
            Value = value;
        }

        public static Box<T> CreateDefault() => new(default!);
    }
}