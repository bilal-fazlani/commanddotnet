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
            var dependencyResolver = commandContext.ExecutionConfig.DependencyResolver;
            if (instance != null)
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
                    else // there are some properties but there is no dependency resolver set
                    {
                        throw new AppRunnerException("Dependency resolver is not set for injecting properties. " +
                                                     "Please use an IoC framework'");
                    }
                }
            }

            return next(commandContext);
        }
    }
}