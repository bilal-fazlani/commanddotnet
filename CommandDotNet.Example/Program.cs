using CommandDotNet.Example.Issues;
using CommandDotNet.Help;

namespace CommandDotNet.Example
{
    class Program
    {
        static int Main(string[] args)
        {
            // return Run<GitApplication>(args);
            return Run<Examples>(args);
        }

        private static int Run<TApp>(string[] args) where TApp : class
        {
            AppRunner<TApp> appRunner = new AppRunner<TApp>(new AppSettings
            {
                Case = Case.KebabCase,
                EnableDirectives = true,
                EnableVersionOption = true,
                Help =
                {
                    TextStyle = HelpTextStyle.Detailed
                }
            });
            return appRunner.Run(args);
        }

        public class Examples
        {
            [SubCommand]
            public GitApplication GitApplication { get; set; }

            [SubCommand]
            public ModelApp ModelApp { get; set; }

            [SubCommand]
            public MyApplication MyApplication { get; set; }

            [SubCommand]
            public IssueApps IssueApps { get; set; }
        }
    }
}