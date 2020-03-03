namespace CommandDotNet.Example.DocExamples
{
    [Command(Description = "This is a crappy calculator", ExtendedHelpText = "Some more help text that appears at the bottom")]
    public class Calculator
    {
        [Command(Description = "Subtracts value2 from value1 and prints output",
            ExtendedHelpText = "Again, some more detailed help text which has no meaning I still have to write to demostrate this feature",
            Name = "subtractValues")]
        public void Subtract2(int value1, int value2)
        {
        }
    }
}