namespace CommandDotNet.TestTools.Scenarios
{
    public class Scenario : IScenario
    {
        public string Name { get; }

        public ScenarioGiven Given { get; } = new ScenarioGiven();

        public string WhenArgs { get; set; }

        /// <summary>Use this for tested arguments that can contain spaces</summary>
        public string[] WhenArgsArray { get; set; }

        public ScenarioThen Then { get; } = new ScenarioThen();

        public Scenario()
        {
        }

        protected Scenario(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}