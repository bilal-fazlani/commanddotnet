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
        public AppRunner(
            AppSettings? settings = null, 
            Resources? resourcesOverride = null) 
            : base(typeof(TRootCommandType), settings, resourcesOverride) { }
    }

    /// <summary>
    /// AppRunner is the entry class for this library.
    /// Use this class to define settings and behaviors
    /// and to execute the Type that defines your commands.
    /// </summary>
    public class AppRunner : IIndentableToString
    {
        private readonly AppConfigBuilder _appConfigBuilder;
        private HandleErrorDelegate? _handleErrorDelegate;
        
        internal AppConfig? AppConfig { get; private set; }

        public AppSettings AppSettings { get; }
        public Type RootCommandType { get; }

        static AppRunner() => LogProvider.IsDisabled = true;

        public AppRunner(Type rootCommandType, 
            AppSettings? settings = null, 
            Resources? resourcesOverride = null)
        {
            LogProvider.IsDisabled = true;

            RootCommandType = rootCommandType ?? throw new ArgumentNullException(nameof(rootCommandType));
            AppSettings = settings ?? new AppSettings();
            
            var localizationAppSettings = AppSettings.Localization;
            if (resourcesOverride != null)
            {
                Resources.A = resourcesOverride;
            }
            else if (localizationAppSettings.Localize != null)
            {
                Resources.A = new ResourcesProxy(
                    localizationAppSettings.Localize, 
                    localizationAppSettings.UseMemberNamesAsKeys);
            }
            
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
            CommandContext? commandContext = null;
            try
            {
                commandContext = BuildCommandContext(args);
                return commandContext.AppConfig.MiddlewarePipeline
                    .InvokePipeline(commandContext)
                    .Result;
            }
            catch (Exception e)
            {
                return HandleException(e, commandContext);
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
            CommandContext? commandContext = null;
            try
            {
                commandContext = BuildCommandContext(args);
                return await commandContext.AppConfig.MiddlewarePipeline
                    .InvokePipeline(commandContext)
                    .ConfigureAwait(false);
            }
            catch (Exception e)
            {
                return HandleException(e, commandContext);
            }
        }

        public AppRunner UseErrorHandler(HandleErrorDelegate handleError)
        {
            _handleErrorDelegate = handleError;
            return this;
        }

        private CommandContext BuildCommandContext(string[] args)
        {
            var tokens = args.Tokenize(includeDirectives: !AppSettings.DisableDirectives);
            var appConfig = AppConfig ??= _appConfigBuilder.Build();
            var commandContext = new CommandContext(args, tokens, appConfig);
            return commandContext;
        }

        private void AddCoreMiddleware()
        {
            _appConfigBuilder
                .UseMiddleware(OnRunCompleted, MiddlewareSteps.OnRunCompleted)
                .UseMiddleware(TokenizerPipeline.TokenizeInputMiddleware, MiddlewareSteps.Tokenize)
                .UseMiddleware(CommandParser.ParseInputMiddleware, MiddlewareSteps.ParseInput);

            this.UseClassDefMiddleware(RootCommandType)
                .UseHelpMiddleware()
                .UseArityValidation()
                .AppendPipedInput();

            // TODO: add middleware between stages to validate CommandContext is exiting a stage with required data populated
            //       i.e. ParseResult should be fully populated after Parse stage
            //            Invocation contexts should be fully populated after BindValues stage
            //            (when ctor options are moved to a middleware method, invocation context should be populated in Parse stage)
        }

        private Task<int> OnRunCompleted(CommandContext context, ExecutionDelegate next)
        {
            var result = next(context);
            context.AppConfig.OnRunCompleted?.Invoke(new OnRunCompletedEventArgs(context));
            return result;
        }

        private int HandleException(Exception ex, CommandContext? commandContext)
        {
            ex = ex.EscapeWrappers();

            var console = commandContext?.Console ?? AppConfig?.Console ?? new SystemConsole();

            if (ex.IsFor<ValueParsingException>(out var vpe))
            {
                console.Error.WriteLine(vpe!.Print(excludeTypeName: true));
                return ExitCodes.Error.Result;
            }
            if (ex.IsFor<InvalidConfigurationException>(out var ice))
            {
                console.Error.WriteLine(ice!.Print());
                return ExitCodes.Error.Result;
            }
            if (_handleErrorDelegate != null)
            {
                return _handleErrorDelegate(commandContext, ex);
            }
            if (commandContext is not null)
            {
                ex.SetCommandContext(commandContext);
            }

            ExceptionDispatchInfo.Capture(ex).Throw();

            // code not reached but required to compile
            // compiler does not realize previous line throws an exception
            return ExitCodes.Error.Result;
        }

        public override string ToString()
        {
            return ToString(new Indent());
        }

        public string ToString(Indent indent)
        {
            return AppConfig == null
                ? $"{indent}{nameof(AppRunner)}<{RootCommandType.Name}>"
                : $"{indent}{nameof(AppRunner)}<{RootCommandType.Name}>:{Environment.NewLine}{indent.Increment()}{AppConfig.ToString(indent.Increment())}";
        }
    }
}