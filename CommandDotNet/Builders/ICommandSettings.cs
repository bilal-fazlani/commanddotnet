using CommandDotNet.Parsing;

namespace CommandDotNet.Builders
{
    internal interface ICommandSettings
    {
        /// <summary>
        /// When false, unexpected operands will generate a parse failure.<br/>
        /// When true, unexpected arguments will be ignored and added to <see cref="ParseResult.RemainingOperands"/><br/>
        /// </summary>
        bool IgnoreUnexpectedOperands { get; set; }

        /// <summary>
        /// The default <see cref="ArgumentSeparatorStrategy"/>.
        /// This can be overridden for a <see cref="Command"/> using the <see cref="CommandAttribute"/>
        /// </summary>
        ArgumentSeparatorStrategy ArgumentSeparatorStrategy { get; set; }
    }
}