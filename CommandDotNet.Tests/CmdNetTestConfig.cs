using CommandDotNet.TestTools;

namespace CommandDotNet.Tests
{
    public class CmdNetTestConfig : TestConfig
    {
        public static TestConfig NewDefault { get; set; } = new TestConfig
        {
            PrintCommandDotNetLogs = true,
            OnError = {Print = {All = true}}
        };
    }
}