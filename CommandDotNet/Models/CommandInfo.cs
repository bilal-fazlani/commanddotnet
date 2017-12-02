using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandDotNet.Attributes;

namespace CommandDotNet.Models
{
    public class CommandInfo
    {
        public CommandInfo(MethodInfo methodInfo, AppSettings settings)
        {
            Name = methodInfo.Name;
            Parameters = methodInfo.GetParameters().Select(pi => new ArguementInfo(pi, settings));
            Description = GetDescription(methodInfo);
        }
        
        public string Name { get; }
        public string Description { get; }
        public IEnumerable<ArguementInfo> Parameters { get; }


        private string GetDescription(MethodInfo methodInfo)
        {
            ApplicationMetadataAttribute descriptionAttribute = methodInfo.GetCustomAttribute<ApplicationMetadataAttribute>(false);
            return descriptionAttribute?.Description;
        }
    }
}