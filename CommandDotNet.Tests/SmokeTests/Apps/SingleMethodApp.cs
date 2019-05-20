using CommandDotNet.Attributes;
using CommandDotNet.Tests.Utils;

namespace CommandDotNet.Tests.SmokeTests.Apps
{
    public class SingleMethodApp
    {
        [InjectProperty]
        public TestWriter Writer { get; set; }
            
        [InjectProperty]
        public TestOutputs TestOutputs { get; set; }
            
        public int X { get; private set; }
        public int Y { get; private set; }
            
        public void Add(int x, int y)
        {
            Writer.Write($"{x}+{y}={x+y}");

            X = x;
            Y = y;
            TestOutputs.Capture(this);
        }
    }
}