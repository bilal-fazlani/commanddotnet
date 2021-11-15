using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentValidation;

namespace CommandDotNet.FluentValidation
{
    internal class ValidatorFactory
    {
        // caching to avoid multiple assembly scanning and improved perf for REPL sessions.
        // this could be skipped when SourceGenerators are used.
        private static readonly HashSet<Assembly> ScannedAssemblies = new();
        private static readonly Dictionary<Type, Type> ValidatorTypesByModel = new();

        private readonly CommandContext _ctx;

        public ValidatorFactory(CommandContext ctx)
        {
            _ctx = ctx;
        }

        public IValidator? Resolve(IArgumentModel model)
        {
            var modelType = model.GetType();

            if (!ValidatorTypesByModel.TryGetValue(modelType, out var validatorType))
            {
                var genericType = typeof(IValidator<>).MakeGenericType(modelType);
                if (_ctx.DependencyResolver is not null && _ctx.DependencyResolver.TryResolve(genericType, out var validator))
                {
                    return validator as IValidator;
                }

                if (!ScannedAssemblies.Contains(modelType.Assembly))
                {
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
                }
            }

            if (validatorType is null)
            {
                return null;
            }

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
    }
}