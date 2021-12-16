namespace CommandDotNet
{
    public static class ArgumentArityExtensions
    {
        // begin-snippet: arity-extensions
        /// <summary><see cref="IArgumentArity.Maximum"/> &gt; 1</summary>
        public static bool AllowsMany(this IArgumentArity arity) => arity.Maximum > 1;

        /// <summary><see cref="IArgumentArity.Maximum"/> &gt;= 1</summary>
        public static bool AllowsOneOrMore(this IArgumentArity arity) => arity.Maximum >= 1;

        /// <summary><see cref="IArgumentArity.Minimum"/> &gt; 0</summary>
        public static bool RequiresAtLeastOne(this IArgumentArity arity) => arity.Minimum > 0;

        /// <summary><see cref="IArgumentArity.Minimum"/> == 1 == <see cref="IArgumentArity.Maximum"/></summary>
        public static bool RequiresExactlyOne(this IArgumentArity arity) => arity.Minimum == 1 && arity.Maximum == 1;

        /// <summary>
        /// <see cref="IArgumentArity.Maximum"/> == 0.
        /// e.g. <see cref="ArgumentArity.Zero"/>
        /// </summary>
        public static bool RequiresNone(this IArgumentArity arity) => arity.Maximum == 0;

        /// <summary>
        /// <see cref="IArgumentArity.Minimum"/> == 0.
        /// e.g. <see cref="ArgumentArity.Zero"/>, <see cref="ArgumentArity.ZeroOrOne"/>, <see cref="ArgumentArity.ZeroOrMore"/>
        /// </summary>
        public static bool AllowsNone(this IArgumentArity arity) => arity.Minimum == 0;

        /// <summary>
        /// <see cref="IArgumentArity.Maximum"/> == <see cref="ArgumentArity.Unlimited"/> (<see cref="int.MaxValue"/>).
        /// e.g. <see cref="ArgumentArity.ZeroOrMore"/>, <see cref="ArgumentArity.OneOrMore"/>
        /// </summary>
        public static bool AllowsUnlimited(this IArgumentArity arity) => arity.Maximum == ArgumentArity.Unlimited;
        // end-snippet
    }
}