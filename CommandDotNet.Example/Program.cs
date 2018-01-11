using CommandDotNet.Models;

namespace CommandDotNet.Example
{
    class Program
    {
        static int Main(string[] args)
        {
            AppRunner<ValidationApp> appRunner = new AppRunner<ValidationApp>(new AppSettings()
            {
                Case = Case.KebabCase,
            });
            return appRunner.Run(args);
        }
    }
}