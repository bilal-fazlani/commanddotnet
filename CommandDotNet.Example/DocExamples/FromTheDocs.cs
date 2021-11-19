namespace CommandDotNet.Example.DocExamples
{
    [Command(nameof(FromTheDocs), Description = "examples used in documentation")]
    public class FromTheDocs
    {
        [Subcommand]
        [Command]
        public class GettingStarted
        {
            [Subcommand(RenameAs = "calc1")]
            public DocExamples.GettingStarted.Eg1_Minumum.Program Calc1 { get; set; } = null!;
            [Subcommand(RenameAs = "calc2")]
            public DocExamples.GettingStarted.Eg2_Descriptions.Program Calc2 { get; set; } = null!;
            [Subcommand(RenameAs = "calc3")]
            public DocExamples.GettingStarted.Eg3_Testable.Program Calc3 { get; set; } = null!;
        }
    }
}