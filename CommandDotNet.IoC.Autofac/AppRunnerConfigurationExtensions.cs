using Autofac;

namespace CommandDotNet.IoC.Autofac
{
    public static class AppRunnerConfigurationExtensions
    {
        public static AppRunner UseAutofac(this AppRunner appRunner, IContainer container)
        {
            return appRunner.Configure(b =>
                b.DependencyResolver = new AutofacResolver(container));
        }
    }
}
