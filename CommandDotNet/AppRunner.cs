using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using CommandDotNet.CommandInvoker;
using CommandDotNet.Exceptions;
using CommandDotNet.HelpGeneration;
using CommandDotNet.MicrosoftCommandLineUtils;
using CommandDotNet.Models;

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

        public AppRunner(AppSettings settings = null)
        {
            _settings = settings ?? new AppSettings();
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

                var parsedArguments = ArgumentParser.SplitFlags(args).ToArray();

                return app.Execute(parsedArguments);
            }
            catch (AppRunnerException e)
            {
                Console.Error.WriteLine(e.Message + "\n");
#if DEBUG
                Console.Error.WriteLine(e.StackTrace);
#endif
                return 1;
            }
            catch (CommandParsingException e)
            {
                Console.Error.WriteLine(e.Message + "\n");
                e.Command.ShowHelp();

#if DEBUG
                Console.Error.WriteLine(e.StackTrace);
#endif

                return 1;
            }
            catch (ValueParsingException e)
            {
                Console.Error.WriteLine(e.Message + "\n");
                return 2;
            }
            catch (AggregateException e) when (e.InnerExceptions.Any(x => x.GetBaseException() is AppRunnerException) ||
                                               e.InnerExceptions.Any(x => x.GetBaseException() is CommandParsingException))
            {
                foreach (var innerException in e.InnerExceptions)
                {
                    Console.Error.WriteLine(innerException.GetBaseException().Message + "\n");
#if DEBUG
                    Console.Error.WriteLine(innerException.GetBaseException().StackTrace);
                    if (e.InnerExceptions.Count > 1)
                        Console.Error.WriteLine("-----------------------------------------------------------------");
#endif
                }

                return 1;
            }
            catch (AggregateException e) when (e.InnerExceptions.Any(x => x.GetBaseException() is ArgumentValidationException))

            {
                ArgumentValidationException validationException =
                    (ArgumentValidationException)e.InnerExceptions.FirstOrDefault(x => x.GetBaseException() is ArgumentValidationException);
                
                foreach (var failure in validationException.ValidationResult.Errors)
                {
                    Console.WriteLine(failure.ErrorMessage);
                }

                return 2;
            }
            catch (AggregateException e) when (e.InnerExceptions.Any(x => x.GetBaseException() is ValueParsingException))

            {
                ValueParsingException valueParsingException =
                    (ValueParsingException)e.InnerExceptions.FirstOrDefault(x => x.GetBaseException() is ValueParsingException);
                
                Console.Error.WriteLine(valueParsingException.Message + "\n");

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
                    Console.WriteLine(failure.ErrorMessage);
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
    }
}