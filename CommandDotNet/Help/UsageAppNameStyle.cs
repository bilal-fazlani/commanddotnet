namespace CommandDotNet.Help
{
    public enum UsageAppNameStyle
    {
        /// <summary>
        /// A combination of the other rules in this order:<br/>
        /// <see cref="GlobalTool"/>
        /// if the root command has a name specified in <see cref="ApplicationMetadataAttribute"/>,
        /// else <see cref="Executable"/> if the file extension is '.exe' <br/>
        /// else <see cref="DotNet"/>
        /// </summary>
        Adaptive,

        /// <summary>"dotnet {fileName}"</summary>
        DotNet,

        /// <summary>"{rootCommand.ApplicationMetadataAttribute.Name}"</summary>
        GlobalTool,

        /// <summary>"{fileName}"</summary>
        Executable
    }
}