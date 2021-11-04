namespace CommandDotNet.Spectre
{
    public class Resources
    {
        public static Resources A = new Resources();

        public virtual string Selection_paging_instructions(string? typeDisplayName) =>
            $"Move up and down to reveal more {typeDisplayName}";
    }
}