using System;
using CommandDotNet.Builders;

namespace CommandDotNet.Extensions
{
    public static class ArgumentExtensions
    {
        public static bool IsHelpOption(this IArgument argument) => argument.Name == Constants.HelpOptionName;

        public static bool IsAppVersionOption(this IArgument argument) =>
            argument.Name == VersionMiddleware.VersionOptionName && argument.Parent.IsRootCommand();

        internal static void SwitchAct(
            this IArgument argument,
            Action<Operand> operandAction,
            Action<Option> optionAction)
        {
            switch (argument)
            {
                case Operand operand:
                    operandAction(operand);
                    break;
                case Option option:
                    optionAction(option);
                    break;
                default:
                    throw new ArgumentException(BuildExMessage(argument));
            }
        }

        internal static TResult SwitchFunc<TResult>(
            this IArgument argument,
            Func<Operand, TResult> operandFunc,
            Func<Option, TResult> optionFunc)
        {
            switch (argument)
            {
                case Operand operand:
                    return operandFunc(operand);
                case Option option:
                    return optionFunc(option);
                default:
                    throw new ArgumentException(BuildExMessage(argument));
            }
        }

        private static string BuildExMessage(IArgument argument)
        {
            return $"argument type must be `{typeof(Operand)}` or `{typeof(Option)}` but was `{argument.GetType()}`. " +
                   $"If `{argument.GetType()}` was created for extensibility, " +
                   $"consider using {nameof(IArgument)}.{nameof(IArgument.Services)} to store service classes instead.";
        }
    }
}