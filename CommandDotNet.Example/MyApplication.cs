using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CommandDotNet.Example
{
    public class MyApplication
    {
        public void Jump(bool jumped, string level, int? feets, 
            IEnumerable<string> friends, double height, bool? log,
            int times = 0, string author = "john")
        {
            Console.WriteLine(JsonConvert.SerializeObject(new
            {
                jumped,
                level,
                feets,
                friends,
                height,
                log,
                author,
                times
            }));
        }
    }
}