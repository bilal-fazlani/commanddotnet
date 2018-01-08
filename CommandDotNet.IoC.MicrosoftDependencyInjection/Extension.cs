using System;

namespace CommandDotNet.IoC.MicrosoftDependencyInjection
{
    public static class Extension
    {
        public static AppRunner<T> UseMicrosoftDependencyInjection<T>(this AppRunner<T> appRunner, IServiceProvider serviceProvider) where T :class
        {
            appRunner.DependencyResolver = new MicrosoftDependencyInjectionResolver(serviceProvider);
            return appRunner;
        }
    }
}