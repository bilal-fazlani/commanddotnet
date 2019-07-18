using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using CommandDotNet.Builders;
using CommandDotNet.ClassModeling;
using CommandDotNet.Directives;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;
using CommandDotNet.Help;
using CommandDotNet.Parsing;
using CommandDotNet.Rendering;

[assembly: InternalsVisibleTo("CommandDotNet.Tests")]

namespace CommandDotNet
{
    /// <summary>
    /// AppRunner is the entry class for this library.
    /// Use this class to define settings and behaviors
    /// and to execute the Type that defines your commands.
    /// </summary>
    /// <typeparam name="T">Type of the application</typeparam>
    public class AppRunner<T> where T : class
    {
        internal IDependencyResolver DependencyResolver;

        private readonly AppSettings _settings;
        private readonly AppBuilder _appBuilder = new AppBuilder();

        public AppRunner(AppSettings settings = null)
        {
            _settings = settings ?? new AppSettings();

            // TODO: add .rsp file transformation as an extension option
        }

        /// <summary>
        /// Executes the specified command with given parameters
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <returns>If target method returns int, this method will return that value. Else, 
        /// it will return 0 in case of success and 1 in case of unhandled exception</returns>
        public int Run(params string[] args)
        {
            try
            {
                return RunAsync(args).Result;
            }
            catch (Exception e)
            {
                return HandleException(e, _settings.Console);
            }
        }

        /// <summary>
        /// Executes the specified command with given parameters
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <returns>If target method returns int, this method will return that value. Else, 
        /// it will return 0 in case of success and 1 in case of unhandled exception</returns>
        public async Task<int> RunAsync(params string[] args)
        {
            try
            {
                return await Execute(args);
            }
            catch (Exception e)
            {
                return HandleException(e, _settings.Console);
            }
        }

        private Task<int> Execute(string[] args)
        {
            AddCoreMiddleware();
            AddOptionalMiddleware();

            var tokens = args.Tokenize(includeDirectives: _settings.EnableDirectives);
            var commandContext = new CommandContext(
                args, tokens, _settings,
                _appBuilder.Build(_settings, DependencyResolver));

            return InvokeMiddleware(commandContext);
        }

        private void AddCoreMiddleware()
        {
            _appBuilder.AddMiddlewareInStage(TokenizerPipeline.TokenizeMiddleware, MiddlewareStages.TransformInput, -1);
            _appBuilder.AddMiddlewareInStage(CommandParser.ParseMiddleware, MiddlewareStages.ParseInput);
            _appBuilder.UseClassDefMiddleware<T>();
            _appBuilder.UseHelpMiddleware();

            // TODO: add middleware between stages to validate CommandContext is exiting a stage with required data populated
            //       i.e. ParseResult should be fully populated after Parse stage
            //            Invocation contexts should be fully populated after BindValues stage
            //            (when ctor options are moved to a middleware method, invocation context should be populated in Parse stage)
        }

        private void AddOptionalMiddleware()
        {
            if (_settings.EnableDirectives)
            {
                _appBuilder.UseDebugDirective();
                _appBuilder.UseParseDirective();
            }

            if (_settings.EnableVersionOption)
            {
                _appBuilder.UseVersionMiddleware();
            }

            if (_settings.PromptForMissingOperands)
            {
                _appBuilder.AddMiddlewareInStage(ValuePromptMiddleware.PromptForMissingOperands,
                    MiddlewareStages.PostParseInputPreBindValues);
            }

            // TODO: move FluentValidation into a separate repo & nuget package?
            //       there are other ways to do validation that could also
            //       be applied to parameters
            _appBuilder.AddMiddlewareInStage(ModelValidator.ValidateModelsMiddleware,
                MiddlewareStages.PostBindValuesPreInvoke);

            _appBuilder.EnablePipedInput();

            if (DependencyResolver != null)
            {
                _appBuilder.AddMiddlewareInStage(DependencyResolveMiddleware.InjectDependencies,
                    MiddlewareStages.PostBindValuesPreInvoke);
            }
        }
        
        private static int HandleException(Exception ex, IConsole console)
        {
            ex = ex.EscapeWrappers();
            switch (ex)
            {
                case AppRunnerException appEx:
                    console.Error.WriteLine(appEx.Message);
                    appEx.PrintStackTrace(console);
                    console.Error.WriteLine();
                    return 1;
                case AggregateException aggEx:
                    ExceptionDispatchInfo.Capture(aggEx).Throw();
                    return 1; // this will only be called if there are no inner exceptions
                default:
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    return 1; // this will only be called if there are no inner exceptions
            }
        }

        private static Task<int> InvokeMiddleware(CommandContext commandContext)
        {
            var pipeline = commandContext.ExecutionConfig.MiddlewarePipeline;

            var middlewareChain = pipeline.Aggregate(
                (first, second) =>
                    (ctx, next) =>
                        first(ctx, c => second(c, next)));

            return middlewareChain(commandContext, ctx => Task.FromResult(0));
        }

        /// <summary>
        /// Replace the internal help provider with given help provider
        /// </summary>
        public AppRunner<T> UseCustomHelpProvider(IHelpProvider customHelpProvider)
        {
            _settings.CustomHelpProvider = customHelpProvider;
            return this;
        }

        /// <summary>
        /// Adds the middleware to the pipeline in the specified <see cref="MiddlewareStages"/>.
        /// Use <see cref="orderWithinStage"/> to specify order in relation
        /// to other middleware within the same stage.
        /// </summary>
        public AppRunner<T> UseMiddleware(ExecutionMiddleware middleware, MiddlewareStages stage,
            int? orderWithinStage = null)
        {
            _appBuilder.AddMiddlewareInStage(middleware, stage, orderWithinStage);
            return this;
        }

        /// <summary>
        /// Adds the transformation to the list of transformations applied to tokens
        /// before they are parsed into commands and arguments
        /// </summary>
        public AppRunner<T> UseInputTransformation(string name, int order,
            Func<TokenCollection, TokenCollection> transformation)
        {
            _appBuilder.AddInputTransformation(name, order, transformation);
            return this;
        }

        /// <summary>
        /// Configures the app to use the resolver to create instances of
        /// properties decorated with <see cref="InjectPropertyAttribute"/>
        /// </summary>
        public AppRunner<T> UseDependencyResolver(IDependencyResolver dependencyResolver)
        {
            DependencyResolver = dependencyResolver;
            return this;
        }

        /// <summary>Replace the internal system console with provided console</summary>
        public AppRunner<T> UseConsole(IConsole console)
        {
            _settings.Console = console;
            return this;
        }

        [Obsolete("Use UseCustomHelpProvider instead")]
        public AppRunner<T> WithCustomHelpProvider(IHelpProvider customHelpProvider)
        {
            return UseCustomHelpProvider(customHelpProvider);
        }
    }
}