using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Execution;

namespace CommandDotNet.FluentValidation
{
    public static class FluentValidationMiddleware
    {
        /// <summary>Enables FluentValidation for <see cref="IArgumentModel"/>s</summary>
        public static AppRunner UseFluentValidation(this AppRunner appRunner)
        {
            return appRunner.Configure(c =>
                c.UseMiddleware(Middleware, MiddlewareStages.PostBindValuesPreInvoke));
        }

        private static Task<int> Middleware(CommandContext commandContext, ExecutionDelegate next)
        {
            var modelValidator = new ModelValidator(commandContext.AppConfig.DependencyResolver);

            var paramValues = commandContext.InvocationPipeline
                .All
                .SelectMany(i => i.Invocation.ParameterValues.OfType<IArgumentModel>());

            try
            {
                var failureResults = paramValues
                    .Select(model => new { model, result = modelValidator.ValidateModel(model) })
                    .Where(v => v.result != null && !v.result.IsValid)
                    .ToList();

                if (failureResults.Any())
                {
                    var console = commandContext.Console;
                    failureResults.ForEach(f =>
                    {
                        console.Error.WriteLine($"'{f.model.GetType().Name}' is invalid");
                        foreach (var error in f.result.Errors)
                        {
                            console.Error.WriteLine($"  {error.ErrorMessage}");
                        }
                    });
                    console.Error.WriteLine();

                    return Task.FromResult(2);
                }
            }
            catch (InvalidValidatorException e)
            {
                commandContext.Console.Error.WriteLine(e.ToString());
                return Task.FromResult(1);
            }

            return next(commandContext);
        }
    }
}