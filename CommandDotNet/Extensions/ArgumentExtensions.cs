using System;

namespace CommandDotNet.Extensions
{
    internal static class ArgumentExtensions
    {
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
            Func<Operand, TResult> operandAction,
            Func<Option, TResult> optionAction)
        {
            switch (argument)
            {
                case Operand operand:
                    return operandAction(operand);
                case Option option:
                    return optionAction(option);
                default:
                    throw new ArgumentException(BuildExMessage(argument));
            }
        }

        private static string BuildExMessage(IArgument argument)
        {
            return $"argument type must be `{typeof(Operand)}` or `{typeof(Option)}` but was `{argument.GetType()}`. " +
                   $"If `{argument.GetType()}` was created for extensibility, " +
                   $"consider using {nameof(IArgument)}.{nameof(IArgument.ContextData)} to store service classes instead.";
        }
    }
}