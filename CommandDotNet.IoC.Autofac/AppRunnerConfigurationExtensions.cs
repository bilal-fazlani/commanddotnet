using System;
using Autofac;
using CommandDotNet.Execution;

namespace CommandDotNet.IoC.Autofac
{
    public static class AppRunnerConfigurationExtensions
    {
        /// <summary>Use the provided Autofac container to resolve command classes and <see cref="IArgumentModel"/>s</summary>
        /// <param name="appRunner">the <see cref="AppRunner"/></param>
        /// <param name="container">the Autofac container to use</param>
        /// <param name="runInScope">if provided, the scope will be created at the beginning of the run and disposed at the end</param>
        public static AppRunner UseAutofac(
            this AppRunner appRunner, 
            IContainer container, 
            Func<CommandContext, IDisposable> runInScope = null)
        {
            return appRunner.Configure(b =>
            {
                b.DependencyResolver = new AutofacResolver(container);
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
