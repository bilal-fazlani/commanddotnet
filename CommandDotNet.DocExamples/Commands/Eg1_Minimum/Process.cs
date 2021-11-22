namespace CommandDotNet.DocExamples.Commands.Eg1_Minimum
{
    // begin-snippet: commands_default_command
    public class Program
    {
        static int Main(string[] args) =>
            new AppRunner<Program>().Run(args);

        [DefaultCommand]
        public void Process()
        {
            // do very important stuff
        }
    }
    // end-snippet
}