using System;

namespace CommandDotNet.TypeDescriptors
{
    public class DelegatedTypeDescriptor<T> : IArgumentTypeDescriptor
    {
        private readonly string _displayName;
        private readonly Func<string, object> _parseValueDelegate;

        public DelegatedTypeDescriptor(string displayName, Func<string, object> parseValueDelegate)
        {
            _displayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
            _parseValueDelegate = parseValueDelegate ?? throw new ArgumentNullException(nameof(parseValueDelegate));
        }

        public bool CanSupport(Type type)
        {
            return type == typeof(T);
        }

        public string GetDisplayName(IArgument argument)
        {
            return _displayName;
        }

        public object ParseString(IArgument argument, string value)
        {
            return _parseValueDelegate(value);
        }

        public override string ToString()
        {
            return $"DelegatedTypeDescriptor<{typeof(T).Name}>: '{_displayName}'";
        }
    }
}