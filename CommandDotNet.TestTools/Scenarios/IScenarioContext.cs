namespace CommandDotNet.TestTools.Scenarios
{
    public interface IScenarioContext
    {
        string Description { get; }
        object Host { get; }
    }
}