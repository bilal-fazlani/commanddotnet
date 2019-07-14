using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using CommandDotNet.Builders;
using CommandDotNet.ClassModeling;
using CommandDotNet.ClassModeling.Definitions;
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
    /// Creates a new instance of AppRunner
    /// </summary>
    /// <typeparam name="T">Type of the application</typeparam>
    public class AppRunner<T> where T : class
    {
        internal IDependencyResolver DependencyResolver;

        private readonly AppSettings _settings;
        private readonly ExecutionBuilder _executionBuilder = new ExecutionBuilder();

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
            return RunAsync(args).Result;
        }

        /// <summary>
        /// Executes the specified command with given parameters
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <returns>If target method returns int, this method will return that value. Else, 
        /// it will return 0 in case of success and 1 in case of unhandled exception</returns>
        public Task<int> RunAsync(params string[] args)
        {
            try
            {
                return Execute(args);
            }
            catch (AppRunnerException e)
            {
                _settings.Console.Error.WriteLine(e.Message + "\n");
#if DEBUG
                _settings.Console.Error.WriteLine(e.StackTrace);
#endif
                return Task.FromResult(1);
            }
            catch (CommandParsingException e)
            {
                _settings.Console.Out.WriteLine(
                    $"Specify --{Constants.HelpArgumentTemplate.Name} for a list of available options and commands.");

                _settings.Console.Error.WriteLine(e.Message + "\n");
                HelpMiddleware.Print(_settings, e.Command);

#if DEBUG
                _settings.Console.Error.WriteLine(e.StackTrace);
#endif

                return Task.FromResult(1);
            }
            catch (ValueParsingException e)
            {
                _settings.Console.Error.WriteLine(e.Message + "\n");
                return Task.FromResult(2);
            }
            catch (AggregateException e) when (e.InnerExceptions.Any(x => x.GetBaseException() is AppRunnerException) ||
                                               e.InnerExceptions.Any(x =>
                                                   x.GetBaseException() is CommandParsingException))
            {
                foreach (var innerException in e.InnerExceptions)
                {
                    _settings.Console.Error.WriteLine(innerException.GetBaseException().Message + "\n");
#if DEBUG
                    _settings.Console.Error.WriteLine(innerException.GetBaseException().StackTrace);
                    if (e.InnerExceptions.Count > 1)
                        _settings.Console.Error.WriteLine("-----------------------------------------------------------------");
#endif
                }

                return Task.FromResult(1);
            }
            catch (AggregateException e) when (e.InnerExceptions.Any(x =>
                x.GetBaseException() is ArgumentValidationException))

            {
                ArgumentValidationException validationException =
                    (ArgumentValidationException) e.InnerExceptions.FirstOrDefault(x =>
                        x.GetBaseException() is ArgumentValidationException);

                foreach (var failure in validationException.ValidationResult.Errors)
                {
                    _settings.Console.Out.WriteLine(failure.ErrorMessage);
                }

                return Task.FromResult(2);
            }
            catch (AggregateException e) when (e.InnerExceptions.Any(x => x.GetBaseException() is ValueParsingException)
            )

            {
                ValueParsingException valueParsingException =
                    (ValueParsingException) e.InnerExceptions.FirstOrDefault(x =>
                        x.GetBaseException() is ValueParsingException);

                _settings.Console.Error.WriteLine(valueParsingException.Message + "\n");

                return Task.FromResult(2);
            }
            catch (AggregateException e) when (e.InnerExceptions.Any(x => x is TargetInvocationException))
            {
                TargetInvocationException ex =
                    (TargetInvocationException) e.InnerExceptions.SingleOrDefault(x => x is TargetInvocationException);
                ExceptionDispatchInfo.Capture(ex.InnerException ?? ex).Throw();
                return Task.FromResult(1); // this will never be called
            }
            catch (AggregateException e)
            {
                foreach (Exception innerException in e.InnerExceptions)
                {
                    ExceptionDispatchInfo.Capture(innerException).Throw();
                }

                return Task.FromResult(1); // this will never be called if there is any inner exception
            }
            catch (ArgumentValidationException ex)
            {
                foreach (var failure in ex.ValidationResult.Errors)
                {
                    _settings.Console.Out.WriteLine(failure.ErrorMessage);
                }

                return Task.FromResult(2);
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException ?? ex).Throw();
                return Task.FromResult(1); // this will never be called
            }
        }

        private Task<int> Execute(string[] args)
        {
            if (_settings.EnableDirectives)
            {
                _executionBuilder.AddMiddlewareInStage(DebugDirective.DebugMiddleware, MiddlewareStages.Configuration, 0);
                _executionBuilder.AddMiddlewareInStage(ParseDirective.ParseMiddleware, MiddlewareStages.Tokenize, -2);
            }

            _executionBuilder.AddMiddlewareInStage(TokenizerPipeline.TokenizeMiddleware, MiddlewareStages.Tokenize, -1);
            _executionBuilder.AddMiddlewareInStage(BuildMiddleware, MiddlewareStages.Building);
            _executionBuilder.AddMiddlewareInStage(ParseMiddleware, MiddlewareStages.Parsing);
            if (_settings.PromptForMissingOperands)
            {
                _executionBuilder.AddMiddlewareInStage(ValuePromptMiddleware.PromptForMissingOperands, MiddlewareStages.Parsing, 300);
            }

            // TODO: add middleware between stages to validate CommandContext is exiting a stage with required data populated
            //       i.e. ParseResult should be fully populated after Parse stage

            _executionBuilder.AddMiddlewareInStage(SetInvocationContextMiddleware, MiddlewareStages.Parsing, 400);
            _executionBuilder.AddMiddlewareInStage(SetValuesMiddleware, MiddlewareStages.Parsing, 500);
            _executionBuilder.AddMiddlewareInStage(ValidateModelsMiddleware, MiddlewareStages.Parsing, 600);
            _executionBuilder.AddMiddlewareInStage(CreateInstancesMiddleware, MiddlewareStages.Invocation, -500);
            _executionBuilder.AddMiddlewareInStage(InjectDependenciesMiddleware, MiddlewareStages.Invocation, -400);
            _executionBuilder.AddMiddlewareInStage(InvokeCommandDefMiddleware, MiddlewareStages.Invocation, int.MaxValue);

            _executionBuilder.UseHelpMiddleware(100);
            if (_settings.EnableVersionOption)
            {
                _executionBuilder.UseVersionMiddleware(200);
            }

            var tokens = args.Tokenize(includeDirectives: _settings.EnableDirectives);
            var executionResult = new CommandContext(args, tokens, _settings, _executionBuilder.Build(_settings, DependencyResolver));

            return InvokeMiddleware(executionResult);
        }

        private Task<int> BuildMiddleware(CommandContext commandContext, Func<CommandContext, Task<int>> next)
        {
            commandContext.CurrentCommand = ClassCommandDef.CreateRootCommand(typeof(T), commandContext);
            return next(commandContext);
        }

        private Task<int> ParseMiddleware(CommandContext commandContext, Func<CommandContext, Task<int>> next)
        {
            new CommandParser(_settings).ParseCommand(commandContext);
            return next(commandContext);
        }

        private Task<int> SetInvocationContextMiddleware(CommandContext commandContext, Func<CommandContext, Task<int>> next)
        {
            var commandDef = commandContext.CurrentCommand.ContextData.Get<ICommandDef>();
            if (commandDef != null)
            {
                var ctx = commandContext.InvocationContext;
                ctx.InstantiateInvocation = commandDef.InstantiateMethodDef;
                ctx.CommandInvocation = commandDef.InvokeMethodDef;
            }

            return next(commandContext);
        }

        private Task<int> SetValuesMiddleware(CommandContext commandContext, Func<CommandContext, Task<int>> next)
        {
            var commandDef = commandContext.CurrentCommand.ContextData.Get<ICommandDef>();
            if (commandDef != null)
            {
                var argumentValues = commandContext.ParseResult.ArgumentValues;
                var parserFactory = new ParserFactory(commandContext.AppSettings);

                // TODO: move to Context object
                var instantiateArgs = commandDef.InstantiateMethodDef.ArgumentDefs;
                var invokeArgs = commandDef.InvokeMethodDef.ArgumentDefs;
                foreach (var argumentDef in instantiateArgs.Union(invokeArgs))
                {
                    if (argumentValues.TryGetValues(argumentDef.Argument, out var values))
                    {
                        var parser = parserFactory.CreateInstance(argumentDef.Argument);
                        var value = parser.Parse(argumentDef.Argument, values);
                        argumentDef.SetValue(value);
                    }
                }
            }
            return next(commandContext);
        }


        private Task<int> ValidateModelsMiddleware(CommandContext commandContext, Func<CommandContext, Task<int>> next)
        {
            var commandDef = commandContext.CurrentCommand.ContextData.Get<ICommandDef>();
            if (commandDef != null)
            {
                var modelValidator = new ModelValidator(commandContext.ExecutionConfig.DependencyResolver);

                // TODO: move to Context object
                var instantiateValues = commandDef.InstantiateMethodDef.ParameterValues;
                var invokeValues = commandDef.InvokeMethodDef.ParameterValues;

                foreach (var model in instantiateValues.Union(invokeValues).OfType<IArgumentModel>())
                {
                    modelValidator.ValidateModel(model);
                }
            }
            return next(commandContext);
        }

        private async Task<int> CreateInstancesMiddleware(CommandContext commandContext, Func<CommandContext, Task<int>> next)
        {
            var command = commandContext.ParseResult.Command;
            var commandDef = command.ContextData.Get<ICommandDef>();

            if (commandDef != null)
            {
                commandContext.InvocationContext.Instance = commandDef.InstantiateMethodDef.Invoke(null);
            }

            try
            {
                return await next(commandContext);
            }
            finally
            {
                // TODO: remove this when the instance is managed by DI
                //       and we can move creation of instance into an
                //       internal implementation of IDependencyResolver
                if (commandContext.InvocationContext.Instance is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }

        private Task<int> InjectDependenciesMiddleware(CommandContext commandContext, Func<CommandContext, Task<int>> next)
        {
            var instance = commandContext.InvocationContext.Instance;
            var dependencyResolver = commandContext.ExecutionConfig.DependencyResolver;
            if (instance != null)
            {
                //detect injection properties
                var properties = instance.GetType().GetDeclaredProperties<InjectPropertyAttribute>().ToList();

                if (properties.Any())
                {
                    if (dependencyResolver != null)
                    {
                        foreach (var propertyInfo in properties)
                        {
                            propertyInfo.SetValue(instance, dependencyResolver.Resolve(propertyInfo.PropertyType));
                        }
                    }
                    else // there are some properties but there is no dependency resolver set
                    {
                        throw new AppRunnerException("Dependency resolver is not set for injecting properties. " +
                                                     "Please use an IoC framework'");
                    }
                }
            }

            return next(commandContext);
        }

        private Task<int> InvokeCommandDefMiddleware(CommandContext commandContext, Func<CommandContext, Task<int>> next)
        {
            var ctx = commandContext.InvocationContext;

            var result = ctx.CommandInvocation.Invoke(ctx.Instance);
            return GetResultCodeAsync(result, commandContext);
        }

        internal static async Task<int> GetResultCodeAsync(object value, CommandContext commandContext)
        {
            switch (value)
            {
                case Task<int> resultCodeTask:
                    return await resultCodeTask;
                case Task task:
                    await task;
                    return commandContext.ExitCode;
                case int resultCode:
                    return resultCode;
                case null:
                    return commandContext.ExitCode;
                default:
                    throw new NotSupportedException();
            }
        }


        private static Task<int> InvokeMiddleware(CommandContext commandContext)
        {
            var pipeline = commandContext.ExecutionConfig.MiddlewarePipeline;

            var middlewareChain = pipeline.Aggregate(
                (first, second) =>
                    (ctx, next) =>
                        first(ctx, c =>
                            ctx.ShouldExit ? Task.FromResult(ctx.ExitCode) : second(c, next)));

            return middlewareChain(commandContext, ctx => Task.FromResult(ctx.ExitCode));
        }

        public AppRunner<T> WithCustomHelpProvider(IHelpProvider customHelpProvider)
        {
            _settings.CustomHelpProvider = customHelpProvider;
            return this;
        }

        public AppRunner<T> AddMiddlewareInStage(ExecutionMiddleware middleware, MiddlewareStages stage, int orderWithinStage = 0)
        {
            _executionBuilder.AddMiddlewareInStage(middleware, stage, orderWithinStage);
            return this;
        }

        public AppRunner<T> OverrideConsole(IConsole console)
        {
            _settings.Console = console;
            return this;
        }

        public AppRunner<T> UseInputTransformation(string name, int order, Func<TokenCollection, TokenCollection> transformation)
        {
            _executionBuilder.AddInputTransformation(name, order, transformation);
            return this;
        }
    }
}