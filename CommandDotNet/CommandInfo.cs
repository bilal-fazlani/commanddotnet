using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandDotNet.Attributes;

namespace CommandDotNet
{
    public class CommandInfo
    {
        public CommandInfo(MethodInfo methodInfo)
        {
            MethodName = methodInfo.Name;
            Parameters = methodInfo.GetParameters().Select(pi => new CommandParameterInfo(pi));
            Description = GetDescription(methodInfo);
        }
        
        public string MethodName { get; }
        public string Description { get; }
        public IEnumerable<CommandParameterInfo> Parameters { get; }


        private string GetDescription(MethodInfo methodInfo)
        {
            CommandAttribute descriptionAttribute = methodInfo.GetCustomAttribute<CommandAttribute>(false);
            return descriptionAttribute?.Description;
        }
    }
}