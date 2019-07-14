using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.ExceptionServices;
using CommandDotNet.ClassModeling;
using CommandDotNet.Parsing;
using CommandDotNet.Rendering;

namespace CommandDotNet.Extensions
{
    internal static class ExceptionExtensions
    {
        public static int HandleException(this Exception ex, IConsole console, Action<ICommand> printHelp)
        {
            ex = ex.EscapeWrappers();
            switch (ex)
            {
                case AppRunnerException appEx:
                    console.Error.WriteLine(appEx.Message);
                    PrintStackTrace(appEx, console);
                    console.Error.WriteLine();
                    return 1;
                case CommandParsingException cpEx:
                    console.Error.WriteLine(cpEx.Message);
                    PrintStackTrace(cpEx, console);
                    console.Error.WriteLine();
                    printHelp(cpEx.Command);
                    return 1;
                case ArgumentValidationException avEx:
                    foreach (var error in avEx.ValidationResult.Errors)
                    {
                        console.Out.WriteLine(error.ErrorMessage);
                    }
                    console.Error.WriteLine();
                    return 2;
                case ValueParsingException vpEx:
                    console.Error.WriteLine(vpEx.Message);
                    console.Error.WriteLine();
                    return 2;
                case AggregateException aggEx:
                    ExceptionDispatchInfo.Capture(aggEx).Throw();
                    return 1; // this will only be called if there are no inner exceptions
                default:
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    return 1; // this will only be called if there are no inner exceptions
            }

            return 0;
        }

        [Conditional("DEBUG")]
        private static void PrintStackTrace(Exception ex, IConsole console)
        {
            console.Error.WriteLine(ex.StackTrace);
        }

        public static Exception EscapeWrappers(this Exception exception)
        {
            if (exception is AggregateException aggEx)
            {
                exception = aggEx.GetBaseException();
                if (exception is AggregateException)
                {
                    // will be AggregateException where there are multiple inner exceptions
                    return exception;
                }
            }

            if (exception is TargetInvocationException tie)
            {
                return tie.InnerException == null 
                    ? tie 
                    : EscapeWrappers(tie.InnerException);
            }

            return exception;
        }
    }
}