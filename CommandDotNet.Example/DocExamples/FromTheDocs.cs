namespace CommandDotNet.Example.DocExamples
{
    [Command(nameof(FromTheDocs), Description = "examples used in documentation")]
    public class FromTheDocs
    {

        [Subcommand]
        [Command]
        public class GettingStarted
        {
            [Subcommand(RenameAs = nameof(Calc1))]
            public DocExamples.GettingStarted.Eg1_Minumum.Program Calc1 { get; set; } = null!;

            [Subcommand(RenameAs = nameof(Calc2))]
            public DocExamples.GettingStarted.Eg2_Descriptions.Program Calc2 { get; set; } = null!;

            [Subcommand(RenameAs = nameof(Calc3))]
            public DocExamples.GettingStarted.Eg3_Testable.Program Calc3 { get; set; } = null!;

            [Subcommand(RenameAs = nameof(Calc4))]
            public DocExamples.GettingStarted.Eg3_Testable.Program Calc4 { get; set; } = null!;

            [Subcommand(RenameAs = nameof(Pipes))]
            public DocExamples.GettingStarted.Eg5_Pipes.Program Pipes { get; set; } = null!;

            [Subcommand(RenameAs = nameof(CtrlC))]
            public DocExamples.GettingStarted.Eg6_CtrlC.Program CtrlC { get; set; } = null!;
        }

        [Subcommand]
        [Command]
        public class Commands
        {
            [Subcommand]
            public DocExamples.Commands.Eg1_Minimum.Calculator Calculator { get; set; } = null!;

            [Subcommand]
            public DocExamples.Commands.Eg1_Minimum.Git Git { get; set; } = null!;
        }
    }
}