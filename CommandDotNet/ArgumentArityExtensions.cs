namespace CommandDotNet
{
    public static class ArgumentArityExtensions
    {
        /// <summary><see cref="IArgumentArity.Maximum"/> gt; 1</summary>
        public static bool AllowsMany(this IArgumentArity arity) => arity.Maximum > 1;

        /// <summary><see cref="IArgumentArity.Minimum"/> gt; 0</summary>
        public static bool RequiresAtLeastOne(this IArgumentArity arity) => arity.Minimum > 0;

        /// <summary><see cref="IArgumentArity.Minimum"/> == 1 == <see cref="IArgumentArity.Maximum"/></summary>
        public static bool RequiresExactlyOne(this IArgumentArity arity) => arity.Minimum == 1 && arity.Maximum == 1;

        /// <summary><see cref="IArgumentArity.Maximum"/> == 0</summary>
        public static bool AllowsNone(this IArgumentArity arity) => arity.Maximum == 0;
    }
}