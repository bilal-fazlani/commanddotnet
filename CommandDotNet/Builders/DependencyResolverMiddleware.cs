using CommandDotNet.Execution;

namespace CommandDotNet.Builders
{
    internal static class DependencyResolverMiddleware
    {
        internal static AppRunner UseDependencyResolver(AppRunner appRunner, IDependencyResolver dependencyResolver,
            ResolveStrategy argumentModelResolveStrategy,
            ResolveStrategy commandClassResolveStrategy)
        {
            return appRunner.Configure(c =>
            {
                c.DependencyResolver = dependencyResolver;
                c.Services.Add(new ResolverService
                {
                    ArgumentModelResolveStrategy = argumentModelResolveStrategy,
                    CommandClassResolveStrategy = commandClassResolveStrategy
                });
            });
        }
    }
}