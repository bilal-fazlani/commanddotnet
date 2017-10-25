using System;

namespace CommandDotNet.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            CommandHelper<MyApplication> app = new CommandHelper<MyApplication>();
            int result = app.Run(args);
            Environment.Exit(result);
        }
    }

    public class MyApplication
    {
        public void Jump(string level)
        {
            Console.WriteLine($"I just jumped {level}, and I felt awesome!");
        }
    }
}