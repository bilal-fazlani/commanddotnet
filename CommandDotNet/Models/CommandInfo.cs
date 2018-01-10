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
        private readonly ArgumentInfoCreator _argumentInfoCreator;

        public CommandInfo(MethodInfo methodInfo, AppSettings settings)
        {
            _methodInfo = methodInfo;
            _settings = settings;

            _metadataAttribute = _methodInfo.GetCustomAttribute<ApplicationMetadataAttribute>(false);
            _argumentInfoCreator = new ArgumentInfoCreator(settings);
            
            Arguments = GetArguments();
        }

        private IEnumerable<ArgumentInfo> GetArguments()
        {
            List<ArgumentInfo> arguments = new List<ArgumentInfo>();

            foreach (ParameterInfo parameterInfo in _methodInfo.GetParameters())
            {
                arguments.AddRange(_argumentInfoCreator.ConvertToArgumentInfos(parameterInfo, _settings.MethodArgumentMode));
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