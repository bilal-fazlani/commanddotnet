using CommandDotNet.Builders;

namespace CommandDotNet
{
    public static class AppRunnerBuilderExtensions
    {
        // DO NOT move this method to another class.  Other nuget packages depend on it.
        public static AppRunner<T> UseDependencyResolver<T>(this AppRunner<T> appRunner, IDependencyResolver dependencyResolver) 
            where T : class
        {
            appRunner.DependencyResolver = dependencyResolver;
            return appRunner;
        }
    }
}