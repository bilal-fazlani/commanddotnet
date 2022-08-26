using System;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Execution;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;

namespace CommandDotNet.FluentValidation
{
    public static class FluentValidationMiddleware
    {
        private static readonly DefaultValidatorSelector Selector = new();

        /// <summary>Enables FluentValidation for <see cref="IArgumentModel"/>s</summary>
        /// <param name="appRunner">the <see cref="AppRunner"/></param>
        /// <param name="showHelpOnError">when true, help will be display for the target command after the validation errors</param>
        /// <param name="validatorFactory">
        /// Override how validators are resolved.
        /// By default, checks the <see cref="CommandContext"/>.<see cref="CommandContext.DependencyResolver"/> for <see cref="IValidator{TModel}"/>
        /// If not found, the assembly of the model is scanned (only once).
        /// </param>
        /// <param name="resourcesOverride">
        /// use with <see cref="ResourcesProxy"/> to localize output this plugin.
        /// This does not cover FluentValidation validations.
        /// </param>
        public static AppRunner UseFluentValidation(this AppRunner appRunner, bool showHelpOnError = false,
            Func<IArgumentModel, IValidator?>? validatorFactory = null,
            Resources? resourcesOverride = null)
        {
            return appRunner.Configure(c =>
            {
                var localizationAppSettings = appRunner.AppSettings.Localization;
                if (resourcesOverride != null)
                {
                    Resources.A = resourcesOverride;
                }
                else if (localizationAppSettings.Localize != null)
                {
                    Resources.A = new ResourcesProxy(
                        localizationAppSettings.Localize, 
                        localizationAppSettings.UseMemberNamesAsKeys);
                }
                c.UseMiddleware(FluentValidationForModels, MiddlewareSteps.FluentValidation);
                c.Services.Add(new Config(showHelpOnError, validatorFactory));
            });
        }

        private class Config
        {
            internal readonly bool ShowHelpOnError;
            internal readonly Func<IArgumentModel, IValidator?>? ValidatorFactory;

            public Config(bool showHelpOnError, Func<IArgumentModel, IValidator?>? validatorFactory)
            {
                ShowHelpOnError = showHelpOnError;
                ValidatorFactory = validatorFactory;
            }
        }

        private static Task<int> FluentValidationForModels(CommandContext ctx, ExecutionDelegate next)
        {
            var config = ctx.Services.GetOrThrow<Config>();

            var validatorFactory = config.ValidatorFactory ?? new ValidatorFactory(ctx).Resolve;
            ValidationResult? Validate(IArgumentModel argumentModel)
            {
                return validatorFactory(argumentModel)?
                    .Validate(new ValidationContext<object>(argumentModel, new PropertyChain(), Selector));
            }

            try
            {
                var paramValues = ctx.InvocationPipeline
                    .All
                    .SelectMany(i => i.Invocation.FlattenedArgumentModels);

                var failureResults = paramValues
                    .Select(model => new { model, result = Validate(model) })
                    .Where(v => v.result is { IsValid: false })
                    .ToList();

                if (failureResults.Any())
                {
                    var console = ctx.Console;
                    failureResults.ForEach(f =>
                    {
                        console.Error.WriteLine(Resources.A.Error_Argument_model_is_invalid(f.model.GetType().Name));
                        foreach (var error in f.result!.Errors)
                        {
                            console.Error.WriteLine($"  {error.ErrorMessage}");
                        }
                    });

                    ctx.ShowHelpOnExit = ctx.AppConfig.Services.GetOrThrow<Config>().ShowHelpOnError;

                    if (ctx.ShowHelpOnExit)
                    {
                        console.Error.WriteLine();
                    }

                    return ExitCodes.ValidationError;
                }
            }
            catch (InvalidValidatorException e)
            {
                ctx.Console.Error.WriteLine(e.ToString());
                return ExitCodes.Error;
            }

            return next(ctx);
        }
    }
}