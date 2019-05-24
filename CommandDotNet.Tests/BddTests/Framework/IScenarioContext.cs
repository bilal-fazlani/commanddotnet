namespace CommandDotNet.Tests.BddTests.Framework
{
    public interface IScenarioContext
    {
        string Description { get; }
        object Host { get; }
    }
}