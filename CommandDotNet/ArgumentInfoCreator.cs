using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandDotNet.Attributes;
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
        
        public IEnumerable<ArgumentInfo> ConvertToArgumentInfos(ParameterInfo parameterInfo, ArgumentMode argumentMode)
        {
            if (typeof(IArgumentModel).IsAssignableFrom(parameterInfo.ParameterType))
            {
                return GetArgumentsFromArgumentModel(parameterInfo.ParameterType, argumentMode);
            }

            var argumentInfo = IsOption(parameterInfo, argumentMode)
                ? (ArgumentInfo) new CommandOptionInfo(parameterInfo, this._settings)
                : new CommandParameterInfo(parameterInfo, this._settings);

            return new[] {argumentInfo};
        }

        private IEnumerable<ArgumentInfo> GetArgumentsFromArgumentModel(Type modelType, ArgumentMode argumentMode)
        {
            return modelType
                .GetDeclaredProperties()
                .Select(propertyInfo => IsOption(propertyInfo, argumentMode)
                    ? (ArgumentInfo) new CommandOptionInfo(propertyInfo, _settings)
                    : new CommandParameterInfo(propertyInfo, _settings));
        }

        private static bool IsOption(ICustomAttributeProvider attributeProvider, ArgumentMode argumentMode)
        {
            // TODO: developer can force the argument mode with an attribute
            
            switch (argumentMode)
            {
                case ArgumentMode.Parameter:
                    return attributeProvider.HasAttribute<OptionAttribute>();
                case ArgumentMode.Option:
                    return !attributeProvider.HasAttribute<ArgumentAttribute>();
                default:
                    throw new ArgumentOutOfRangeException(nameof(argumentMode), argumentMode, null);
            }
        }
    }
}