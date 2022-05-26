using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;
using CommandDotNet.Extensions;
using static System.Environment;

namespace CommandDotNet.Diagnostics
{
    public static class ExceptionExtensions
    {
        internal static void SetCommandContext(this Exception ex, CommandContext ctx)
        {
            ex.Data[nameof(CommandContext)] = new NonSerializableWrapper(ctx, skipPrint: true);
        }

        internal static void RemoveCommandContext(this Exception ex)
        {
            if (ex.Data.Contains(typeof(CommandContext)))
            {
                ex.Data.Remove(typeof(CommandContext));
            }
        }

        public static CommandContext? GetCommandContext(this Exception ex)
        {
            return ex.Data.GetValueOrDefault<NonSerializableWrapper>(nameof(CommandContext))?.As<CommandContext>() 
                   ?? ex.InnerException?.GetCommandContext();
        }

        public static bool IsFor<TEx>(this Exception exception, out TEx? ex)
        {
            if (exception is TEx tex)
            {
                ex = tex;
                return true;
            }

            if (exception.InnerException is null)
            {
                ex = default(TEx);
                return false;
            }

            return exception.InnerException.IsFor(out ex);
        }

        public static string Print(this Exception ex, Indent? indent = null,
            bool includeProperties = false, bool includeData = false, bool includeStackTrace = false,
            bool excludeTypeName = false)
        {
            var sb = new StringBuilder();
            ex.Print(line => sb.AppendLine(line), 
                indent, includeProperties, includeData, includeStackTrace, excludeTypeName);
            // trim trailing new line
            sb.Length = sb.Length - NewLine.Length;
            return sb.ToString();
        }

        public static void Print(this Exception ex, IConsole console, Indent? indent = null,
            bool includeProperties = false, bool includeData = false, bool includeStackTrace = false,
            bool excludeTypeName = false)
        {
            ex.Print(line => console.Error.WriteLine(line), 
                indent, includeProperties, includeData, includeStackTrace, excludeTypeName);
        }
        
        public static void Print(this Exception ex, Action<string?> writeLine, Indent? indent = null, 
            // begin-snippet: exception_print_parameters
            bool includeProperties = false,  // print exception properties
            bool includeData = false,        // print values from ex.Data dictionary
            bool includeStackTrace = false,  // print stack trace
            bool excludeTypeName = false     // do not print exception type name
            // end-snippet
            )
        {
            if (ex is null)
            {
                throw new ArgumentNullException(nameof(ex));
            }

            indent ??= new Indent();

            writeLine(excludeTypeName
                ? $"{indent}{ex.Message}"
                : $"{indent}{ex.GetType().FullName}: {ex.Message}");

            if (includeProperties)
            {
                var properties = ex.GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(p => p.DeclaringType != typeof(Exception))
                    .ToList();
                if (properties.Any())
                {
                    writeLine($"{indent}{Resources.A.Exceptions_Properties}:");
                    indent = indent.Increment();
                    foreach (var property in properties)
                    {
                        writeLine($"{indent}{property.Name}: {property.GetValue(ex).ToIndentedString(indent)}");
                    }
                    indent = indent.Decrement();
                }
            }

            if (includeData && ex.Data.Count > 0)
            {
                writeLine($"{indent}{Resources.A.Exceptions_Data}:");
                indent = indent.Increment();
                foreach (DictionaryEntry entry in ex.Data)
                {
                    var skip = entry.Value is NonSerializableWrapper { SkipPrint: true };
                    if (!skip)
                    {
                        writeLine($"{indent}{entry.Key}: {entry.Value.ToIndentedString(indent)}");
                    }
                }
                indent = indent.Decrement();
            }

            if (includeStackTrace && ex.StackTrace is { })
            {
                writeLine($"{indent}{Resources.A.Exceptions_StackTrace}:");
                indent = indent.Increment();
                // replace default indents
                var stack = ex.StackTrace.Replace(
                    $"{NewLine}   {Resources.A.Exceptions_StackTrace_at} ", 
                    $"{NewLine}{indent}{Resources.A.Exceptions_StackTrace_at} ");
                writeLine($"{indent}{stack.Remove(0,3)}");
                indent.Decrement();
            }
        }

        internal static Exception EscapeWrappers(this Exception exception)
        {
            if (exception is AggregateException aggEx)
            {
                var original = exception;
                exception = aggEx.GetBaseException().WithDataFrom(original);

                if (exception is AggregateException)
                {
                    // will be AggregateException where there are multiple inner exceptions
                    return exception;
                }
            }

            if (exception is TargetInvocationException { InnerException: { } } tie)
            {
                exception = EscapeWrappers(tie.InnerException!).WithDataFrom(tie);
            }

            return exception;
        }

        private static Exception WithDataFrom(this Exception exception, Exception original)
        {
            // copy exception.Data that can be used store debug context
            foreach (DictionaryEntry entry in original.Data)
            {
                if (!exception.Data.Contains(entry.Key))
                {
                    exception.Data[entry.Key] = entry.Value;
                }
            }

            return exception;
        }
    }
}