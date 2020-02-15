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
        /// <param name="argumentModelResolveStrategy">
        /// the <see cref="ResolveStrategy"/> used to resolve <see cref="IArgumentModel"/>s.
        /// </param>
        /// <param name="commandClassResolveStrategy">
        /// the <see cref="ResolveStrategy"/> used to resolve command classes.
        /// </param>
        /// <param name="useLegacyInjectDependenciesAttribute"> 
        /// when true, resolve instances for properties marked with [InjectProperty].
        /// This feature is deprecated and may be removed with next major release.
        /// </param>
        public static AppRunner UseMicrosoftDependencyInjection(
            this AppRunner appRunner, 
            IServiceProvider serviceProvider, 
            Func<CommandContext, IDisposable> runInScope = null,
            ResolveStrategy argumentModelResolveStrategy = ResolveStrategy.TryResolve,
            ResolveStrategy commandClassResolveStrategy = ResolveStrategy.Resolve,
            bool useLegacyInjectDependenciesAttribute = false)
        {
            return appRunner.UseDependencyResolver(new MicrosoftDependencyInjectionResolver(serviceProvider),
                runInScope,
                argumentModelResolveStrategy,
                commandClassResolveStrategy,
                useLegacyInjectDependenciesAttribute);
        }
    }
}
