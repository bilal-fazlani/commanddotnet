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
            Parameters = methodInfo.GetParameters().Select(pi => new ArgumentInfo(pi, settings));
            Description = GetDescription(methodInfo);
            MethodName = methodInfo.Name;
            ExtendedHelpText = GetExtendedHelpText(methodInfo);
        }

        public string Name { get; }

        public string MethodName { get; }

        public string Description { get; }

        public string ExtendedHelpText { get; }

        public IEnumerable<ArgumentInfo> Parameters { get; }


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

        private string GetExtendedHelpText(MethodInfo methodInfo)
        {
            return methodInfo.GetCustomAttribute<ApplicationMetadataAttribute>(false)?.ExtendedHelpText;
        }
    }
}