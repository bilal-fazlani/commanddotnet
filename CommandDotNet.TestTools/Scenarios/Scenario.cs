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

        IScenarioContext IScenario.Context { get; set; }

        public Scenario()
        {
        }

        protected Scenario(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            var context = ((IScenario)this).Context;

            if (context == null)
            {
                return Name;
            }

            return context.Description == null 
                ? $"{context.Host.GetType().Name} > {Name}" 
                : $"{context.Host.GetType().Name} > {Name} ({context.Description})";
        }
    }
}