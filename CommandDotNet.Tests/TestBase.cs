using System;
using Xunit.Abstractions;

namespace CommandDotNet.Tests
{
    public class TestBase
    {
        protected readonly ITestOutputHelper TestOutputHelper;
        protected readonly TestConsoleWriter TestConsoleWriter;

        public TestBase(ITestOutputHelper testOutputHelper)
        {
            TestOutputHelper = testOutputHelper;
            TestConsoleWriter = new TestConsoleWriter();
            
            TestConsoleWriter.WriteLineEvent += OnWriteLine;
            
            Console.SetOut(TestConsoleWriter);
            Console.SetError(TestConsoleWriter);
        }

        private void OnWriteLine(object sender, string value)
        {
            try
            {
                TestOutputHelper?.WriteLine(value);
            }
            catch (InvalidOperationException e) when(e.Message == "There is no currently active test.")
            {
                
            }
        }

        public void Dispose()
        {
            TestConsoleWriter.WriteLineEvent -= OnWriteLine;
            TestConsoleWriter.Dispose();
        }
    }
}