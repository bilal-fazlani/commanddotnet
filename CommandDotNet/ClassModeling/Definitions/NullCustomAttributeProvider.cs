using System;
using System.Reflection;

namespace CommandDotNet.ClassModeling.Definitions
{
    public class NullCustomAttributeProvider : ICustomAttributeProvider
    {
        internal static readonly NullCustomAttributeProvider Instance = new();

        private NullCustomAttributeProvider()
        {
        }

        public object[] GetCustomAttributes(bool inherit)
        {
            return Array.Empty<object>();
        }

        public object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return Array.Empty<object>();
        }

        public bool IsDefined(Type attributeType, bool inherit)
        {
            return false;
        }
    }
}