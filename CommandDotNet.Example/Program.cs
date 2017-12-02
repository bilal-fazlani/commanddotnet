using System;
using CommandDotNet.Models;

namespace CommandDotNet.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            AppRunner<MyApplication> appRunner = new AppRunner<MyApplication>();
            int result = appRunner.Run(args);
            Environment.Exit(result);
        }
    }
}