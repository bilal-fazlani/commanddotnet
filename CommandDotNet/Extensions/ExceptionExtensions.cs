using System;
using System.Diagnostics;
using System.Reflection;
using CommandDotNet.Rendering;

namespace CommandDotNet.Extensions
{
    internal static class ExceptionExtensions
    {
        [Conditional("DEBUG")]
        internal static void PrintStackTrace(this Exception ex, IConsole console)
        {
            console.Error.WriteLine(ex.StackTrace);
        }

        internal static Exception EscapeWrappers(this Exception exception)
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