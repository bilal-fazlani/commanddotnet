using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using CommandDotNet.Attributes;
using Humanizer;

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

            foreach (var parameter in _methodInfo.GetParameters())
            {
                if (_settings.MethodArgumentMode == ArgumentMode.Parameter)
                {
                    if (parameter.HasAttribute<OptionAttribute>())
                    {
                        arguments.Add(new CommandOptionInfo(parameter, _settings));
                    }
                    else
                    {
                        arguments.Add(new CommandParameterInfo(parameter, _settings));
                    }
                }
                else
                {
                    if (parameter.HasAttribute<ArgumentAttribute>())
                    {
                        arguments.Add(new CommandParameterInfo(parameter, _settings));
                    }
                    else
                    {
                        arguments.Add(new CommandOptionInfo(parameter, _settings));
                    }
                }
            }

            return arguments;
        }

        public string Name => _metadataAttribute?.Name ?? _methodInfo.Name.ChangeCase(_settings.Case);

        public string MethodName => _methodInfo.Name; 

        public string Description => _metadataAttribute?.Description;

        public string ExtendedHelpText => _metadataAttribute?.ExtendedHelpText;

        public string Syntax => _metadataAttribute?.Syntax;

        public IEnumerable<ArgumentInfo> Arguments { get; }
    }
}