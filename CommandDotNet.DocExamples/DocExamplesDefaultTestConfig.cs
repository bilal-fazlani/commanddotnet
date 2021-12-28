using CommandDotNet.Builders;
using CommandDotNet.TestTools;

namespace CommandDotNet.DocExamples
{
    public class DocExamplesDefaultTestConfig : IDefaultTestConfig
    {
        public TestConfig Default => new()
        {
            AppInfoOverride = new AppInfo(
                false, false, false, 
                typeof(DocExamplesDefaultTestConfig).Assembly, 
                "doc-examples.dll", "doc-examples.dll", "1.1.1.1")
        };
    }
}