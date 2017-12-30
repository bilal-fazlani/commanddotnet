using System;
using CommandDotNet.Models;

namespace CommandDotNet.Example
{
    class Program
    {
        static int Main(string[] args)
        {
            AppRunner<DirUtilities> appRunner = new AppRunner<DirUtilities>();
            return appRunner.Run(args);
        }
    }
}