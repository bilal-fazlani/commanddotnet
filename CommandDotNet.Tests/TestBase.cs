using System;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Models;
using Xunit.Abstractions;

namespace CommandDotNet.Tests
{
    public class TestBase
    {
        protected readonly ITestOutputHelper TestOutputHelper;
        protected readonly TestConsoleWriter TestConsoleWriter;

        private List<EventHandler<string>> _outputHandlers = new List<EventHandler<string>>();

        public TestBase(ITestOutputHelper testOutputHelper)
        {
            TestOutputHelper = testOutputHelper;
            TestConsoleWriter = new TestConsoleWriter();

            CaptureOutputLine(OnWriteLine);
            
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

        protected void CaptureOutputLine(EventHandler<string> handler)
        {
            TestConsoleWriter.WriteLineEvent += handler;
            _outputHandlers.Append(handler);
        }

        public void Dispose()
        {
            foreach (var handler in _outputHandlers)
            {
                TestConsoleWriter.WriteLineEvent -= handler;
            }
            TestConsoleWriter.Dispose();
        }
    }
}