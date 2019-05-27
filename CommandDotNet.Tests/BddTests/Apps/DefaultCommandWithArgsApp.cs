using CommandDotNet.Attributes;
using CommandDotNet.Tests.Utils;

namespace CommandDotNet.Tests.BddTests.Apps
{
    public class DefaultCommandWithArgsApp
    {
        [InjectProperty]
        public TestOutputs TestOutputs { get; set; }

        [DefaultMethod]
        public void DefaultMethod(
            [Argument(Description = "some text")]
            string text)
        {
            TestOutputs.Capture(text);
        }
    }
}