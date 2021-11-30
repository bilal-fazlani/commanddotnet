using System.IO;

namespace CommandDotNet.Rendering
{
    public interface IStandardError
    {
        /// <summary>Gets the standard error output stream.</summary>
        TextWriter Error { get; }

        void SetError(TextWriter newError);

        /// <summary>Gets a value that indicates whether the error output stream has been redirected from the standard error stream.</summary>
        bool IsErrorRedirected { get; }
    }
}