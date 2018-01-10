using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandDotNet.Attributes;

namespace CommandDotNet.Models
{
    public class CommandInfo
    {
        private readonly MethodInfo _methodInfo;
        private readonly AppSettings _settings;
        private readonly ApplicationMetadataAttribute _metadataAttribute;

        public CommandInfo(MethodInfo methodInfo, AppSettings settings)
        {
            _methodInfo = methodInfo;
            _settings = settings;

            _metadataAttribute = _methodInfo.GetCustomAttribute<ApplicationMetadataAttribute>(false);
            
            Arguments = GetArguments();
        }

        private IEnumerable<ArgumentInfo> GetArguments()
        {
            List<ArgumentInfo> arguments = new List<ArgumentInfo>();

            foreach (ParameterInfo parameterInfo in _methodInfo.GetParameters())
            {
                arguments.AddRange(ConvertToArgumentInfo(parameterInfo));
            }
            
            return arguments;
        }

        private IEnumerable<ArgumentInfo> ConvertToArgumentInfo(ParameterInfo parameterInfo)
        {
            if (!typeof(IArgumentModel).IsAssignableFrom(parameterInfo.ParameterType))
            {
                if (_settings.MethodArgumentMode == ArgumentMode.Parameter)
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
                foreach (ArgumentInfo argumentInfo in GetArgumentsFromArgumentModel(parameterInfo.ParameterType))
                {
                    yield return argumentInfo;
                }
            }
        }

        private IEnumerable<ArgumentInfo> GetArgumentsFromArgumentModel(Type modelType)
        {
            foreach (var propertyInfo in modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                if (_settings.MethodArgumentMode == ArgumentMode.Parameter)
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

        public string Name => _metadataAttribute?.Name ?? _methodInfo.Name.ChangeCase(_settings.Case);

        public string MethodName => _methodInfo.Name; 

        public string Description => _metadataAttribute?.Description;

        public string ExtendedHelpText => _metadataAttribute?.ExtendedHelpText;

        public string Syntax => _metadataAttribute?.Syntax;

        public IEnumerable<ArgumentInfo> Arguments { get; }
    }
}