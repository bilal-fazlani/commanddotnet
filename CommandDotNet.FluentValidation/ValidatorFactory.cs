using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentValidation;

namespace CommandDotNet.FluentValidation;

internal class ValidatorFactory(CommandContext ctx)
{
    // caching to avoid multiple assembly scanning and improved perf for REPL sessions.
    // this could be skipped when SourceGenerators are used.
    private static readonly HashSet<Assembly> ScannedAssemblies = new();
    private static readonly Dictionary<Type, Type> ValidatorTypesByModel = new();

    public IValidator? Resolve(IArgumentModel model)
    {
        var (validator, validatorType) = LoadValidatorType(model.GetType());

        if (validator is not null) return validator;
        if (validatorType is null) return null;

        try
        {
            return Activator.CreateInstance(validatorType) as IValidator;
        }
        catch (Exception e)
        {
            var msg = Resources.A.Error_Could_not_create_instance_of(validatorType.Name);
            throw new InvalidValidatorException(msg, e);
        }
    }

    private (IValidator? validator, Type? validatorType) LoadValidatorType(Type modelType)
    {
        if (ValidatorTypesByModel.TryGetValue(modelType, out var validatorType)) return (null, validatorType);
        
        var genericType = typeof(IValidator<>).MakeGenericType(modelType);
        
        if (ctx.DependencyResolver?.TryResolve(genericType, out var validator) == true)
        {
            return (validator as IValidator, null);
        }

        if (ScannedAssemblies.Contains(modelType.Assembly)) return (null, validatorType);
        
        AssemblyScanner
            .FindValidatorsInAssembly(modelType.Assembly)
            .ForEach(r =>
            {
                var validatorModelType = r.InterfaceType.GenericTypeArguments.First();
                ValidatorTypesByModel.Add(validatorModelType, r.ValidatorType);
                if (validatorModelType == modelType)
                {
                    validatorType = r.ValidatorType;
                }
            });

        ScannedAssemblies.Add(modelType.Assembly);
        return (null, validatorType);

    }
}