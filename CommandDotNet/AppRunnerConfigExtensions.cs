using System;
using System.Collections.Generic;
using CommandDotNet.Builders;
using CommandDotNet.ClassModeling.Definitions;
using CommandDotNet.Directives;
using CommandDotNet.Execution;
using CommandDotNet.Parsing;
using CommandDotNet.Prompts;
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
            return PipedInputMiddleware.AppendPipedInputToOperandList(appRunner);
        }

        /// <summary>Use the <see cref="IDependencyResolver"/> to create the command classes.</summary>
        /// <param name="appRunner">the <see cref="AppRunner"/> instance</param>
        /// <param name="dependencyResolver">the <see cref="IDependencyResolver"/> to use</param>
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
        public static AppRunner UseDependencyResolver(
            this AppRunner appRunner, 
            IDependencyResolver dependencyResolver,
            bool useResolveForArgumentModel = false,
            bool useTryResolveForCommandClass = false,
            bool useLegacyInjectDependenciesAttribute = false)
        {
            return DependencyResolverMiddleware.UseDependencyResolver(appRunner, dependencyResolver, 
                useResolveForArgumentModel, useTryResolveForCommandClass, useLegacyInjectDependenciesAttribute);
        }

        /// <summary>
        /// Adds support for prompting. <see cref="IPrompter"/> parameters can be used in interceptor and command methods.
        /// <see cref="IPrompter"/> simplifies prompting and is supported by the TestTools nuget package.
        /// </summary>
        /// <param name="appRunner">The <see cref="AppRunner"/> instance</param>
        /// <param name="prompterOverride">The <see cref="IPrompter"/> to use instead of the default.  Overriding this may impact TestTool support.</param>
        /// <param name="skipForMissingArguments">When true, users will be NOT prompted for each missing argument</param>
        /// <param name="argumentPromptTextOverride">Override the default prompt text format.</param>
        /// <param name="argumentFilter">Filter the arguments that will be prompted. i.e. Create a [PromptWhenMissing] attribute, or only prompt for operands.</param>
        public static AppRunner UsePrompting(
            this AppRunner appRunner,
            Func<CommandContext, IPrompter> prompterOverride = null,
            bool skipForMissingArguments = false,
            Func<CommandContext, IArgument, string> argumentPromptTextOverride = null,
            Predicate<IArgument> argumentFilter = null)
        {
            return ValuePromptMiddleware.UsePrompting(appRunner, prompterOverride, skipForMissingArguments, argumentPromptTextOverride, argumentFilter);
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

        /// <summary>
        /// Returns the list of all possible types that could be instantiated to execute commands.<br/>
        /// Use get the list of types to register in your DI container.
        /// </summary>
        public static IEnumerable<Type> GetCommandClassTypes(this AppRunner appRunner) =>
            ClassCommandDef.GetAllCommandClassTypes(appRunner.RootCommandType);

        private static void AssertDirectivesAreEnabled(AppRunner appRunner)
        {
            if (appRunner.AppSettings.DisableDirectives)
            {
                throw new AppRunnerException($"Directives are not enabled.  " +
                                             $"{nameof(AppRunner)}.{nameof(AppRunner.AppSettings)}.{nameof(AppSettings.DisableDirectives)} " +
                                             "must not be set to true");
            }
        }
    }
}