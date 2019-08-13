namespace CommandDotNet.Example.Issues
{
    [Command(Name = "issues", Description = "Apps displaying issues or verifying fixes for issues")]
    public class IssueApps
    {
        [SubCommand]
        public Issue55 Issue55 { get; set; }
    }
}