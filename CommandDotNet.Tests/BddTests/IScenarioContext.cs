namespace CommandDotNet.Tests.BddTests
{
    public interface IScenarioContext
    {
        string Description { get; }
        object Host { get; }
    }
}