namespace CommandDotNet
{
    public static class IoCExtension
    {
        public static AppRunner<T> UseDependencyResolver<T>(this AppRunner<T> appRunner, IDependencyResolver dependencyResolver) where T :class
        {
            appRunner.DependencyResolver = dependencyResolver;
            return appRunner;
        }
    }
}