namespace CommandDotNet
{
    public static class ArgumentArityExtensions
    {
        /// <summary><see cref="IArgumentArity.MaximumNumberOfValues"/> gt; 1</summary>
        public static bool AllowsMany(this IArgumentArity arity) => arity.MaximumNumberOfValues > 1;

        /// <summary><see cref="IArgumentArity.MinimumNumberOfValues"/> gt; 0</summary>
        public static bool RequiresAtLeastOne(this IArgumentArity arity) => arity.MinimumNumberOfValues > 0;

        /// <summary><see cref="IArgumentArity.MinimumNumberOfValues"/> == 1 == <see cref="IArgumentArity.MaximumNumberOfValues"/></summary>
        public static bool RequiresExactlyOne(this IArgumentArity arity) => arity.MinimumNumberOfValues == 1 && arity.MaximumNumberOfValues == 1;

        /// <summary><see cref="IArgumentArity.MaximumNumberOfValues"/> == 0</summary>
        public static bool AllowsNone(this IArgumentArity arity) => arity.MaximumNumberOfValues == 0;
    }
}