using System;
using System.Reflection;
using CommandDotNet.Builders;
using FluentValidation;
using FluentValidation.Attributes;
using FluentValidation.Results;

namespace CommandDotNet.FluentValidation
{
    internal class ModelValidator
    {
        private readonly IDependencyResolver? _dependencyResolver;

        internal ModelValidator(IDependencyResolver? dependencyResolver)
        {
            _dependencyResolver = dependencyResolver;
        }

        internal ValidationResult? ValidateModel(IArgumentModel model)
        {
            Type modelType = model.GetType();

            Type? declaredValidatorType = modelType.GetCustomAttribute<ValidatorAttribute>()?.ValidatorType;
            
            if (declaredValidatorType is { })
            {
                object? validator;   
                try
                {
                    if (_dependencyResolver is null || !_dependencyResolver.TryResolve(declaredValidatorType, out validator))
                    {
                        validator = Activator.CreateInstance(declaredValidatorType);
                    }
                }
                catch (Exception e)
                {
                    var msg = Resources.A.Error_Could_not_create_instance_of(declaredValidatorType.Name);
                    throw new InvalidValidatorException(msg, e);
                }

                return ((IValidator)validator!).Validate(model);
            }

            return null;
        }
    }
}