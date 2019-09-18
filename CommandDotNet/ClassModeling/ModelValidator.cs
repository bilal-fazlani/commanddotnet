using System;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Builders;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;
using FluentValidation;
using FluentValidation.Attributes;
using FluentValidation.Results;

namespace CommandDotNet.ClassModeling
{
    public class ModelValidator
    {
        // TODO: move FluentValidation into a separate repo & nuget package?
        //       there are other ways to do validation that could also
        //       be applied to parameters
        private readonly IDependencyResolver _dependencyResolver;

        public ModelValidator(IDependencyResolver dependencyResolver)
        {
            _dependencyResolver = dependencyResolver;
        }

        internal static Task<int> FluentValidationMiddleware(CommandContext commandContext, ExecutionDelegate next)
        {
            var modelValidator = new ModelValidator(commandContext.AppConfig.DependencyResolver);

            var paramValues = commandContext.InvocationPipeline
                .All
                .SelectMany(i => i.Invocation.ParameterValues.OfType<IArgumentModel>());
            
            var failureResults = paramValues
                .Select(model => new { model, result = modelValidator.ValidateModel(model) })
                .Where(v => v.result != null && !v.result.IsValid)
                .ToList();

            if (failureResults.Any())
            {
                var console = commandContext.Console;
                failureResults.ForEach(f =>
                {
                    console.Out.WriteLine($"'{f.model.GetType().Name}' is invalid");
                    foreach (var error in f.result.Errors)
                    {
                        console.Out.WriteLine($"  {error.ErrorMessage}");
                    }
                });
                console.Error.WriteLine();

                return Task.FromResult(2);
            }
            return next(commandContext);
        }

        private ValidationResult ValidateModel(IArgumentModel model)
        {
            Type modelType = model.GetType();

            Type declaredValidatorType = modelType.GetCustomAttribute<ValidatorAttribute>()?.ValidatorType;
            
            if (declaredValidatorType != null)
            {
                object validator;   
                try
                {
                    if (_dependencyResolver == null || !_dependencyResolver.TryResolve(declaredValidatorType, out validator))
                    {
                        validator = Activator.CreateInstance(declaredValidatorType);
                    }
                }
                catch (Exception e)
                {
                    throw new AppRunnerException($"Could not create instance of {declaredValidatorType.Name}. Please ensure it's either injected via IoC or has a default constructor.\n" +
                                                 "This exception could also occur if default constructor threw an exception", e);
                }

                return ((IValidator)validator).Validate(model);
            }

            return null;
        }
    }
}