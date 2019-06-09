using System.Collections.Generic;
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
            
            Arguments = new ArgumentInfoCreator(settings).GetArgumentsFromMethod(methodInfo);
        }

        public string Name => _metadataAttribute?.Name ?? _methodInfo.Name.ChangeCase(_settings.Case);

        public string MethodName => _methodInfo.Name; 

        public string Description => _metadataAttribute?.Description;

        public string ExtendedHelpText => _metadataAttribute?.ExtendedHelpText;

        public string Syntax => _metadataAttribute?.Syntax;

        public IEnumerable<ArgumentInfo> Arguments { get; }
    }
}