using CommandDotNet.Models;

namespace CommandDotNet.Example
{
    class Program
    {
        static int Main(string[] args)
        {
            AppRunner<ModelApp> appRunner = new AppRunner<ModelApp>(new AppSettings()
            {
                Case = Case.KebabCase,
            });
            return appRunner.Run(args);
        }
    }
}