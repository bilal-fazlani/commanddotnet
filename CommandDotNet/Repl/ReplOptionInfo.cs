namespace CommandDotNet.Repl
{
    public class ReplOptionInfo
    {
        public static readonly ReplOptionInfo Default = new (
            "interactive", 'i', "enter an interactive session");

        public string? LongName { get; }
        public char? ShortName { get; }
        public string? Description { get; }

        public ReplOptionInfo(string? longName = null, char? shortName = null, string? description = null)
        {
            LongName = longName;
            ShortName = shortName;
            Description = description;

            if (longName is null && shortName is null)
            {
                throw new InvalidConfigurationException($"must define either {longName} or {shortName} or both");
            }
        }
    }
}