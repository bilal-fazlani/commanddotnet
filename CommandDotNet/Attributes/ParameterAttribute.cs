using System;

namespace CommandDotNet.Attributes
{
    public class ParameterAttribute : Attribute
    {        
        public string Description { get; set; }

        public bool RequiredString { get; set; }
        //public string Alias { get; set; }
    }
}