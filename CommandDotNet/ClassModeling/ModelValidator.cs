using System;
using CommandDotNet.Builders;
using CommandDotNet.Extensions;
using FluentValidation;
using FluentValidation.Attributes;
using FluentValidation.Results;

namespace CommandDotNet.ClassModeling
{
    public class ModelValidator
    {
        private readonly IDependencyResolver _dependencyResolver;

        public ModelValidator(IDependencyResolver dependencyResolver)
        {
            _dependencyResolver = dependencyResolver;
        }

        public void ValidateModel(IArgumentModel model)
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