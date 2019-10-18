using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using CommandDotNet.ClassModeling;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;
using CommandDotNet.Help;
using CommandDotNet.Parsing;
using CommandDotNet.Rendering;
using CommandDotNet.Tokens;

[assembly: InternalsVisibleTo("CommandDotNet.Tests")]
[assembly: InternalsVisibleTo("CommandDotNet.TestTools")]

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
        private readonly AppConfigBuilder _appConfigBuilder;

        public AppSettings AppSettings { get; }
        public Type RootCommandType { get; }

        public AppRunner(Type rootCommandType, AppSettings settings = null)
        {
            RootCommandType = rootCommandType ?? throw new ArgumentNullException(nameof(rootCommandType));
            AppSettings = settings ?? new AppSettings();
            _appConfigBuilder = new AppConfigBuilder(AppSettings);
            AddCoreMiddleware();
        }

        public AppRunner Configure(Action<AppConfigBuilder> configureCallback)
        {
            configureCallback(_appConfigBuilder);
            return this;
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
                return await Execute(args).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                return HandleException(e, _appConfigBuilder.Console);
            }
        }

        private async Task<int> Execute(string[] args)
        {
            var tokens = args.Tokenize(includeDirectives: AppSettings.EnableDirectives);
            
            var appConfig = _appConfigBuilder.Build();
            var commandContext = new CommandContext(args, tokens, appConfig);

            var result = await commandContext.AppConfig.MiddlewarePipeline
                .InvokePipeline(commandContext).ConfigureAwait(false);

            appConfig.OnRunCompleted?.Invoke(new OnRunCompletedEventArgs(commandContext));

            return result;
        }

        private void AddCoreMiddleware()
        {
            _appConfigBuilder
                .UseMiddleware(TokenizerPipeline.TokenizeInputMiddleware, MiddlewareStages.Tokenize, -1)
                .UseMiddleware(CommandParser.ParseInputMiddleware, MiddlewareStages.ParseInput);

            this.UseClassDefMiddleware(RootCommandType)
                .UseHelpMiddleware();

            // TODO: add middleware between stages to validate CommandContext is exiting a stage with required data populated
            //       i.e. ParseResult should be fully populated after Parse stage
            //            Invocation contexts should be fully populated after BindValues stage
            //            (when ctor options are moved to a middleware method, invocation context should be populated in Parse stage)
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
    }
}