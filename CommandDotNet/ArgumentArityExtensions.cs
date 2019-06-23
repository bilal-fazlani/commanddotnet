namespace CommandDotNet
{
    public static class ArgumentArityExtensions
    {
        public static bool AllowsZeroOrMore(this IArgumentArity arity) => arity.MaximumNumberOfValues > 1;
        public static bool AllowsZeroOrOne(this IArgumentArity arity) => arity.MaximumNumberOfValues == 1;
        public static bool AllowsNone(this IArgumentArity arity) => arity.MaximumNumberOfValues == 0;
    }
}