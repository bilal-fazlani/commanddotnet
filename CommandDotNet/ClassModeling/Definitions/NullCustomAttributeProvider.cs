using System;
using System.Reflection;

namespace CommandDotNet.ClassModeling.Definitions
{
    public class NullCustomAttributeProvider : ICustomAttributeProvider
    {
        internal static readonly NullCustomAttributeProvider Instance = new NullCustomAttributeProvider();

        private NullCustomAttributeProvider()
        {
        }

        public object[] GetCustomAttributes(bool inherit)
        {
            return new object[0];
        }

        public object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return new object[0];
        }

        public bool IsDefined(Type attributeType, bool inherit)
        {
            return false;
        }
    }
}