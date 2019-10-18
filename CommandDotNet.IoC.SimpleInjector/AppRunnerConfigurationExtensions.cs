using System;
using CommandDotNet.Builders;
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
        /// <param name="useResolveForArgumentModel">
        /// <see cref="IDependencyResolver.TryResolve"/> is the default to resolve <see cref="IArgumentModel"/>s.
        /// Set this to true to use <see cref="IDependencyResolver.Resolve"/>.
        /// If Resolve is used and returns null, this framework will attempt to
        /// instantiate an instance.
        /// </param>
        /// <param name="useTryResolveForCommandClass">
        /// <see cref="IDependencyResolver.Resolve"/> is the default to resolve command classes.
        /// Set this to true to use <see cref="IDependencyResolver.TryResolve"/>.
        /// If Resolve is used and returns null, this framework will attempt to
        /// instantiate an instance.
        /// </param>
        /// <param name="useLegacyInjectDependenciesAttribute"> 
        /// when true, resolve instances for properties marked with [InjectProperty].
        /// This feature is deprecated and may be removed with next major release.
        /// </param>
        public static AppRunner UseSimpleInjector(
            this AppRunner appRunner, 
            Container container, 
            Func<CommandContext, IDisposable> runInScope = null,
            bool useResolveForArgumentModel = false,
            bool useTryResolveForCommandClass = false,
            bool useLegacyInjectDependenciesAttribute = false)
        {
            return appRunner
                .UseDependencyResolver(new SimpleInjectorResolver(container), 
                    useResolveForArgumentModel: useResolveForArgumentModel,
                    useTryResolveForCommandClass: useTryResolveForCommandClass,
                    useLegacyInjectDependenciesAttribute: useLegacyInjectDependenciesAttribute)
                .Configure(b =>
            {
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
