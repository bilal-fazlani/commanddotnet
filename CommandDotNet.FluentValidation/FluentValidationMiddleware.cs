using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Execution;

namespace CommandDotNet.FluentValidation
{
    public static class FluentValidationMiddleware
    {
        /// <summary>Enables FluentValidation for <see cref="IArgumentModel"/>s</summary>
        /// <param name="appRunner">the <see cref="AppRunner"/></param>
        /// <param name="showHelpOnError">when true, help will be display for the target command after the validation errors</param>
        public static AppRunner UseFluentValidation(this AppRunner appRunner, bool showHelpOnError = false)
        {
            return appRunner.Configure(c =>
            {
                c.Services.Add(new Config(showHelpOnError));
                c.UseMiddleware(Middleware, MiddlewareStages.PostBindValuesPreInvoke);
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

        private static Task<int> Middleware(CommandContext ctx, ExecutionDelegate next)
        {
            var modelValidator = new ModelValidator(ctx.AppConfig.DependencyResolver);

            var paramValues = ctx.InvocationPipeline
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
                    var console = ctx.Console;
                    failureResults.ForEach(f =>
                    {
                        console.Error.WriteLine($"'{f.model.GetType().Name}' is invalid");
                        foreach (var error in f.result.Errors)
                        {
                            console.Error.WriteLine($"  {error.ErrorMessage}");
                        }
                    });
                    console.Error.WriteLine();

                    ctx.ShowHelpOnExit = ctx.AppConfig.Services.Get<Config>().ShowHelpOnError;

                    return Task.FromResult(2);
                }
            }
            catch (InvalidValidatorException e)
            {
                ctx.Console.Error.WriteLine(e.ToString());
                return Task.FromResult(1);
            }

            return next(ctx);
        }
    }
}