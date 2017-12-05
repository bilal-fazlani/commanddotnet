using System;

namespace CommandDotNet.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApplicationMetadataAttribute : Attribute
    {
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public string ExtendedHelpText { get; set; }
    }
}