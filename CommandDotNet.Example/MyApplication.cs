using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CommandDotNet.Attributes;
using Newtonsoft.Json;

namespace CommandDotNet.Example
{
    [ApplicationMetadata(Description = "Sample application for demonstation")]
    public class MyApplication
    {
        [Arguement(Description = "Appends the given text at the end of line", Template = "-t | --text")]
        public string AppendText { get; set; }
        
        public int Timeout { get; set; }
        
        [ApplicationMetadata(Description = "makes someone jump")]
        public void Jump(
            [Arguement(Description = "did someone jump?")]
            bool jumped, 
            
            string level, 
            
            int? feets, 
            
            IEnumerable<string> friends, 
            
            double height, 
            
            bool? log,
            
            [Arguement(RequiredString = true)]
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
            
            Console.WriteLine(AppendText);
        }
    }
}