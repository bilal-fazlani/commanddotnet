using System;
using CommandDotNet.Attributes;
using CommandDotNet.Example.Issues;
using CommandDotNet.Models;

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
            AppRunner<TApp> appRunner = new AppRunner<TApp>(new AppSettings()
            {
                Case = Case.KebabCase,
                HelpTextStyle = HelpTextStyle.Detailed
            });
            return appRunner.Run(args);
        }
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

        public void Add(int x, int y)
        {
            Console.Out.WriteLine(x+y);
        }
    }
}