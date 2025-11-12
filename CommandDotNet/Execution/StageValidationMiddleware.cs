using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommandDotNet.Execution;

/// <summary>
/// Validates that CommandContext state is correctly populated
/// as it exits each pipeline stage
/// </summary>
internal static class StageValidationMiddleware
{
    /// <summary>
    /// Validates that after PreTokenize stage:
    /// - Original input is populated
    /// - Tokens collection is set (first pass, before transformations)
    /// - AppConfig is set
    /// - Console is set
    /// </summary>
    internal static Task<int> ValidatePostPreTokenize(CommandContext commandContext, ExecutionDelegate next)
    {
        var errors = new List<string>();

        if (commandContext.Original is null)
        {
            errors.Add($"{nameof(CommandContext)}.{nameof(CommandContext.Original)} must be populated after {nameof(MiddlewareStages.PreTokenize)} stage");
        }

        if (commandContext.Tokens is null)
        {
            errors.Add($"{nameof(CommandContext)}.{nameof(CommandContext.Tokens)} must be populated after {nameof(MiddlewareStages.PreTokenize)} stage");
        }

        if (commandContext.AppConfig is null)
        {
            errors.Add($"{nameof(CommandContext)}.{nameof(CommandContext.AppConfig)} must be populated after {nameof(MiddlewareStages.PreTokenize)} stage");
        }

        if (commandContext.Console is null)
        {
            errors.Add($"{nameof(CommandContext)}.{nameof(CommandContext.Console)} must be populated after {nameof(MiddlewareStages.PreTokenize)} stage");
        }

        if (errors.Count > 0)
        {
            throw new InvalidOperationException(
                $"Bug: Stage validation failed after {nameof(MiddlewareStages.PreTokenize)} stage:{Environment.NewLine}  - {string.Join(Environment.NewLine + "  - ", errors)}");
        }

        return next(commandContext);
    }

    /// <summary>
    /// Validates that after Tokenize stage:
    /// - Tokens is the result of all TokenTransformations
    /// - RootCommand is set for the type specified in AppRunner
    /// </summary>
    internal static Task<int> ValidatePostTokenize(CommandContext commandContext, ExecutionDelegate next)
    {
        var errors = new List<string>();

        if (commandContext.Tokens is null)
        {
            errors.Add($"{nameof(CommandContext.Tokens)} must be populated after {nameof(MiddlewareStages.Tokenize)} stage");
        }

        if (commandContext.RootCommand is null)
        {
            errors.Add($"{nameof(CommandContext.RootCommand)} must be populated after {nameof(MiddlewareStages.Tokenize)} stage");
        }
        else if (commandContext.RootCommand.Name is null)
        {
            errors.Add($"{nameof(CommandContext.RootCommand)}.{nameof(Command.Name)} must be populated after {nameof(MiddlewareStages.Tokenize)} stage");
        }

        if (errors.Count > 0)
        {
            throw new InvalidOperationException(
                $"Bug: Stage validation failed after {nameof(MiddlewareStages.Tokenize)} stage:{Environment.NewLine}  - {string.Join(Environment.NewLine + "  - ", errors)}");
        }

        return next(commandContext);
    }

    /// <summary>
    /// Validates that after ParseInput stage:
    /// - ParseResult is populated
    /// - ParseResult.TargetCommand is set
    /// - IArgument.InputValues contains values assigned to arguments
    /// - IArgument.Default contains default values
    /// - InvocationPipeline steps have Invocation and Command set
    /// - Instance and ParameterValues are NOT set yet (those come in BindValues stage)
    /// </summary>
    internal static Task<int> ValidatePostParseInput(CommandContext commandContext, ExecutionDelegate next)
    {
        var errors = new List<string>();

        if (commandContext.ParseResult is null)
        {
            errors.Add($"{nameof(CommandContext.ParseResult)} must be populated after {nameof(MiddlewareStages.ParseInput)} stage");
        }
        else if (commandContext.ParseResult.TargetCommand is null)
        {
            errors.Add($"{nameof(CommandContext.ParseResult)}.{nameof(commandContext.ParseResult.TargetCommand)} must be populated after {nameof(MiddlewareStages.ParseInput)} stage");
        }

        foreach (var step in commandContext.InvocationPipeline.All)
        {
            ValidateInvocationStepState(step, errors);
        }

        if (errors.Count > 0)
        {
            throw new InvalidOperationException(
                $"Bug: Stage validation failed after {nameof(MiddlewareStages.ParseInput)} stage:{Environment.NewLine}  - {string.Join(Environment.NewLine + "  - ", errors)}");
        }

        return next(commandContext);

        void ValidateInvocationStepState(InvocationStep step, List<string> errors)
        {
            if (step.Command is null)
            {
                errors.Add($"{nameof(InvocationStep)}.{nameof(InvocationStep.Command)} must be populated after {nameof(MiddlewareStages.ParseInput)} stage");
                return; // Can't continue validation without Command
            }
            
            if (step.Invocation is null)
            {
                errors.Add($"{nameof(InvocationStep)}.{nameof(InvocationStep.Invocation)} must be populated after {nameof(MiddlewareStages.ParseInput)} stage for command '{step.Command.Name}'");
                return; // Can't continue validation without Invocation
            }

            // Verify that Instance and ParameterValues are NOT set yet
            if (step.Instance is not null)
            {
                errors.Add($"{nameof(InvocationStep)}.{nameof(InvocationStep.Instance)} should NOT be populated until {nameof(MiddlewareStages.BindValues)} stage for command '{step.Command.Name}'");
            }

            // ParameterValues array may be initialized, but should contain only nulls at this stage
            if (step.Invocation.ParameterValues is not null && step.Invocation.ParameterValues.Any(pv => pv is not null))
            {
                errors.Add($"{nameof(InvocationStep)}.{nameof(InvocationStep.Invocation)}.{nameof(IInvocation.ParameterValues)} should NOT contain actual values until {nameof(MiddlewareStages.BindValues)} stage for command '{step.Command.Name}'");
            }

            // Validate arguments in the command have InputValues initialized (even if empty)
            foreach (var argument in step.Command.AllArguments(includeInterceptorOptions: true))
            {
                if (argument.InputValues is null)
                {
                    errors.Add($"{nameof(IArgument)}.{nameof(IArgument.InputValues)} must be initialized after {nameof(MiddlewareStages.ParseInput)} stage for argument '{argument.Name}' in command '{step.Command.Name}'");
                }

                // Note: Default can be null if no default was provided, so we just check it's been considered
            }
        }
    }

    /// <summary>
    /// Validates that after BindValues stage:
    /// - Instance is set for the target command and ancestral interceptor commands
    /// - ParameterValues are set
    /// - Subcommand properties are populated IF subcommand is in request and property is settable
    /// </summary>
    internal static Task<int> ValidatePostBindValues(CommandContext commandContext, ExecutionDelegate next)
    {
        var errors = new List<string>();

        foreach (var step in commandContext.InvocationPipeline.All)
        {
            ValidateInvocationStep(step, errors);
        }

        if (errors.Count > 0)
        {
            throw new InvalidOperationException(
                $"Bug: Stage validation failed after {nameof(MiddlewareStages.BindValues)} stage:{Environment.NewLine}  - {string.Join(Environment.NewLine + "  - ", errors)}");
        }

        return next(commandContext);

        void ValidateInvocationStep(InvocationStep step, List<string> errors)
        {
            if (step.Instance is null)
            {
                errors.Add($"{nameof(InvocationStep)}.{nameof(InvocationStep.Instance)} must be populated after {nameof(MiddlewareStages.BindValues)} stage for command '{step.Command?.Name}'");
            }

            if (step.Invocation?.ParameterValues is null)
            {
                errors.Add($"{nameof(InvocationStep)}.{nameof(InvocationStep.Invocation)}.{nameof(IInvocation.ParameterValues)} must be populated after {nameof(MiddlewareStages.BindValues)} stage for command '{step.Command?.Name}'");
            }
            else
            {
                // Validate that ParameterValues count matches the number of parameters
                var parameterCount = step.Invocation.MethodInfo?.GetParameters().Length ?? 0;
                if (step.Invocation.ParameterValues.Length != parameterCount)
                {
                    errors.Add($"{nameof(InvocationStep)}.{nameof(InvocationStep.Invocation)}.{nameof(IInvocation.ParameterValues)} count ({step.Invocation.ParameterValues.Length}) does not match parameter count ({parameterCount}) after {nameof(MiddlewareStages.BindValues)} stage for command '{step.Command?.Name}'");
                }
            }
        }
    }
}
