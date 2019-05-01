using System;
using CommandDotNet.Models;

namespace CommandDotNet.TypeDescriptors
{
    public class DelegatedTypeDescriptor<T> : IArgumentTypeDescriptor
    {
        private readonly string _displayName;
        private readonly Func<ArgumentInfo, string, object> _parseValueDelegate;

        public DelegatedTypeDescriptor(string displayName, Func<ArgumentInfo, string, object> parseValueDelegate)
        {
            _displayName = displayName;
            _parseValueDelegate = parseValueDelegate;
        }

        public bool CanSupport(Type type)
        {
            return type == typeof(T);
        }

        public string GetDisplayName(ArgumentInfo argumentInfo)
        {
            return _displayName;
        }

        public object ParseString(ArgumentInfo argumentInfo, string value)
        {
            return _parseValueDelegate(argumentInfo, value);
        }
    }
}