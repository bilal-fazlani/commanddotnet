namespace CommandDotNet.Tests.BddTests.Framework
{
    public class ScenarioContext: IScenarioContext
    {
        public string Description { get; set; }
        public object Host { get; set; }

        public override string ToString()
        {
            return Description == null 
                ? Host.GetType().Name 
                : $"{Host.GetType().Name}:{Description}";
        }
    }
}