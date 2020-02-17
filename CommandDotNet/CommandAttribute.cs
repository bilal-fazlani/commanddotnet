using System;

namespace CommandDotNet
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CommandAttribute : Attribute, INameAndDescription
    {
        public string Name { get; set; }
        
        public string Description { get; set; }

        public string Usage { get; set; }

        public string ExtendedHelpText { get; set; }
    }

    // keeping in this namespace for backwards compatibility with preview versions
    [Obsolete("Use CommandAttribute instead")]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApplicationMetadataAttribute : CommandAttribute
    {

    }
}

namespace CommandDotNet.Attributes
{
    // keeping in this namespace for backwards compatibility
    [Obsolete("Use CommandAttribute instead")]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApplicationMetadataAttribute : CommandAttribute
    {

    }
}