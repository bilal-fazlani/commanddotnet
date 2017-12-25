using System;

namespace CommandDotNet.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class OptionAttribute : ArgumentBaseAttribute
    {        
        public string ShortName { get; set; }
        
        public string LongName { get; set; }
    }
}