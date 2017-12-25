using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using CommandDotNet.Attributes;

namespace CommandDotNet.Models
{
    public class CommandInfo
    {
        private readonly MethodInfo _methodInfo;
        
        private readonly ApplicationMetadataAttribute _metadataAttribute;

        public CommandInfo(MethodInfo methodInfo, AppSettings settings)
        {
            _methodInfo = methodInfo;
            
            _metadataAttribute = _methodInfo.GetCustomAttribute<ApplicationMetadataAttribute>(false);
            
            Parameters = methodInfo.GetParameters().Select(pi => new ArgumentInfo(pi, settings));
        }

        public string Name => _metadataAttribute?.Name ?? _methodInfo.Name;

        public string MethodName => _methodInfo.Name; 

        public string Description => _metadataAttribute?.Description;

        public string ExtendedHelpText => _metadataAttribute?.ExtendedHelpText;

        public string Syntax => _metadataAttribute?.Syntax;

        public IEnumerable<ArgumentInfo> Parameters { get; }
    }
}