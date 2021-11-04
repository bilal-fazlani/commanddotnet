using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommandDotNet.Tests.Utils
{
    public static class AssemblyExtensions
    {
        private const string CommandDotNet = "CommandDotNet";

        public static IEnumerable<Assembly> GetAllCommandDotNetAssemblies()
        {
            return Assembly
                .GetExecutingAssembly()
                .GetReferencedAssemblies()
                .Where(a => a.FullName == CommandDotNet || a.FullName.StartsWith(CommandDotNet))
                .Select(Assembly.Load);
        }
    }
}