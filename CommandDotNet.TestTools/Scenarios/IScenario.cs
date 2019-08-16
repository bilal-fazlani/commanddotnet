namespace CommandDotNet.TestTools.Scenarios
{
    public interface IScenario
    {
        IScenarioContext Context { get; set; }
        ScenarioGiven Given { get; }
        string WhenArgs { get; }
        string[] WhenArgsArray { get; }
        ScenarioThen Then { get; }
    }
}