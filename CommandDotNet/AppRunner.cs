using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using CommandDotNet.ClassModeling;
using CommandDotNet.Diagnostics;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;
using CommandDotNet.Help;
using CommandDotNet.Logging;
using CommandDotNet.Parsing;
using CommandDotNet.Rendering;
using CommandDotNet.Tokens;

[assembly: InternalsVisibleTo("CommandDotNet.CommandLogger")]
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
    public class AppRunner : IIndentableToString
    {
        private readonly AppConfigBuilder _appConfigBuilder;

        public AppSettings AppSettings { get; }
        public Type RootCommandType { get; }

        internal AppConfig AppConfig { get; private set; }

        static AppRunner() => LogProvider.IsDisabled = true;

        public AppRunner(Type rootCommandType, AppSettings settings = null)
        {
            LogProvider.IsDisabled = true;

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
                return Execute(args).Result;
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
            var tokens = args.Tokenize(includeDirectives: !AppSettings.DisableDirectives);

            var appConfig = AppConfig ?? (AppConfig = _appConfigBuilder.Build());
            var commandContext = new CommandContext(args, tokens, appConfig);

            try
            {
                var result = await commandContext.AppConfig.MiddlewarePipeline
                    .InvokePipeline(commandContext).ConfigureAwait(false);

                appConfig.OnRunCompleted?.Invoke(new OnRunCompletedEventArgs(commandContext));

                return result;
            }
            catch (Exception ex)
            {
                ex.SetCommandContext(commandContext);
                throw;
            }
        }

        private void AddCoreMiddleware()
        {
            _appConfigBuilder
                .UseMiddleware(TokenizerPipeline.TokenizeInputMiddleware, MiddlewareSteps.Tokenize.Stage, MiddlewareSteps.Tokenize.Order)
                .UseMiddleware(CommandParser.ParseInputMiddleware, MiddlewareSteps.ParseInput.Stage, MiddlewareSteps.ParseInput.Order);

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

        public override string ToString()
        {
            return ToString(new Indent());
        }

        public string ToString(Indent indent)
        {
            return AppConfig == null
                ? $"{indent}{nameof(AppRunner)}<{RootCommandType.Name}>"
                : $"{indent}{nameof(AppRunner)}<{RootCommandType.Name}>:{Environment.NewLine}{indent.Increment()}{AppConfig.ToString(indent.IncrementBy(2))}";
        }
    }
}