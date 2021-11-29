using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CommandDotNet.Extensions;

namespace CommandDotNet.Execution
{
    internal static class ValidateArityMiddleware
    {
        internal static AppRunner UseArityValidation(this AppRunner appRunner) =>
            appRunner.Configure(c =>
                {
                    if (NullabilityInfoContext.IsSupported)
                    {
                        c.UseMiddleware(ValidateArity, MiddlewareSteps.ValidateArity);
                    }
                });

        private static Task<int> ValidateArity(CommandContext ctx, ExecutionDelegate next)
        {
            if (!ctx.AppConfig.AppSettings.Arguments.SkipArityValidation)
            {
                var errors = ctx.InvocationPipeline.All
                    .SelectMany(i => i.Invocation.Arguments)
                    .Select(Validate)
                    .Where(v => v is not null)
                    .ToCsv(Environment.NewLine);

                if (!errors.IsNullOrWhitespace())
                {
                    ctx.Console.Error.WriteLine(errors);
                    return ExitCodes.ValidationError;
                }
            }

            return next(ctx);
        }

        private static string? Validate(IArgument argument)
        {
            var arity = argument.Arity;
            if (arity.AllowsNone() && arity.AllowsUnlimited())
            {
                // exit early
                return null;
            }
            
            // TODO: if min/max arity defined, require ICollection type because we cannot validate for a stream
            // TODO: localize these errors
            // TODO: create a PipedInputStream class for easy checking and ability to peek to validate at least one value

            // Piped input stream cannot define arity limits so this cannot be a stream now.
            var value = argument.Value;

            if (value is null)
            {
                return arity.RequiresAtLeastOne() 
                    ? $"{argument.Name} is required" 
                    : null;
            }

            var valueCount = value is ICollection coll ? coll.Count : 1;

            if (valueCount == 0)
            {
                return arity.RequiresAtLeastOne()
                    ? $"{argument.Name} is required"
                    : null;
            }

            if (argument is Option option && option.IsFlag && valueCount == 1)
            {
                // flags are assigned true or false.
                return null;
            }

            if (arity.RequiresExactlyOne())
            {
                return valueCount > 1
                    ? $"{argument.Name} can only have {arity.Minimum} value"
                    : null;
            }

            if (valueCount < arity.Minimum)
            {
                return $"{argument.Name} requires at least {arity.Minimum} values but {valueCount} were provided.";
            }

            if (valueCount > arity.Maximum)
            {
                return $"{argument.Name} can have no more than {arity.Maximum} values but {valueCount} were provided.";
            }

            return null;
        }
    }
}