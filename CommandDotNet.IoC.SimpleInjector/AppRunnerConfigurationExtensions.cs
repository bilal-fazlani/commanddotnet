using System;
using CommandDotNet.Execution;
using SimpleInjector;

namespace CommandDotNet.IoC.SimpleInjector
{
    public static class AppRunnerConfigurationExtensions
    {
        /// <summary>Use the provided SimpleInjector container to resolve command classes and <see cref="IArgumentModel"/>s</summary>
        /// <param name="appRunner">the <see cref="AppRunner"/></param>
        /// <param name="container">the SimpleInjector container to use</param>
        /// <param name="runInScope">if provided, the scope will be created at the beginning of the run and disposed at the end</param>
        public static AppRunner UseSimpleInjector(
            this AppRunner appRunner, 
            Container container, 
            Func<CommandContext, IDisposable> runInScope = null)
        {
            return appRunner.Configure(b =>
            {
                b.DependencyResolver = new SimpleInjectorResolver(container);
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
