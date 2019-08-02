using System;
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
using CommandDotNet.Tokens;

[assembly: InternalsVisibleTo("CommandDotNet.Tests")]

namespace CommandDotNet
{
    /// <summary>
    /// AppRunner is the entry class for this library.<br/>
    /// Use this class to define settings and behaviors
    /// and to execute the Type that defines your commands.<br/>
    /// This is a convenience class for simpler declaration of RootCommandType
    /// </summary>
    /// <typeparam name="TRootCommandType">Type of the application</typeparam>
    public class AppRunner<TRootCommandType> : AppRunner where TRootCommandType : class
    {
        public AppRunner(AppSettings settings = null) : base(typeof(TRootCommandType), settings) { }
    }

    /// <summary>
    /// AppRunner is the entry class for this library.
    /// Use this class to define settings and behaviors
    /// and to execute the Type that defines your commands.
    /// </summary>
    public class AppRunner
    {
        private readonly Type _rootCommandType;
        private readonly AppSettings _settings;
        private readonly AppConfigBuilder _appConfigBuilder = new AppConfigBuilder(); 

        public AppRunner(Type rootCommandType, AppSettings settings = null)
        {
            _rootCommandType = rootCommandType ?? throw new ArgumentNullException(nameof(rootCommandType));
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
                return HandleException(e, _appConfigBuilder.Console);
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
                return HandleException(e, _appConfigBuilder.Console);
            }
        }

        private Task<int> Execute(string[] args)
        {
            AddCoreMiddleware();
            AddOptionalMiddleware();

            var tokens = args.Tokenize(includeDirectives: _settings.EnableDirectives);
            var commandContext = new CommandContext(
                args, tokens, _settings,
                _appConfigBuilder.Build(_settings));

            return InvokeMiddleware(commandContext);
        }

        private void AddCoreMiddleware()
        {
            _appConfigBuilder
                .UseMiddleware(TokenizerPipeline.TokenizeMiddleware, MiddlewareStages.TransformTokens, -1)
                .UseMiddleware(CommandParser.ParseMiddleware, MiddlewareStages.ParseInput);

            this.UseClassDefMiddleware(_rootCommandType)
                .UseHelpMiddleware();

            // TODO: add middleware between stages to validate CommandContext is exiting a stage with required data populated
            //       i.e. ParseResult should be fully populated after Parse stage
            //            Invocation contexts should be fully populated after BindValues stage
            //            (when ctor options are moved to a middleware method, invocation context should be populated in Parse stage)
        }

        private void AddOptionalMiddleware()
        {
            if (_settings.EnableDirectives)
            {
                this.UseDebugDirective()
                    .UseParseDirective();
            }

            if (_settings.EnableVersionOption)
            {
                this.UseVersionMiddleware();
            }

            if (_settings.PromptForMissingOperands)
            {
                _appConfigBuilder.UseMiddleware(ValuePromptMiddleware.PromptForMissingOperands,
                    MiddlewareStages.PostParseInputPreBindValues);
            }

            // TODO: move FluentValidation into a separate repo & nuget package?
            //       there are other ways to do validation that could also
            //       be applied to parameters
            _appConfigBuilder.UseMiddleware(ModelValidator.ValidateModelsMiddleware,
                MiddlewareStages.PostBindValuesPreInvoke);

            _appConfigBuilder.UseMiddleware(DependencyResolveMiddleware.InjectDependencies,
                MiddlewareStages.PostBindValuesPreInvoke);
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
            var pipeline = commandContext.AppConfig.MiddlewarePipeline;

            var middlewareChain = pipeline.Aggregate(
                (first, second) =>
                    (ctx, next) =>
                        first(ctx, c => second(c, next)));

            return middlewareChain(commandContext, ctx => Task.FromResult(0));
        }

        public AppRunner Configure(Action<AppConfigBuilder> configureCallback)
        {
            configureCallback(_appConfigBuilder);
            return this;
        }

        /// <summary>
        /// Configures the app to use the resolver to create instances of
        /// properties decorated with <see cref="InjectPropertyAttribute"/>
        /// </summary>
        public AppRunner UseDependencyResolver(IDependencyResolver dependencyResolver)
        {
            _appConfigBuilder.UseDependencyResolver(dependencyResolver);
            return this;
        }
    }
}