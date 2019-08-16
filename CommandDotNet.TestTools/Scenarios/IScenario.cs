namespace CommandDotNet.TestTools.Scenarios
{
    public interface IScenario
    {
        ScenarioGiven Given { get; }
        string WhenArgs { get; }
        string[] WhenArgsArray { get; }
        ScenarioThen Then { get; }
    }
}