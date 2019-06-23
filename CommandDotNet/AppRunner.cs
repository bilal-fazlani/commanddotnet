using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using CommandDotNet.CommandInvoker;
using CommandDotNet.Exceptions;
using CommandDotNet.HelpGeneration;
using CommandDotNet.MicrosoftCommandLineUtils;
using CommandDotNet.Models;
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
                AppCreator appCreator = new AppCreator(_settings);

                CommandLineApplication app = appCreator.CreateApplication(typeof(T), DependencyResolver);

                return app.Execute(_parserBuilder.Build(), args);
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
                var optionHelp = e.Command.OptionHelp;
                if (optionHelp != null)
                {
                    _settings.Out.WriteLine(
                        $"Specify --{optionHelp.Name} for a list of available options and commands.");
                }

                _settings.Error.WriteLine(e.Message + "\n");
                e.Command.ShowHelp();

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

        public AppRunner<T> UseArgumentTransform(string name, int order, Func<Tokens, Tokens> transformation)
        {
            _parserBuilder.AddArgumentTransformation(name, order, transformation);
            return this;
        }
    }
}