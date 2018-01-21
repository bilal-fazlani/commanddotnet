using CommandDotNet.Models;

namespace CommandDotNet.Example
{
    class Program
    {
        static int Main(string[] args)
        {
            AppRunner<MyApplication> appRunner = new AppRunner<MyApplication>(new AppSettings()
            {
                Case = Case.KebabCase,
                HelpTextStyle = HelpTextStyle.Tabular
            });
            return appRunner.Run(args);
        }
    }
}