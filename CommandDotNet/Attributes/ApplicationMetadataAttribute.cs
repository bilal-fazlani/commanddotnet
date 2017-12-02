using System;

namespace CommandDotNet.Attributes
{
    public class ApplicationMetadataAttribute : Attribute
    {
        public string Name { get; set; }
        
        public string Description { get; set; }
    }
}