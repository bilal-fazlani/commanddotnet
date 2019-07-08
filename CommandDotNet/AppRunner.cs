using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using CommandDotNet.Builders;
using CommandDotNet.ClassModeling;
using CommandDotNet.Directives;
using CommandDotNet.Help;
using CommandDotNet.Invocation;
using CommandDotNet.Parsing;

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
        private readonly ParserBuilder _parserBuilder = new ParserBuilder();

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
                return Execute(args);
            }
            catch (AppRunnerException e)
            {
                _settings.Error.WriteLine(e.Message + "\n");
#if DEBUG
                _settings.Error.WriteLine(e.StackTrace);
#endif
                return 1;
            }
            catch (CommandParsingException e)
            {
                _settings.Out.WriteLine(
                    $"Specify --{Constants.HelpArgumentTemplate.Name} for a list of available options and commands.");

                _settings.Error.WriteLine(e.Message + "\n");
                HelpOptionSource.Print(_settings, e.Command);

#if DEBUG
                _settings.Error.WriteLine(e.StackTrace);
#endif

                return 1;
            }
            catch (ValueParsingException e)
            {
                _settings.Error.WriteLine(e.Message + "\n");
                return 2;
            }
            catch (AggregateException e) when (e.InnerExceptions.Any(x => x.GetBaseException() is AppRunnerException) ||
                                               e.InnerExceptions.Any(x =>
                                                   x.GetBaseException() is CommandParsingException))
            {
                foreach (var innerException in e.InnerExceptions)
                {
                    _settings.Error.WriteLine(innerException.GetBaseException().Message + "\n");
#if DEBUG
                    _settings.Error.WriteLine(innerException.GetBaseException().StackTrace);
                    if (e.InnerExceptions.Count > 1)
                        _settings.Error.WriteLine("-----------------------------------------------------------------");
#endif
                }

                return 1;
            }
            catch (AggregateException e) when (e.InnerExceptions.Any(x =>
                x.GetBaseException() is ArgumentValidationException))

            {
                ArgumentValidationException validationException =
                    (ArgumentValidationException) e.InnerExceptions.FirstOrDefault(x =>
                        x.GetBaseException() is ArgumentValidationException);

                foreach (var failure in validationException.ValidationResult.Errors)
                {
                    _settings.Out.WriteLine(failure.ErrorMessage);
                }

                return 2;
            }
            catch (AggregateException e) when (e.InnerExceptions.Any(x => x.GetBaseException() is ValueParsingException)
            )

            {
                ValueParsingException valueParsingException =
                    (ValueParsingException) e.InnerExceptions.FirstOrDefault(x =>
                        x.GetBaseException() is ValueParsingException);

                _settings.Error.WriteLine(valueParsingException.Message + "\n");

                return 2;
            }
            catch (AggregateException e) when (e.InnerExceptions.Any(x => x is TargetInvocationException))
            {
                TargetInvocationException ex =
                    (TargetInvocationException) e.InnerExceptions.SingleOrDefault(x => x is TargetInvocationException);
                ExceptionDispatchInfo.Capture(ex.InnerException ?? ex).Throw();
                return 1; // this will never be called
            }
            catch (AggregateException e)
            {
                foreach (Exception innerException in e.InnerExceptions)
                {
                    ExceptionDispatchInfo.Capture(innerException).Throw();
                }

                return 1; // this will never be called if there is any inner exception
            }
            catch (ArgumentValidationException ex)
            {
                foreach (var failure in ex.ValidationResult.Errors)
                {
                    _settings.Out.WriteLine(failure.ErrorMessage);
                }

                return 2;
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException ?? ex).Throw();
                return 1; // this will never be called
            }
        }

        private int Execute(string[] args)
        {
            if (_settings.EnableDirectives)
            {
                _parserBuilder.AddMiddleware(DebugDirective.Execute, 0);
                _parserBuilder.AddMiddleware(ParseDirective.Execute, 1);
            }

            _parserBuilder.AddMiddleware(ExecuteTheRest, 1);


            var tokens = args.Tokenize(includeDirectives: _settings.EnableDirectives);
            var executionResult = new ExecutionResult(args, tokens, _settings);
            executionResult.ParserConfig = _parserBuilder.Build(executionResult);
            return InvokeMiddleware(executionResult);
        }

        private int ExecuteTheRest(ExecutionResult executionResult, Func<ExecutionResult, int> next)
        {
            new TokenizerPipeline().Tokenize(executionResult);
            if (executionResult.ShouldExit)
            {
                return executionResult.ExitCode;
            }

            AppCreator appCreator = new AppCreator(_settings);
            Command rootCommand = appCreator.CreateRootCommand(typeof(T), DependencyResolver);
            new CommandParser(_settings).ParseCommand(executionResult, rootCommand);
            if (executionResult.ShouldExit)
            {
                return executionResult.ExitCode;
            }

            return ((Command) executionResult.ParseResult.Command).Execute();
        }

        private static int InvokeMiddleware(ExecutionResult executionResult)
        {
            var middlewares = executionResult.ParserConfig.MiddlewarePipeline;

            var middlewareChain = middlewares.Aggregate(
                (first, second) =>
                    (ctx, next) =>
                        first(ctx,
                            c => second(c, next)));

            return middlewareChain(executionResult, ctx => ctx.ExitCode);
        }

        public AppRunner<T> WithCustomHelpProvider(IHelpProvider customHelpProvider)
        {
            _settings.CustomHelpProvider = customHelpProvider;
            return this;
        }

        public AppRunner<T> WithCommandInvoker(Func<ICommandInvoker, ICommandInvoker> commandInvokerProvider)
        {
            _settings.CommandInvoker = commandInvokerProvider(_settings.CommandInvoker);
            return this;
        }

        public AppRunner<T> OverrideConsoleOut(TextWriter writer)
        {
            _settings.Out = writer;
            return this;
        }

        public AppRunner<T> OverrideConsoleError(TextWriter writer)
        {
            _settings.Error = writer;
            return this;
        }

        public AppRunner<T> UseInputTransformation(string name, int order, Func<TokenCollection, TokenCollection> transformation)
        {
            _parserBuilder.AddInputTransformation(name, order, transformation);
            return this;
        }
    }
}