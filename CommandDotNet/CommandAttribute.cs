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

        /// <summary>
        /// Overrides <see cref="AppSettings.ParseSeparatedArguments"/><br/>
        /// Arguments after the argument separator '--' are added to <see cref="ParseResult.SeparatedArguments"/><br/>
        /// When true, the arguments will be parsed as operands and also added to <see cref="ParseResult.SeparatedArguments"/>
        /// </summary>
        public bool ParseSeparatedArguments
        {
            get => ParseSeparatedArgumentsAsNullable.GetValueOrDefault();
            set => ParseSeparatedArgumentsAsNullable = value;
        }

        /// <summary>Returns a nullable version of <see cref="ParseSeparatedArguments"/> that will be null if a value was not assigned</summary>
        public bool? ParseSeparatedArgumentsAsNullable { get; private set; }
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