using System;

namespace CommandDotNet.Attributes
{
    public class ApplicationMetadataAttribute : Attribute
    {
        public string Description { get; set; }
    }
}