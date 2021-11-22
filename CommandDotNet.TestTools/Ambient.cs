using System;
using System.Threading;

namespace CommandDotNet.TestTools
{
    internal static class Ambient<T>
    {
        private static readonly AsyncLocal<T?> Internal = new();

        public static T? Instance
        {
            get => Internal.Value;
            set => Internal.Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public static T InstanceOrThrow => Internal.Value 
            ?? throw new InvalidOperationException($"Ambient<{typeof(T).Name}> has not been set for the current test");

        public static void Set(T instance) => Internal.Value = instance ?? throw new ArgumentNullException(nameof(instance));

        public static void SetOrClear(T? instance) => Internal.Value = instance;

        public static void Clear() => Internal.Value = default;
    }
}