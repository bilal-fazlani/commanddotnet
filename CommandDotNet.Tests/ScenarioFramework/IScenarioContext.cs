namespace CommandDotNet.Tests.ScenarioFramework
{
    public interface IScenarioContext
    {
        string Description { get; }
        object Host { get; }
    }
}