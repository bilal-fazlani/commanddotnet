using System.IO;

namespace CommandDotNet.Rendering
{
    public interface IStandardOut
    {
        TextWriter Out { get; }
        
        void SetOut(TextWriter newOut);

        bool IsOutputRedirected { get; }
    }
}