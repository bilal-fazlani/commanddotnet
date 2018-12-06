using CommandDotNet.Models;

namespace CommandDotNet.Example
{
    class Program
    {
        static int Main(string[] args)
        {
            AppRunner<Issue55> appRunner = new AppRunner<Issue55>(new AppSettings()
            {
                Case = Case.KebabCase,
                HelpTextStyle = HelpTextStyle.Detailed
            });
            return appRunner.Run(args);
        }
    }
}