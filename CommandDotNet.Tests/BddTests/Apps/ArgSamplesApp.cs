using System.Collections.Generic;
using CommandDotNet.Attributes;
using CommandDotNet.Tests.Utils;

namespace CommandDotNet.Tests.BddTests.Apps
{
    public class ArgSamplesApp
    {
        [InjectProperty]
        public TestOutputs TestOutputs { get; set; }

        public void List(
            [Argument]
            List<string> extras)
        {
            this.TestOutputs.Capture(new ListResults{Extras = extras});
        }

        public void ListPlusOne(
            [Argument]
            string one,
            [Argument]
            List<string> extras)
        {
            this.TestOutputs.Capture(new ListResults
            {
                One = one,
                Extras = extras
            });
        }

        public class ListResults
        {
            public string One { get; set; }
            public List<string> Extras { get; set; }
        }
    }
}