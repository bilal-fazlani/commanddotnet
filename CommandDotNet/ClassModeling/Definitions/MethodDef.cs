using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;

namespace CommandDotNet.ClassModeling.Definitions
{
    internal class MethodDef
    {
        private readonly MethodBase _method;
        private readonly ExecutionConfig _executionConfig;
        private readonly Lazy<IReadOnlyCollection<IArgumentDef>> _arguments;
        private object[] _values;

        public IReadOnlyCollection<IArgumentDef> Arguments => _arguments.Value;

        public MethodDef(MethodBase method, ExecutionConfig executionConfig)
        {
            _method = method ?? throw new ArgumentNullException(nameof(method));
            _executionConfig = executionConfig ?? throw new ArgumentNullException(nameof(executionConfig));

            _arguments = new Lazy<IReadOnlyCollection<IArgumentDef>>(BuildArguments);
        }

        public object Invoke(object instance)
        {
            // TODO: make async
            // TODO: pass execution context
            return _method is ConstructorInfo ctor
                ? ctor.Invoke(_values)
                : _method.Invoke(instance, _values);
        }

        private IReadOnlyCollection<IArgumentDef> BuildArguments()
        {
            var isCtor = _method is ConstructorInfo;

            var argumentMode = isCtor
                ? ArgumentMode.Option
                : _executionConfig.AppSettings.MethodArgumentMode;

            var parametersByName = _method.GetParameters().ToDictionary(
                p => p.Name,
                p => (param: p, args: GetArgsFromParameter(p, argumentMode).ToList()));

            _values = new object[parametersByName.Count];

            var arguments = parametersByName.Values
                .OrderBy(v => v.param.Position)
                .SelectMany(p => p.args)
                .ToList();

            return arguments.AsReadOnly();
        }

        private IEnumerable<IArgumentDef> GetArgsFromParameter(ParameterInfo parameterInfo, ArgumentMode argumentMode) =>
            parameterInfo.ParameterType.InheritsFrom<IArgumentModel>()
                ? GetArgumentsFromModel(
                    parameterInfo.ParameterType,
                    argumentMode,
                    null,
                    value => _values[parameterInfo.Position] = value)
                : new ParameterArgumentDef(
                        parameterInfo,
                        GetArgumentType(parameterInfo, argumentMode),
                        _executionConfig,
                        value => _values[parameterInfo.Position] = value)
                    .ToEnumerable();

        private IEnumerable<IArgumentDef> GetArgsFromProperty(PropertyInfo propertyInfo, ArgumentMode argumentMode, object modelInstance) =>
            propertyInfo.PropertyType.InheritsFrom<IArgumentModel>()
                ? GetArgumentsFromModel(
                    propertyInfo.PropertyType,
                    argumentMode,
                    propertyInfo.GetValue(modelInstance),
                    value => propertyInfo.SetValue(modelInstance, value))
                : new PropertyArgumentDef(
                        propertyInfo,
                        GetArgumentType(propertyInfo, argumentMode),
                        _executionConfig,
                        modelInstance)
                    .ToEnumerable();

        private IEnumerable<IArgumentDef> GetArgumentsFromModel(Type modelType, ArgumentMode argumentMode, object existingDefault, Action<object> instanceCreated)
        {
            // Enhancement: add IDependencyResolver.TryResolve and try resolve first.
            var instance = existingDefault ?? Activator.CreateInstance(modelType);

            if (existingDefault == null)
            {
                instanceCreated?.Invoke(instance);
            }

            return modelType
                .GetDeclaredProperties()
                .SelectMany(propertyInfo => GetArgsFromProperty(propertyInfo, argumentMode, instance));
        }

        private static ArgumentType GetArgumentType(ICustomAttributeProvider info, ArgumentMode argumentMode)
        {
            return info.IsOption(argumentMode) ? ArgumentType.Option : ArgumentType.Operand;
        }

        public override string ToString()
        {
            return $"{_method.GetType().Name}:{_method.DeclaringType.Name}.{_method.Name}(" +
                   $"{_method.GetParameters().Select(p => $"{p.ParameterType} {p.Name}").ToCsv()})";
        }
    }
}