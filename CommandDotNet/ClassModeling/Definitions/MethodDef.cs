using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;

namespace CommandDotNet.ClassModeling.Definitions
{
    internal class MethodDef : IMethodDef
    {
        public static readonly Type MiddlewareNextParameterType = typeof(ExecutionDelegate);
        public static readonly Type InterceptorNextParameterType = typeof(InterceptorExecutionDelegate);

        private readonly AppConfig _appConfig;
        private IReadOnlyCollection<IArgumentDef> _argumentDefs;
        private IReadOnlyCollection<IArgument> _arguments;
        private ParameterInfo[] _parameters;
        private object[] _values;

        private ArgumentMode _argumentMode;
        private ParameterInfo _nextParameterInfo;
        private readonly List<Action<CommandContext>> _resolvers = new List<Action<CommandContext>>();

        public MethodInfo MethodInfo { get; }

        public IReadOnlyCollection<IArgumentDef> ArgumentDefs => EnsureInitialized(() => _argumentDefs);

        public IReadOnlyCollection<IArgument> Arguments => EnsureInitialized(ref _arguments,
            () => ArgumentDefs.Select(a => a.Argument).ToReadOnlyCollection());

        public IReadOnlyCollection<ParameterInfo> Parameters => EnsureInitialized(() => _parameters);

        public object[] ParameterValues => EnsureInitialized(() => _values);

        public MethodDef(MethodInfo method, AppConfig appConfig)
        {
            MethodInfo = method ?? throw new ArgumentNullException(nameof(method));
            _appConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));
        }

        public static bool IsInterceptorMethod(MethodBase methodBase)
        {
            return methodBase.GetParameters().Any(IsExecutionDelegate);
        }

        private static bool IsExecutionDelegate(ParameterInfo parameterInfo)
        {
            return parameterInfo.ParameterType == MiddlewareNextParameterType 
                   || parameterInfo.ParameterType == InterceptorNextParameterType;
        }

        public object Invoke(CommandContext commandContext, object instance, ExecutionDelegate next)
        {
            if (_nextParameterInfo != null)
            {
                if (next == null)
                {
                    throw new AppRunnerException(
                        $"Invalid operation. {nameof(ExecutionDelegate)} {_nextParameterInfo.Name} parameter not provided for method: {_nextParameterInfo.Member.FullName()}. " +
                        $"Check middleware to ensure it hasn't misconfigured the {nameof(CommandContext.InvocationPipeline)}");
                }

                if (_nextParameterInfo.ParameterType == InterceptorNextParameterType)
                {
                    var nextLite = new InterceptorExecutionDelegate(() => next(commandContext));
                    _values[_nextParameterInfo.Position] = nextLite;
                }
                else
                {
                    _values[_nextParameterInfo.Position] = next;
                }
            }

            _resolvers?.ForEach(r => r(commandContext));

            return MethodInfo.Invoke(instance, _values);
        }

        private T EnsureInitialized<T>(ref T current, Func<T> getValues)
        {
            if (current == null)
            {
                current = getValues();
            }

            return current;
        }

        private T EnsureInitialized<T>(Func<T> getValues)
        {
            if (_argumentDefs == null)
            {
                Initialize();
            }
            return getValues();
        }

        private void Initialize()
        {
            _parameters = MethodInfo.GetParameters();

            var isMiddleware = _parameters.Any(IsExecutionDelegate);

            _argumentMode = isMiddleware
                ? ArgumentMode.Option
                : _appConfig.AppSettings.DefaultArgumentMode;
            
            _values = new object[_parameters.Length];
            
            var parametersByName = _parameters.ToDictionary(
                p => p.Name,
                p => (param: p, args: GetArgsFromParameter(p).ToCollection()));

            var arguments = parametersByName.Values
                .OrderBy(v => v.param.Position)
                .SelectMany(p => p.args)
                .ToReadOnlyCollection();

            _argumentDefs = arguments;
        }

        private IEnumerable<IArgumentDef> GetArgsFromParameter(ParameterInfo parameterInfo)
        {
            if (parameterInfo.ParameterType.InheritsFrom<IArgumentModel>())
            {
                return GetArgumentsFromModel(
                    parameterInfo.ParameterType,
                    null,
                    value => _values[parameterInfo.Position] = value);
            }

            if (_appConfig.ParameterResolversByType.TryGetValue(parameterInfo.ParameterType, out var resolve))
            {
                _resolvers.Add(context => _values[parameterInfo.Position] = resolve(context));
                return Enumerable.Empty<IArgumentDef>();
            }

            if (IsExecutionDelegate(parameterInfo))
            {
                _nextParameterInfo = parameterInfo;
                return Enumerable.Empty<IArgumentDef>();
            }

            return new ParameterArgumentDef(
                    parameterInfo,
                    GetArgumentType(parameterInfo, _argumentMode),
                    _appConfig,
                    _values)
                .ToEnumerable();
        }

        private IEnumerable<IArgumentDef> GetArgsFromProperty(PropertyData propertyData, object modelInstance)
        {
            var propertyInfo = propertyData.PropertyInfo;
            return propertyData.IsArgModel
                ? GetArgumentsFromModel(
                    propertyInfo.PropertyType,
                    propertyInfo.GetValue(modelInstance),
                    value => propertyInfo.SetValue(modelInstance, value), propertyData)
                : new PropertyArgumentDef(
                        propertyInfo,
                        GetArgumentType(propertyInfo, _argumentMode),
                        _appConfig,
                        modelInstance)
                    .ToEnumerable();
        }

        private IEnumerable<IArgumentDef> GetArgumentsFromModel(Type modelType, 
            object existingDefault, Action<object> instanceCreated, PropertyData parentProperty = null)
        {
            var instance = existingDefault ?? _appConfig.ResolverService.ResolveArgumentModel(modelType);

            if (existingDefault == null)
            {
                instanceCreated?.Invoke(instance);
            }

            return modelType
                .GetDeclaredProperties()
                .Select(p => new PropertyData(
                    _appConfig.AppSettings.GuaranteeOperandOrderInArgumentModels,
                    p,
                    parentProperty,
                    GetArgumentType(p, _argumentMode)))
                .SelectMany(propertyInfo => GetArgsFromProperty(propertyInfo, instance));
        }

        private static CommandNodeType GetArgumentType(ICustomAttributeProvider info, ArgumentMode argumentMode)
        {
            return info.IsOption(argumentMode) ? CommandNodeType.Option : CommandNodeType.Operand;
        }

        public override string ToString()
        {
            return $"{MethodInfo.GetType().Name}:{MethodInfo?.DeclaringType?.Name}.{MethodInfo.Name}(" +
                   $"{MethodInfo.GetParameters().Select(p => $"{p.ParameterType} {p.Name}").ToCsv()})";
        }

        private class PropertyData
        {
            private readonly PropertyData _parentProperty;

            public PropertyInfo PropertyInfo { get; }
            public bool IsArgModel { get; }
            public int? LineNumber { get; }

            public PropertyData(bool guaranteeOrder, PropertyInfo propertyInfo, PropertyData parentProperty, CommandNodeType commandNode)
            {
                _parentProperty = parentProperty;
                PropertyInfo = propertyInfo;
                IsArgModel = propertyInfo.PropertyType.InheritsFrom<IArgumentModel>();
                LineNumber = propertyInfo.GetCustomAttribute<OrderByPositionInClassAttribute>()?.CallerLineNumber
                             ?? propertyInfo.GetCustomAttribute<OperandAttribute>()?.CallerLineNumber;

                var isOperand = !IsArgModel && commandNode == CommandNodeType.Operand;

                if (isOperand && guaranteeOrder)
                {
                    if (!LineNumber.HasValue)
                    {
                        throw new InvalidConfigurationException($"Operand property must be attributed with " +
                                                                $"{nameof(OperandAttribute)} or {nameof(OrderByPositionInClassAttribute)} to guarantee consistent order. " +
                                                                $"Property: {propertyInfo.DeclaringType?.FullName}.{propertyInfo.Name}");
                    }

                    var parentsWithoutLineNumber = ParentsWithoutLineNumber().ToList();
                    if (parentsWithoutLineNumber.Any())
                    {
                        var props = parentsWithoutLineNumber
                            .Select(p => p.PropertyInfo)
                            .Select(p => $"  {p.DeclaringType?.FullName}.{p.Name}")
                            .ToCsv(Environment.NewLine);
                        throw new InvalidConfigurationException($"Operand property must be attributed with " +
                                                                $"{nameof(OperandAttribute)} or {nameof(OrderByPositionInClassAttribute)} to guarantee consistent order. " +
                                                                $"Properties:{Environment.NewLine}{props}");
                    }
                }
            }

            private IEnumerable<PropertyData> ParentsWithoutLineNumber()
            {
                for (var p = _parentProperty; p != null; p = p._parentProperty)
                {
                    if (!p.LineNumber.HasValue)
                    {
                        yield return p;
                    }
                }
            }
        }
    }
}