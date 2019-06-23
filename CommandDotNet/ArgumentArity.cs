using System;
using CommandDotNet.Extensions;
using CommandDotNet.MicrosoftCommandLineUtils;

namespace CommandDotNet
{
    public class ArgumentArity : IArgumentArity
    {
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

        public static IArgumentArity ZeroOrMore => new ArgumentArity(0, byte.MaxValue);

        public static IArgumentArity OneOrMore => new ArgumentArity(1, byte.MaxValue);

        public static IArgumentArity Default(Type type, BooleanMode booleanMode)
        {
            if (type != typeof(string) && type.IsCollection())
            {
                return ArgumentArity.ZeroOrMore;
            }

            if ((type == typeof(bool) || type == typeof(bool?)) && booleanMode == BooleanMode.Implicit)
            {
                return ArgumentArity.Zero;
            }

            return ZeroOrOne;
        }
        
        public static CommandOptionType ToCommandOptionType(IArgumentArity arity)
        {
            return arity.AllowsZeroOrMore()
                ? CommandOptionType.MultipleValue
                : arity.AllowsZeroOrOne()
                    ? CommandOptionType.SingleValue
                    : CommandOptionType.NoValue;
        }

        public static IArgumentArity FromCommandOptionType(CommandOptionType optionType)
        {
            switch (optionType)
            {
                case CommandOptionType.MultipleValue:
                    return ZeroOrMore;
                case CommandOptionType.SingleValue:
                    return ZeroOrOne;
                case CommandOptionType.NoValue:
                    return Zero;
                default:
                    throw new ArgumentOutOfRangeException(nameof(optionType), optionType, null);
            }
        }
    }
}