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
            Name = GetName(methodInfo);
            Parameters = methodInfo.GetParameters().Select(pi => new ArguementInfo(pi, settings));
            Description = GetDescription(methodInfo);
            MethodName = methodInfo.Name;
        }
        
        public string Name { get; }
        public string MethodName { get; }
        public string Description { get; }
        public IEnumerable<ArguementInfo> Parameters { get; }


        private string GetName(MethodInfo methodInfo)
        {
            ApplicationMetadataAttribute applicationMetadataAttribute = methodInfo.GetCustomAttribute<ApplicationMetadataAttribute>(false);
            return applicationMetadataAttribute?.Name ?? methodInfo.Name;
        }
        
        private string GetDescription(MethodInfo methodInfo)
        {
            ApplicationMetadataAttribute descriptionAttribute = methodInfo.GetCustomAttribute<ApplicationMetadataAttribute>(false);
            return descriptionAttribute?.Description;
        }
    }
}