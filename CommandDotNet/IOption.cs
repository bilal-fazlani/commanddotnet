using System;

namespace CommandDotNet
{
    public interface IOption : IArgument
    {
        string Template { get; }
        string ShortName { get; }
        string SymbolName { get; }
        bool Inherited { get; }

        /// <summary>True when option is help or version</summary>
        bool IsSystemOption { get; }

        /// <summary>
        /// When provided, the delegate will be invoked immediately
        /// and no further options will be parsed and the command
        /// will not be executed.
        /// This will be replaced by middleware.  It is used
        /// to decouple Help & Version options from the command
        /// processing
        /// </summary>
        Action InvokeAsCommand { get; }
    }
}