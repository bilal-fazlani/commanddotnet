using System;
using CommandDotNet.Extensions;

namespace CommandDotNet.TestTools.Scenarios
{
    public class Scenario : IScenario
    {
        public string Name { get; }

        /// <summary>The user input</summary>
        public ScenarioWhen When { get; } = new ScenarioWhen();

        /// <summary>
        /// The input as would be typed in the shell.
        /// Can handle quoted strings. 
        /// </summary>
        [Obsolete("Use When { Args = ...")]
        public string WhenArgs
        {
            set => When.Args = value;
        }

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
                ? $"{nameof(Scenario)}: args={When.Args ?? When.ArgsArray.ToCsv(" ")}"
                : $"{nameof(Scenario)}: {Name} > args={When.Args ?? When.ArgsArray.ToCsv(" ")}";
        }
    }
}