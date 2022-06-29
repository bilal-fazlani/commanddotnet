using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CommandDotNet.Builders.ArgumentDefaults;
using CommandDotNet.Extensions;

namespace CommandDotNet.Execution
{
    internal static class ValidateArityMiddleware
    {
        internal static AppRunner UseArityValidation(this AppRunner appRunner) =>
            appRunner.Configure(c =>
                {
                    c.UseMiddleware(ValidateArity, MiddlewareSteps.ValidateArity);
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
            // TODO: create a PipedInputStream class for easy checking and ability to peek to validate at least one value

            // Piped input stream cannot define arity limits so this cannot be a stream now.
            var value = argument.Value;

            if (value is null)
            {
                return arity.RequiresAtLeastOne() 
                    ? Resources.A.Arity_is_required(argument.Name) 
                    : null;
            }

            var valueCount = value is ICollection coll ? coll.Count : 1;

            if (valueCount == 0)
            {
                return arity.RequiresAtLeastOne()
                    ? Resources.A.Arity_is_required(argument.Name)
                    : null;
            }

            if (argument is Option option && option.IsFlag && valueCount == 1)
            {
                // flags are assigned true or false.
                return null;
            }

            // non-nullable value type properties
            var valueType = value.GetType();
            var wasInput = argument.InputValues.Any();
            if (!wasInput && !valueType.IsClass && value.IsDefaultFor(valueType))
            {
                return arity.RequiresAtLeastOne()
                    ? Resources.A.Arity_is_required(argument.Name)
                    : null;
            }

            if (valueCount < arity.Minimum)
            {
                return Resources.A.Arity_min_not_reached(argument.Name, arity.Minimum.ToString(), valueCount.ToString());
            }

            if (valueCount > arity.Maximum)
            {
                return Resources.A.Arity_max_exceeded(argument.Name, arity.Maximum.ToString(), valueCount.ToString());
            }

            return null;
        }
    }
}