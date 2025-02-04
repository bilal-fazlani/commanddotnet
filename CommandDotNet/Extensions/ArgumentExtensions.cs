﻿using System;
using JetBrains.Annotations;

namespace CommandDotNet.Extensions;

[PublicAPI]
public static class ArgumentExtensions
{
    /// <summary>Returns true if value was input from the shell</summary>
    public static bool HasValueFromInput(this IArgument argument) 
        => !argument.InputValues.IsEmpty();

    /// <summary>Returns true if a default value is available</summary>
    public static bool HasValueFromDefault(this IArgument argument)
        => !argument.Default?.Value.IsNullValue() ?? false;

    /// <summary>Returns true if value was input from the shell or a default value is available</summary>
    public static bool HasValueFromInputOrDefault(this IArgument argument)
        => argument.HasValueFromInput() || argument.HasValueFromDefault();

    /// <summary>Returns true if argument name is <see cref="Resources.Command_help"/></summary>
    public static bool IsHelpOption(this IArgument argument) => 
        argument.ThrowIfNull().Name == Resources.A.Command_help;

    /// <summary>Returns true if argument name is <see cref="Resources.Command_version"/> and is on the root command.</summary>
    public static bool IsAppVersionOption(this IArgument argument) => 
        argument.ThrowIfNull().Name == Resources.A.Command_version && argument.Parent!.IsRootCommand();

    public static bool IsObscured(this IArgument argument) =>
        argument.TypeInfo.UnderlyingType == typeof(Password);

    /// <summary>
    /// For the given <see cref="argument"/>,
    /// execute <see cref="operandAction"/> when <see cref="Operand"/>
    /// and <see cref="optionAction"/> when <see cref="Option"/>
    /// </summary>
    public static void SwitchAct(
        this IArgument argument,
        Action<Operand>? operandAction = null,
        Action<Option>? optionAction = null)
    {
        switch (argument)
        {
            case null:
                throw new ArgumentNullException(nameof(argument));
            case Operand operand:
                operandAction?.Invoke(operand);
                break;
            case Option option:
                optionAction?.Invoke(option);
                break;
            default:
                throw new ArgumentException(BuildExMessage(argument));
        }
    }

    /// <summary>
    /// For the given <see cref="argument"/>,
    /// execute <see cref="operandFunc"/> when <see cref="Operand"/>
    /// and <see cref="optionFunc"/> when <see cref="Option"/>
    /// </summary>
    public static TResult? SwitchFunc<TResult>(
        this IArgument argument,
        Func<Operand, TResult>? operandFunc = null,
        Func<Option, TResult>? optionFunc = null)
        where TResult : class =>
        argument switch
        {
            null => throw new ArgumentNullException(nameof(argument)),
            Operand operand => operandFunc?.Invoke(operand),
            Option option => optionFunc?.Invoke(option),
            _ => throw new ArgumentException(BuildExMessage(argument))
        };

    /// <summary>
    /// For the given <see cref="argument"/>,
    /// execute <see cref="operandFunc"/> when <see cref="Operand"/>
    /// and <see cref="optionFunc"/> when <see cref="Option"/>
    /// </summary>
    public static TResult SwitchFuncStruct<TResult>(
        this IArgument argument,
        Func<Operand, TResult> operandFunc,
        Func<Option, TResult> optionFunc)
        where TResult : struct =>
        argument switch
        {
            null => throw new ArgumentNullException(nameof(argument)),
            Operand operand => operandFunc.Invoke(operand),
            Option option => optionFunc.Invoke(option),
            _ => throw new ArgumentException(BuildExMessage(argument))
        };

    private static string BuildExMessage(IArgument argument) =>
        $"argument type must be `{typeof(Operand)}` or `{typeof(Option)}` but was `{argument.GetType()}`. " +
        $"If `{argument.GetType()}` was created for extensibility, " +
        $"consider using {nameof(IArgument)}.{nameof(IArgument.Services)} to store service classes instead.";
}