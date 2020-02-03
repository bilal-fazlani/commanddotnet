using System;

namespace CommandDotNet.Help
{
    public enum UsageAppNameStyle
    {
        /// <summary>
        /// else <see cref="Executable"/> if the file extension is '.exe' <br/>
        /// else <see cref="DotNet"/>
        /// </summary>
        Adaptive,

        /// <summary>"dotnet {fileName}"</summary>
        DotNet,

        /// <summary>"{rootCommand.CommandAttribute.Name}"</summary>
        [Obsolete("configure via AppSettings.Help.UsageAppName instead")]
        GlobalTool,

        /// <summary>"{fileName}"</summary>
        Executable
    }
}