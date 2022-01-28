using CommandDotNet.Extensions;

namespace CommandDotNet
{
    public class CommandAppSettings : IIndentableToString
    {
        /// <summary>When true, methods on base classes will be included as commands.</summary>
        public bool InheritCommandsFromBaseClasses { get; set; }

        public override string ToString()
        {
            return ToString(new Indent());
        }

        public string ToString(Indent indent)
        {
            return this.ToStringFromPublicProperties(indent);
        }
    }
}