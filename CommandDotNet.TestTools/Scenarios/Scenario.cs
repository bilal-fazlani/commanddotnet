using CommandDotNet.Extensions;

namespace CommandDotNet.TestTools.Scenarios
{
    public class Scenario : IScenario
    {
        public string Name { get; }

        /// <summary>The starting context</summary>
        public ScenarioGiven Given { get; } = new ScenarioGiven();

        /// <summary>
        /// The input as would be typed in the shell.
        /// Can handle quoted strings. 
        /// </summary>
        public string WhenArgs { get; set; }

        /// <summary>Use this for tested arguments that can contain spaces</summary>
        public string[] WhenArgsArray { get; set; }

        /// <summary>The expectations</summary>
        public ScenarioThen Then { get; } = new ScenarioThen();

        public Scenario()
        {
        }

        /// <summary>Use this in data-driven tests to distinguish between scenarios</summary>
        public Scenario(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name.IsNullOrWhitespace()
                ? $"{nameof(Scenario)}: args={WhenArgs ?? WhenArgsArray.ToCsv(" ")}"
                : $"{nameof(Scenario)}: {Name} > args={WhenArgs ?? WhenArgsArray.ToCsv(" ")}";
        }
    }
}