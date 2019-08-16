namespace CommandDotNet.TestTools.Scenarios
{
    /// <summary>A command line scenario</summary>
    public interface IScenario
    {
        /// <summary>The starting context</summary>
        ScenarioGiven Given { get; }

        /// <summary>
        /// The input as would be typed in the shell.
        /// Can handle quoted strings. 
        /// </summary>
        string WhenArgs { get; }

        /// <summary>The input as would be provided by the Program.Main</summary>
        string[] WhenArgsArray { get; }

        /// <summary>The expectations</summary>
        ScenarioThen Then { get; }
    }
}