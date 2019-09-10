using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;

namespace CommandDotNet.Builders
{
    internal static class DependencyResolverMiddleware
    {
        internal static AppRunner UseDependencyResolver(AppRunner appRunner, IDependencyResolver dependencyResolver, bool useLegacyInjectDependenciesAttribute)
        {
            return appRunner.Configure(c =>
            {
                c.DependencyResolver = dependencyResolver;
                if (useLegacyInjectDependenciesAttribute)
                {
                    c.UseMiddleware(InjectDependencies, MiddlewareStages.PostBindValuesPreInvoke);
                }
            });
        }

        internal static Task<int> InjectDependencies(CommandContext commandContext, ExecutionDelegate next)
        {
            var instance = commandContext.InvocationContext.Instance;
            var dependencyResolver = commandContext.AppConfig.DependencyResolver;
            if (instance != null && dependencyResolver != null)
            {
                //detect injection properties
                var properties = instance.GetType().GetDeclaredProperties<InjectPropertyAttribute>().ToList();

                if (properties.Any())
                {
                    foreach (var propertyInfo in properties)
                    {
                        propertyInfo.SetValue(instance, dependencyResolver.Resolve(propertyInfo.PropertyType));
                    }
                }
            }

            return next(commandContext);
        }
    }
}