namespace CommandDotNet
{
    public static class IoCExtension
    {
        public static AppRunner<T> UseDependencyResolver<T>(this AppRunner<T> appRunner, IDependencyResolver dependencyResolver) where T :class
        {
            // keep this method for backwards compatibility w/ Autofac and Microsoft resolvers
            return appRunner.UseDependencyResolver(dependencyResolver);
        }
    }
}