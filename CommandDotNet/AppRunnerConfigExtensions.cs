﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using CommandDotNet.Builders;
using CommandDotNet.Builders.ArgumentDefaults;
using CommandDotNet.ClassModeling.Definitions;
using CommandDotNet.Diagnostics;
using CommandDotNet.Execution;
using CommandDotNet.Localization;
using CommandDotNet.Parsing;
using CommandDotNet.Parsing.Typos;
using CommandDotNet.Prompts;
using CommandDotNet.Tokens;

namespace CommandDotNet
{
    /// <summary>Extensions to enable and configure features</summary>
    public static class AppRunnerConfigExtensions
    {
        internal static bool InUseDefaultMiddleware => !ExcludeParamName.IsNullOrWhitespace();
        internal static string? ExcludeParamName { get; private set; }
        
        /// <summary>
        /// Configures the <see cref="AppRunner"/> with the 'default' set of middleware.
        /// See the 'exclude...' parameters for the list of included middleware.
        /// </summary>
        public static AppRunner UseDefaultMiddleware(this AppRunner appRunner,
            bool excludeCancellationHandlers = false,
            bool excludeDebugDirective = false,
            bool excludeParseDirective = false,
            bool excludeTimeDirective = false,
            bool excludePrompting = false,
            bool excludeResponseFiles = false,
            bool excludeVersionMiddleware = false,
            bool excludeAppendPipedInputToOperandList = false,
            bool excludeTypoSuggestions = false)
        {
            static void Register(bool exclude, string paramName, Action register)
            {
                if (exclude) return;
                ExcludeParamName = paramName;
                register();
                ExcludeParamName = null;
            }
            
            Register(
                excludeCancellationHandlers, 
                nameof(excludeCancellationHandlers),
                ()=> appRunner.UseCancellationHandlers());
            Register(
                excludeDebugDirective, 
                nameof(excludeDebugDirective),
                ()=> appRunner.UseDebugDirective());
            Register(
                excludeParseDirective, 
                nameof(excludeParseDirective),
                ()=> appRunner.UseParseDirective());
            Register(
                excludeTimeDirective,
                nameof(excludeTimeDirective),
                () => appRunner.UseTimeDirective());
            Register(
                excludePrompting, 
                nameof(excludePrompting),
                ()=> appRunner.UsePrompting());
            Register(
                excludeResponseFiles, 
                nameof(excludeResponseFiles),
                ()=> appRunner.UseResponseFiles());
            Register(
                excludeVersionMiddleware, 
                nameof(excludeVersionMiddleware),
                ()=> appRunner.UseVersionMiddleware());
            Register(
                excludeAppendPipedInputToOperandList, 
                nameof(excludeAppendPipedInputToOperandList),
                ()=> appRunner.AppendPipedInputToOperandList());
            Register(
                excludeTypoSuggestions, 
                nameof(excludeTypoSuggestions),
                ()=> appRunner.UseTypoSuggestions());
            return appRunner;
        }

        /// <summary>When an invalid arguments is entered, suggests context based alternatives</summary>
        public static AppRunner UseTypoSuggestions(this AppRunner appRunner, int maxSuggestionCount = 5)
        {
            return TypoSuggestionsMiddleware.UseTypoSuggestions(appRunner, maxSuggestionCount);
        }

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
        /// When the first argument is [time], the framework will time the execution of the command
        /// and output the result to the console.
        /// </summary>
        public static AppRunner UseTimeDirective(this AppRunner appRunner)
        {
            AssertDirectivesAreEnabled(appRunner);
            return TimeDirective.UseTimeDirective(appRunner);
        }

        /// <summary>
        /// When the first argument is [parse], the framework will output the result of all <see cref="TokenTransformation"/>s<br/>
        /// </summary>
        public static AppRunner UseParseDirective(this AppRunner appRunner)
        {
            AssertDirectivesAreEnabled(appRunner);
            return ParseDirective.UseParseDirective(appRunner);
        }
        
        /// <summary>
        /// When the first argument is [loc:{culture}], the framework will set the current culture
        /// to the culture provided.  Useful for testing localization.
        /// </summary>
        public static AppRunner UseLocalizeDirective(this AppRunner appRunner)
        {
            return CultureDirective.UseCultureDirective(appRunner);
        }

        /// <summary>Piped input will be appended to an operand list if one exists for the command</summary>
        public static AppRunner AppendPipedInputToOperandList(this AppRunner appRunner)
        {
            return PipedInputMiddleware.AppendPipedInputToOperandList(appRunner);
        }

        /// <summary>Use the <see cref="IDependencyResolver"/> to create the command classes.</summary>
        /// <param name="appRunner">the <see cref="AppRunner"/> instance</param>
        /// <param name="dependencyResolver">the <see cref="IDependencyResolver"/> to use</param>
        /// <param name="runInScope">if provided, the scope will be created at the beginning of the run and disposed at the end</param>
        /// <param name="argumentModelResolveStrategy">
        /// the <see cref="ResolveStrategy"/> used to resolve <see cref="IArgumentModel"/>s.</param>
        /// <param name="commandClassResolveStrategy">
        /// the <see cref="ResolveStrategy"/> used to resolve command classes. </param>
        public static AppRunner UseDependencyResolver(
            this AppRunner appRunner, 
            IDependencyResolver dependencyResolver,
            Func<CommandContext, IDisposable>? runInScope = null,
            ResolveStrategy argumentModelResolveStrategy = ResolveStrategy.TryResolve,
            ResolveStrategy commandClassResolveStrategy = ResolveStrategy.Resolve)
        {
            return DependencyResolverMiddleware.UseDependencyResolver(
                appRunner, dependencyResolver, runInScope,
                argumentModelResolveStrategy, commandClassResolveStrategy);
        }

        /// <summary>
        /// Adds support for prompting. <see cref="IPrompter"/> parameters can be used in interceptor and command methods.
        /// <see cref="IPrompter"/> simplifies prompting and is supported by the TestTools nuget package.
        /// </summary>
        /// <param name="appRunner">The <see cref="AppRunner"/> instance</param>
        /// <param name="prompterOverride">The <see cref="IPrompter"/> to use instead of the default.  Overriding this may impact TestTool support.</param>
        /// <param name="promptForMissingArguments">
        /// Prompt users for missing arguments.<br/>
        /// Default is True.
        /// </param>
        /// <param name="argumentPromptTextOverride">Override the default prompt text format.</param>
        /// <param name="argumentFilter">
        /// Filter the arguments that will be prompted. i.e. Create a [PromptWhenMissing] attribute, or only prompt for operands.<br/>
        /// Default filter includes only arguments where <see cref="IArgumentArity.Minimum"/> is greater than zero.
        /// </param>
        public static AppRunner UsePrompting(
            this AppRunner appRunner,
            Func<CommandContext, IPrompter>? prompterOverride = null,
            bool promptForMissingArguments = true,
            Func<CommandContext, IArgument, string>? argumentPromptTextOverride = null,
            Predicate<IArgument>? argumentFilter = null)
        {
            return ValuePromptMiddleware.UsePrompting(appRunner, prompterOverride, promptForMissingArguments, argumentPromptTextOverride, argumentFilter);
        }

        /// <summary>Prefix a filepath with @ and it will be replaced by its contents during <see cref="MiddlewareStages.Tokenize"/></summary>
        /// <param name="appRunner">The <see cref="AppRunner"/></param>
        /// <param name="tokenTransformationOrder">
        /// The order response files token transformation should be run in relation to other <see cref="TokenTransformation"/>s.<br/>
        /// The default is 1. Change this if other transformations should be run before it.
        /// </param>
        public static AppRunner UseResponseFiles(this AppRunner appRunner, int tokenTransformationOrder = 1)
        {
            return ExpandResponseFilesTransformation.UseResponseFiles(appRunner, tokenTransformationOrder);
        }

        /// <summary>
        /// Sets <see cref="CommandContext.CancellationToken"/> and cancels the token on
        /// <see cref="Console.CancelKeyPress"/>, <see cref="AppDomain.ProcessExit"/> and
        /// <see cref="AppDomain.UnhandledException"/> if <see cref="UnhandledExceptionEventArgs.IsTerminating"/> is true.<br/>
        /// Once cancelled, the pipelines will not progress to the next step.
        /// </summary>
        public static AppRunner UseCancellationHandlers(this AppRunner appRunner)
        {
            return CancellationMiddleware.UseCancellationHandlers(appRunner);
        }

        /// <summary>
        /// When <see cref="AppSettingAttribute"/> is present, looks for the key in the provided appSettings configs.
        /// </summary>
        /// <param name="appRunner">The <see cref="AppRunner"/></param>
        /// <param name="appSettings">the provided appSettings configs</param>
        /// <param name="includeNamingConventions">
        /// when true, also uses command and argument names as keys.<br/>
        /// command_Name.--option_longName, command_Name.-option_shortName or command_Name.operand_Name<br/>
        /// command_Name can be excluded too and the config will be applied to all arguments matching the name.
        /// </param>
        public static AppRunner UseDefaultsFromAppSetting(this AppRunner appRunner, NameValueCollection appSettings, bool includeNamingConventions = false)
        {
            return includeNamingConventions
                ? appRunner.UseDefaultsFromConfig(
                    DefaultSources.AppSetting.GetDefaultValue(appSettings, 
                        DefaultSources.AppSetting.GetKeyFromAttribute, 
                        DefaultSources.AppSetting.GetKeysFromConvention))
                : appRunner.UseDefaultsFromConfig(
                    DefaultSources.AppSetting.GetDefaultValue(appSettings, DefaultSources.AppSetting.GetKeyFromAttribute));
        }

        /// <summary>
        /// When <see cref="EnvVarAttribute"/> is present, looks for the key in the provided envVars config.<br/>
        /// If envVars is not provided the default <see cref="Environment.GetEnvironmentVariables()"/>
        /// </summary>
        public static AppRunner UseDefaultsFromEnvVar(this AppRunner appRunner, IDictionary? envVars = null)
        {
            return appRunner.UseDefaultsFromConfig(
                DefaultSources.EnvVar.GetDefaultValue(envVars, DefaultSources.EnvVar.GetKeyFromAttribute));
        }

        /// <summary>Provide your own strategies for setting argument defaults from a configuration source</summary>
        public static AppRunner UseDefaultsFromConfig(this AppRunner appRunner,
            params Func<IArgument, ArgumentDefault?>[] getDefaultValueCallbacks)
            => SetArgumentDefaultsMiddleware.SetArgumentDefaultsFrom(appRunner, getDefaultValueCallbacks);

        /// <summary></summary>
        /// <param name="appRunner">The <see cref="AppRunner"/></param>
        /// <param name="writerFactory">If null, `cmdlog` directive is enabled without Console.Out as the target</param>
        /// <param name="excludeSystemInfo">Exclude machine name, username, OS, .net version and tool version</param>
        /// <param name="includeAppConfig">Prints the entire app configuration</param>
        /// <param name="additionalInfoCallback">Additional information to include.</param>
        public static AppRunner UseCommandLogger(this AppRunner appRunner,
            Func<CommandContext, Action<string?>?>? writerFactory = null,
            bool excludeSystemInfo = false,
            bool includeAppConfig = false,
            Func<CommandContext, IEnumerable<(string key, string value)>?>? additionalInfoCallback = null)
        {
            return CommandLoggerMiddleware.UseCommandLogger(appRunner, 
                writerFactory, 
                !excludeSystemInfo,
                includeAppConfig, 
                additionalInfoCallback);
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
                throw new InvalidConfigurationException("Directives are not enabled. " +
                                                        $"{nameof(AppRunner)}.{nameof(AppRunner.AppSettings)}.{nameof(AppSettings.DisableDirectives)} " +
                                                        "must not be set to true");
            }
        }
    }
}