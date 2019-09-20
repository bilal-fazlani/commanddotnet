using System;

namespace CommandDotNet
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CommandAttribute : Attribute, INameAndDescription
    {
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public string ExtendedHelpText { get; set; }
    }

    [Obsolete("Use CommandAttribute instead")]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApplicationMetadataAttribute : CommandAttribute
    {

    }
}