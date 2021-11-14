using CommandDotNet.TestTools;

namespace CommandDotNet.Example.Tests
{
    public class ExampleDefaultTestConfig : IDefaultTestConfig
    {
        public TestConfig Default => new()
        {
            //PrintCommandDotNetLogs = true,
            OnSuccess = {Print = {ConsoleOutput = true}},
            OnError = {Print = {ConsoleOutput = true, CommandContext = true}}
        };
    }
}