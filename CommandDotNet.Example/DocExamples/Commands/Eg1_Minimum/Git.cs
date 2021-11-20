namespace CommandDotNet.Example.DocExamples.Commands.Eg1_Minimum
{
    // begin-snippet: commands_git
    public class Git
    {
        [Subcommand]
        public class Stash
        {
            [Command(Usage = "%AppName% %CmdPath%")]
            public void Pop(){ }

            [DefaultCommand]
            public void StashChanges(){ }
        }
    }
    // end-snippet
}