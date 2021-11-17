using System.IO;

namespace CommandDotNet.Rendering
{
    public interface IStandardIn
    {
        TextReader In { get; }
        
        void SetIn(TextReader newIn);

        bool IsInputRedirected { get; }
    }
}