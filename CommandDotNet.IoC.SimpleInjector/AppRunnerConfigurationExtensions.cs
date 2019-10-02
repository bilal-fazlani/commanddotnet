using SimpleInjector;

namespace CommandDotNet.IoC.SimpleInjector
{
    public static class AppRunnerConfigurationExtensions
    {
        public static AppRunner UseSimpleInjector(this AppRunner appRunner, Container container)
        {
            return appRunner.Configure(b =>
                b.DependencyResolver = new SimpleInjectorResolver(container));
        }
    }
}
