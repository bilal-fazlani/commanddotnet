using System.IO;
using System.Text;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.Utils
{
    public class TestConsoleWriter : TextWriter
    {
        private readonly StringWriter _writer;
        private readonly ITestOutputHelper _testOutputHelper;

        public override Encoding Encoding => Encoding.UTF8;

        public TestConsoleWriter(ITestOutputHelper testOutputHelper)
        {
            _writer = new StringWriter();
            _testOutputHelper = testOutputHelper;
        }

        public override void WriteLine(string value)
        {
            _testOutputHelper?.WriteLine(value);
            _writer.WriteLine(value);
            base.WriteLine(value);
        }

        public override string ToString()
        {
            return _writer.ToString();
        }
    }
}