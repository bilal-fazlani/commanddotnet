namespace CommandDotNet.Spectre
{
    public class Resources
    {
        public static Resources A = new();

        public virtual string Selection_paging_instructions(string argumentName) =>
            $"[grey](Move up and down to reveal more {argumentName})[/]";

        public virtual string MultiSelection_selection_instructions(string argumentName) =>
            $"[grey](Press [blue]<space>[/] to toggle the {argumentName}, [green]<enter>[/] to accept)[/]";
    }
}