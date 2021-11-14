using CommandDotNet.Builders;
using CommandDotNet.TestTools;

namespace CommandDotNet.Tests
{
    public class CmdNetDefaultTestConfig : IDefaultTestConfig
    {
        public TestConfig Default => new()
        {
            OnError = {Print = {ConsoleOutput = true, CommandContext = true}},
            AppInfoOverride = new AppInfo(
                false, false, false, 
                typeof(CmdNetDefaultTestConfig).Assembly, 
                "testhost.dll", "testhost.dll", "1.1.1.1")
        };
    }
}