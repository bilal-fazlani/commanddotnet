using CommandDotNet.Attributes;
using CommandDotNet.Tests.Utils;

namespace CommandDotNet.Tests.BddTests.Apps
{
    public class DefaultCommandNoArgsApp
    {
        public const string DefaultMethodExecuted = "default executed";

        [InjectProperty]
        public TestOutputs TestOutputs { get; set; }

        [DefaultMethod]
        public void DefaultMethod()
        {
            this.TestOutputs.Capture(DefaultMethodExecuted);
        }
    }
}