using System;
using CommandDotNet.Models;

namespace CommandDotNet.Example
{
    class Program
    {
        static int Main(string[] args)
        {
            AppRunner<GitApplication> appRunner = new AppRunner<GitApplication>();
            return appRunner.Run(args);
        }
    }
}