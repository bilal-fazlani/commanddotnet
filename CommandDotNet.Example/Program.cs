using System;
using System.Collections.Generic;
using Newtonsoft.Json;

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
        public void Jump(bool jumped, string level, int feets, IEnumerable<string> friends)
        {
            Console.WriteLine(JsonConvert.SerializeObject(new
            {
                jumped,
                level,
                feets,
                friends
            }));
        }
    }
}