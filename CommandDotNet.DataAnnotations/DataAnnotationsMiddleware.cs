using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Execution;

namespace CommandDotNet.DataAnnotations
{
    public static class DataAnnotationsMiddleware
    {
        public static AppRunner UseDataAnnotationValidations(this AppRunner appRunner, bool showHelpOnError = false)
        {
            return appRunner.Configure(c =>
            {
                c.UseMiddleware(DataAnnotationsValidation, MiddlewareSteps.DataAnnotations);
                c.Services.Add(new Config(showHelpOnError));
            });
        }

        private class Config
        {
            public bool ShowHelpOnError { get; }

            public Config(bool showHelpOnError)
            {
                ShowHelpOnError = showHelpOnError;
            }
        }

        private static Task<int> DataAnnotationsValidation(CommandContext ctx, ExecutionDelegate next)
        {
            var failureResults = ctx.InvocationPipeline.All
                .SelectMany(ValidateStep)
                .ToList();

            if (failureResults.Any())
            {
                var console = ctx.Console;
                failureResults.ForEach(f =>
                {
                    foreach (var error in f.Errors)
                    {
                        console.Error.WriteLine(error);
                    }
                });

                ctx.ShowHelpOnExit = ctx.AppConfig.Services.GetOrThrow<Config>().ShowHelpOnError;

                if (ctx.ShowHelpOnExit)
                {
                    console.Error.WriteLine();
                }

                return ExitCodes.ValidationError;
            }
            
            return next(ctx);
        }


        private static IEnumerable<ArgumentErrors> ValidateStep(InvocationStep step)
        {
            return step.Invocation.Arguments
                .Select(GetArgumentErrors)
                .Where(a => a != null);
        }

        private static ArgumentErrors GetArgumentErrors(IArgument argument)
        {
            List<string> errors = null;
            
            var validationAttributes = argument.CustomAttributes
                .GetCustomAttributes(true)
                .OfType<ValidationAttribute>()
                .ToList();
            
            foreach (var validationAttribute in validationAttributes)
            {
                var isValid = validationAttribute.IsValid(argument.Value);
                if (!isValid)
                {
                    // the user expects the name to map to an argument, not a field.
                    // update the terminology. This is naive and will need to change
                    // when we handle localizaton
                    var message = validationAttribute.FormatErrorMessage($"'{argument.Name}'")
                        .Replace($"The '{argument.Name}' field", $"'{argument.Name}'")
                        .Replace($"The field '{argument.Name}'", $"'{argument.Name}'");
                    (errors ??= new List<string>()).Add(message);
                }
            }
            
            return errors == null
                ? null
                : new ArgumentErrors(errors);
        }

        private class ArgumentErrors
        {
            public readonly ICollection<string> Errors;
            public ArgumentErrors(ICollection<string> errors)
            {
                Errors = errors;
            }
        }
    }
}