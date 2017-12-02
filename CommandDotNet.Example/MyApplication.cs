using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CommandDotNet.Attributes;
using Newtonsoft.Json;

namespace CommandDotNet.Example
{
    [ConsoleApplication(Description = "Sample application for demonstation")]
    public class MyApplication
    {
        public string AppendText { get; set; }
        
        [Command(Description = "makes someone jump")]
        public void Jump(
            [Parameter(Description = "did someone jump?")]
            bool jumped, 
            
            string level, 
            
            int? feets, 
            
            IEnumerable<string> friends, 
            
            double height, 
            
            bool? log,
            
            [Parameter(RequiredString = true)]
            string password,
            
            int times = 0, 
            
            string author = "john")
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