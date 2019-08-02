using System;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;

namespace CommandDotNet.Builders
{
    internal static class DependencyResolveMiddleware
    {
        internal static Task<int> InjectDependencies(CommandContext commandContext, Func<CommandContext, Task<int>> next)
        {
            var instance = commandContext.InvocationContext.Instance;
            var dependencyResolver = commandContext.AppConfig.DependencyResolver;
            if (instance != null && dependencyResolver != null)
            {
                //detect injection properties
                var properties = instance.GetType().GetDeclaredProperties<InjectPropertyAttribute>().ToList();

                if (properties.Any())
                {
                    if (dependencyResolver != null)
                    {
                        foreach (var propertyInfo in properties)
                        {
                            propertyInfo.SetValue(instance, dependencyResolver.Resolve(propertyInfo.PropertyType));
                        }
                    }
                }
            }

            return next(commandContext);
        }
    }
}