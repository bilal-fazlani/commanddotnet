using CommandDotNet.Diagnostics;

namespace CommandDotNet.DocExamples.Diagnostics;

public static class Diagnostics_Examples
{
    public class App
    {
        public void Process() { }
    }

    // begin-snippet: diagnostics_debug_directive
    class Program
    {
        static int Main(string[] args)
        {
            Debugger.AttachIfDebugDirective(args);
            return new AppRunner<App>().Run(args);
        }
    }
    // end-snippet
}
