using System;
using CommandDotNet.Extensions;

namespace CommandDotNet
{
    public class ArgumentArity : IArgumentArity
    {
        public const byte MaximumValue = byte.MaxValue;

        // copied and modified from System.CommandLine
        // https://github.com/dotnet/command-line-api/blob/master/src/System.CommandLine/ArgumentArity.cs

        public ArgumentArity(int minimumNumberOfValues, int maximumNumberOfValues)
        {
            if (minimumNumberOfValues < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minimumNumberOfValues));
            }

            if (maximumNumberOfValues < minimumNumberOfValues)
            {
                throw new ArgumentException($"{nameof(maximumNumberOfValues)} must be greater than or equal to {nameof(minimumNumberOfValues)}");
            }

            MinimumNumberOfValues = minimumNumberOfValues;
            MaximumNumberOfValues = maximumNumberOfValues;
        }

        public int MinimumNumberOfValues { get; }

        public int MaximumNumberOfValues { get; }

        public static IArgumentArity Zero => new ArgumentArity(0, 0);

        public static IArgumentArity ZeroOrOne => new ArgumentArity(0, 1);

        public static IArgumentArity ExactlyOne => new ArgumentArity(1, 1);

        public static IArgumentArity ZeroOrMore => new ArgumentArity(0, MaximumValue);

        public static IArgumentArity OneOrMore => new ArgumentArity(1, MaximumValue);

        public static IArgumentArity Default(Type type, BooleanMode booleanMode)
        {
            if (type != typeof(string) && type.IsEnumerable())
            {
                return ZeroOrMore;
            }

            if ((type == typeof(bool) || type == typeof(bool?)) && booleanMode == BooleanMode.Implicit)
            {
                return Zero;
            }

            return ZeroOrOne;
        }
    }
}