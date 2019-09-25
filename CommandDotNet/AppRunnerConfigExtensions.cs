using System;
using CommandDotNet.Builders;
using CommandDotNet.ClassModeling;
using CommandDotNet.Directives;
using CommandDotNet.Execution;
using CommandDotNet.Parsing;
using CommandDotNet.Tokens;

namespace CommandDotNet
{
    /// <summary>Extensions to enable and configure features</summary>
    public static class AppRunnerConfigExtensions
    {
        /// <summary>Adds the --version option to the app</summary>
        public static AppRunner UseVersionMiddleware(this AppRunner appRunner)
        {
            return VersionMiddleware.UseVersionMiddleware(appRunner);
        }

        /// <summary>
        /// When the first argument is [debug], the framework will wait for a debugger to attach.<br/>
        /// Note: Use with <see cref="UseCancellationHandlers"/> to be able to cancel before attaching the debugger.
        /// </summary>
        public static AppRunner UseDebugDirective(this AppRunner appRunner)
        {
            AssertDirectivesAreEnabled(appRunner);
            return DebugDirective.UseDebugDirective(appRunner);
        }
        
        /// <summary>
        /// When the first argument is [parse], the framework will output the result of all <see cref="TokenTransformation"/>s<br/>
        /// </summary>
        public static AppRunner UseParseDirective(this AppRunner appRunner)
        {
            AssertDirectivesAreEnabled(appRunner);
            return ParseDirective.UseParseDirective(appRunner);
        }

        /// <summary>Piped input will be appended to an operand list if one exists for the command</summary>
        public static AppRunner AppendPipedInputToOperandList(this AppRunner appRunner)
        {
            return PipedInputMiddleware.EnablePipedInput(appRunner);
        }

        /// <summary>Enables FluentValidation for <see cref="IArgumentModel"/>s</summary>
        public static AppRunner UseFluentValidation(this AppRunner appRunner)
        {
            // TODO: move FluentValidation into a separate repo & nuget package?
            //       there are other ways to do validation that could also
            //       be applied to parameters

            return appRunner.Configure(c =>
                c.UseMiddleware(ModelValidator.FluentValidationMiddleware, MiddlewareStages.PostBindValuesPreInvoke));
        }

        /// <summary>Use the <see cref="IDependencyResolver"/> to create the command classes.</summary>
        public static AppRunner UseDependencyResolver(
            this AppRunner appRunner, 
            IDependencyResolver dependencyResolver, 
            bool useLegacyInjectDependenciesAttribute = false)
        {
            return DependencyResolverMiddleware.UseDependencyResolver(appRunner, dependencyResolver, useLegacyInjectDependenciesAttribute);
        }

        /// <summary>Users will be prompted for values when operands are not provided</summary>
        public static AppRunner UsePromptForMissingOperands(this AppRunner appRunner)
        {
            return appRunner.Configure(c =>
                c.UseMiddleware(ValuePromptMiddleware.PromptForMissingOperands, MiddlewareStages.PostParseInputPreBindValues));
        }

        /// <summary>Prefix a filepath with @ and it will be replaced by its contents during <see cref="MiddlewareStages.Tokenize"/></summary>
        public static AppRunner UseResponseFiles(this AppRunner appRunner)
        {
            return ExpandResponseFilesTransformation.UseResponseFiles(appRunner);
        }

        /// <summary>
        /// Sets <see cref="AppConfig.CancellationToken"/> and cancels the token on
        /// <see cref="Console.CancelKeyPress"/>, <see cref="AppDomain.ProcessExit"/> and
        /// <see cref="AppDomain.UnhandledException"/> if <see cref="UnhandledExceptionEventArgs.IsTerminating"/> is true.<br/>
        /// Once cancelled, the pipelines will not progress to the next step.
        /// </summary>
        public static AppRunner UseCancellationHandlers(this AppRunner appRunner)
        {
            return CancellationMiddleware.UseCancellationHandlers(appRunner);
        }

        private static void AssertDirectivesAreEnabled(AppRunner appRunner)
        {
            if (!appRunner.AppSettings.EnableDirectives)
            {
                throw new AppRunnerException($"Directives are not enabled.  " +
                                             $"{nameof(AppRunner)}.{nameof(AppRunner.AppSettings)}.{nameof(AppSettings.EnableDirectives)} " +
                                             "must be set to true");
            }
        }
    }
}