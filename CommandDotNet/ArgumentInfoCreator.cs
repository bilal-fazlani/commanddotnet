using System;
using System.Collections.Generic;
using System.Reflection;
using CommandDotNet.Attributes;
using CommandDotNet.Models;

namespace CommandDotNet
{
    public class ArgumentInfoCreator
    {
        private readonly AppSettings _settings;

        public ArgumentInfoCreator(AppSettings settings)
        {
            _settings = settings;
        }
        
        public IEnumerable<ArgumentInfo> ConvertToArgumentInfos(ParameterInfo parameterInfo, ArgumentMode argumentMode)
        {
            if (!typeof(IArgumentModel).IsAssignableFrom(parameterInfo.ParameterType))
            {
                if (argumentMode == ArgumentMode.Parameter)
                {
                    if (parameterInfo.HasAttribute<OptionAttribute>())
                    {
                        yield return new CommandOptionInfo(parameterInfo, _settings);
                    }
                    else
                    {
                        yield return new CommandParameterInfo(parameterInfo, _settings);
                    }
                }
                else
                {
                    if (parameterInfo.HasAttribute<ArgumentAttribute>())
                    {
                        yield return new CommandParameterInfo(parameterInfo, _settings);
                    }
                    else
                    {
                        yield return new CommandOptionInfo(parameterInfo, _settings);
                    }
                }
            }
            else
            {
                foreach (ArgumentInfo argumentInfo in GetArgumentsFromArgumentModel(parameterInfo.ParameterType, argumentMode))
                {
                    yield return argumentInfo;
                }
            }
        }

        private IEnumerable<ArgumentInfo> GetArgumentsFromArgumentModel(Type modelType, ArgumentMode argumentMode)
        {
            foreach (var propertyInfo in modelType.GetDeclaredProperties())
            {
                if (argumentMode == ArgumentMode.Parameter)
                {
                    if (propertyInfo.HasAttribute<OptionAttribute>())
                    {
                        yield return new CommandOptionInfo(propertyInfo, _settings);
                    }
                    else
                    {
                        yield return new CommandParameterInfo(propertyInfo, _settings);
                    }
                }
                else
                {
                    if (propertyInfo.HasAttribute<ArgumentAttribute>())
                    {
                        yield return new CommandParameterInfo(propertyInfo, _settings);
                    }
                    else
                    {
                        yield return new CommandOptionInfo(propertyInfo, _settings);
                    }
                }
            }
        }
    }
}