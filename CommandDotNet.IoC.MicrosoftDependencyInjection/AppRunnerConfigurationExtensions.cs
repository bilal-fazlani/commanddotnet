using System;
using CommandDotNet.Execution;

namespace CommandDotNet.IoC.MicrosoftDependencyInjection
{
    public static class AppRunnerConfigurationExtensions
    {
        /// <summary>Use the provided Microsoft service provider to resolve command classes and <see cref="IArgumentModel"/>s</summary>
        /// <param name="appRunner">the <see cref="AppRunner"/></param>
        /// <param name="serviceProvider">the <see cref="IServiceProvider"/> to use</param>
        /// <param name="runInScope">if provided, the scope will be created at the beginning of the run and disposed at the end</param>
        public static AppRunner UseMicrosoftDependencyInjection(
            this AppRunner appRunner, 
            IServiceProvider serviceProvider, 
            Func<CommandContext, IDisposable> runInScope = null)
        {
            return appRunner.Configure(b =>
            {
                b.DependencyResolver = new MicrosoftDependencyInjectionResolver(serviceProvider);
                if (runInScope != null)
                {
                    b.UseMiddleware((context, next) =>
                    {
                        using (runInScope(context))
                        {
                            return next(context);
                        }
                    }, MiddlewareStages.Tokenize, int.MinValue);
                }
            });
        }
    }
}
