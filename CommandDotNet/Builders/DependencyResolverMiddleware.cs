using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;

namespace CommandDotNet.Builders
{
    internal static class DependencyResolverMiddleware
    {
        internal static AppRunner UseDependencyResolver(AppRunner appRunner, IDependencyResolver dependencyResolver,
            bool useResolveForArgumentModel,
            bool useTryResolveForCommandClass,
            bool useLegacyInjectDependenciesAttribute)
        {
            return appRunner.Configure(c =>
            {
                c.DependencyResolver = dependencyResolver;
                c.Services.Add(new ResolverService
                {
                    UseResolveForArgumentModel = useResolveForArgumentModel,
                    UseTryResolveForCommandClass = useTryResolveForCommandClass
                });
                if (useLegacyInjectDependenciesAttribute)
                {
                    c.UseMiddleware(LegacyInjectPropertiesDependencies, MiddlewareStages.PostBindValuesPreInvoke);
                }
            });
        }

        internal static Task<int> LegacyInjectPropertiesDependencies(CommandContext commandContext, ExecutionDelegate next)
        {
            var resolver = commandContext.AppConfig.DependencyResolver;
            if (resolver != null)
            {
                commandContext.InvocationPipeline.All
                    .Select(i => i.Instance)
                    .ForEach(instance =>
                    {
                        //detect injection properties
                        var properties = instance.GetType().GetDeclaredProperties<InjectPropertyAttribute>().ToList();

                        if (properties.Any())
                        {
                            foreach (var propertyInfo in properties)
                            {
                                propertyInfo.SetValue(instance, resolver.Resolve(propertyInfo.PropertyType));
                            }
                        }
                    });
            }

            return next(commandContext);
        }
    }
}