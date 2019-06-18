using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandDotNet.Attributes;
using CommandDotNet.Exceptions;
using CommandDotNet.Extensions;
using CommandDotNet.Models;

namespace CommandDotNet
{
    internal class ArgumentInfoCreator
    {
        private readonly AppSettings _settings;

        public ArgumentInfoCreator(AppSettings settings)
        {
            _settings = settings;
        }

        public IEnumerable<ArgumentInfo> GetArgumentsFromMethod(MethodBase methodBase)
        {
            var isCtor = methodBase is ConstructorInfo;

            var argumentMode = isCtor 
                ? ArgumentMode.Option 
                : _settings.MethodArgumentMode;
            
            return methodBase
                .GetParameters()
                .SelectMany(p =>
                {
                    if (isCtor && p.HasAttribute<OperandAttribute>())
                    {
                        throw new AppRunnerException("Constructor arguments can not have [Operand] attribute. Please use [Option] attribute");
                    }
                    return GetArgumentsFromParameter(p, argumentMode);
                })
                .ToList();
        }
        
        public IEnumerable<ArgumentInfo> GetArgumentsFromModel(Type modelType, ArgumentMode argumentMode)
        {
            return modelType
                .GetDeclaredProperties()
                .SelectMany(propertyInfo => GetArgumentFromProperty(propertyInfo, argumentMode));
        }

        private IEnumerable<ArgumentInfo> GetArgumentsFromParameter(ParameterInfo parameterInfo, ArgumentMode argumentMode)
        {
            if (parameterInfo.ParameterType.InheritsFrom<IArgumentModel>())
            {
                return GetArgumentsFromModel(parameterInfo.ParameterType, argumentMode);
            }
            
            var argumentInfo = IsOption(parameterInfo, argumentMode)
                ? (ArgumentInfo) new OptionArgumentInfo(parameterInfo, _settings)
                : new OperandArgumentInfo(parameterInfo, _settings);

            return new[] {argumentInfo};
        }

        private IEnumerable<ArgumentInfo> GetArgumentFromProperty(PropertyInfo propertyInfo, ArgumentMode argumentMode)
        {
            if (propertyInfo.PropertyType.InheritsFrom<IArgumentModel>())
            {
                return GetArgumentsFromModel(propertyInfo.PropertyType, argumentMode);
            }

            var argumentInfo = IsOption(propertyInfo, argumentMode)
                ? (ArgumentInfo) new OptionArgumentInfo(propertyInfo, _settings)
                : new OperandArgumentInfo(propertyInfo, _settings);

            return new[] {argumentInfo};
        }

        private static bool IsOption(ICustomAttributeProvider attributeProvider, ArgumentMode argumentMode)
        {
            // developer can force the argument mode with an attribute
            
            switch (argumentMode)
            {
                case ArgumentMode.Operand:
                    return attributeProvider.HasAttribute<OptionAttribute>();
                case ArgumentMode.Option:
                    return !attributeProvider.HasAttribute<OperandAttribute>()
                        && !attributeProvider.HasAttribute<ArgumentAttribute>();
                default:
                    throw new ArgumentOutOfRangeException(nameof(argumentMode), argumentMode, null);
            }
        }
    }
}
