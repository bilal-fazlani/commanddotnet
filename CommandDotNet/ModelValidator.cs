using System;
using CommandDotNet.Exceptions;
using CommandDotNet.Extensions;
using FluentValidation.Attributes;
using FluentValidation.Results;

namespace CommandDotNet
{
    public class ModelValidator
    {
        private readonly IDependencyResolver _dependencyResolver;

        public ModelValidator(IDependencyResolver dependencyResolver)
        {
            _dependencyResolver = dependencyResolver;
        }

        public void ValidateModel(dynamic model)
        {
            if (!(model is IArgumentModel)) return;
            
            Type modelType = model.GetType();

            Type declaredValidatorType = modelType.GetCustomAttribute<ValidatorAttribute>()?.ValidatorType;
            
            if (declaredValidatorType != null)
            {
                //Type validatorType = typeof(AbstractValidator<>).MakeGenericType(modelType);
                
                dynamic validator;
                try
                {
                    validator = _dependencyResolver?.Resolve(declaredValidatorType) ??
                                Activator.CreateInstance(declaredValidatorType);
                }
                catch (Exception)
                {
                    throw new AppRunnerException($"Could not create instance of {declaredValidatorType.Name}. Please ensure it's either injected via IoC or has a default constructor.\n" +
                                                 "This exception could also occur if default constructor threw an exception");
                }

                ValidationResult validationResult = validator.Validate(model);
                    //((AbstractValidator<dynamic>)validator).Validate(model);

                if (!validationResult.IsValid)
                {
                    throw new ArgumentValidationException(validationResult);
                }
            }
        }
    }
}