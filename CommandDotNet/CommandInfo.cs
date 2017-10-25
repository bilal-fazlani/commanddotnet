using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommandDotNet
{
    public class CommandInfo
    {
        public CommandInfo(MethodInfo methodInfo)
        {
            MethodName = methodInfo.Name;
            Parameters = methodInfo.GetParameters().Select(pi => new CommandParameterInfo(pi));
        }
        
        public string MethodName { get; }
        public IEnumerable<CommandParameterInfo> Parameters { get; }
    }
}