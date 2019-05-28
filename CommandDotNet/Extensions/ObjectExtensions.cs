using System;

namespace CommandDotNet.Extensions
{
    internal static class ObjectExtensions
    {
        internal static bool IsNullValue(this object obj)
        {
            return obj == null || obj == DBNull.Value;
        }
    }
}