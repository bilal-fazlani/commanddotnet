using System;

namespace CommandDotNet.IoC.MicrosoftDependencyInjection
{
    public static class AppRunnerConfigurationExtensions
    {
        public static AppRunner UseMicrosoftDependencyInjection(this AppRunner appRunner, IServiceProvider serviceProvider)
        {
            return appRunner.Configure(b =>
                b.DependencyResolver = new MicrosoftDependencyInjectionResolver(serviceProvider));
        }
    }
}
