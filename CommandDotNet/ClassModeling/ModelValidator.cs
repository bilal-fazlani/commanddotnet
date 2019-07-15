using System;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Builders;
using CommandDotNet.ClassModeling.Definitions;
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

        internal static Task<int> ValidateModelsMiddleware(CommandContext commandContext, Func<CommandContext, Task<int>> next)
        {
            var commandDef = commandContext.CurrentCommand.ContextData.Get<ICommandDef>();
            if (commandDef != null)
            {
                var modelValidator = new ModelValidator(commandContext.ExecutionConfig.DependencyResolver);

                // TODO: move to Context object
                var instantiateValues = commandDef.InstantiateMethodDef.ParameterValues;
                var invokeValues = commandDef.InvokeMethodDef.ParameterValues;

                foreach (var model in instantiateValues.Union(invokeValues).OfType<IArgumentModel>())
                {
                    modelValidator.ValidateModel(model);
                }
            }
            return next(commandContext);
        }

        private void ValidateModel(IArgumentModel model)
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

                ValidationResult validationResult = ((IValidator)validator).Validate(model);

                if (!validationResult.IsValid)
                {
                    throw new ArgumentValidationException(validationResult);
                }
            }
        }
    }
}