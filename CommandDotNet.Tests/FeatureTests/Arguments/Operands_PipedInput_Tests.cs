using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Execution;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class Operands_PipedInput_Tests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Operands_PipedInput_Tests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }
        
        [Fact]
        public void GivenPipedInputAndNoExplicitValue_PipedInputIsAppended()
        {
            new AppRunner<App>()
                .AppendPipedInputToOperandList()
                .VerifyScenario(_testOutputHelper,
                    new Scenario
                    {
                        Given = {PipedInput = new[] {"aaa", "bbb"}},
                        WhenArgs = $"{nameof(App.List)}",
                        Then =
                        {
                            Outputs = {new List<string> {"aaa", "bbb"}}
                        }
                    });
        }

        [Fact]
        public void GivenPipedInputAndExplicitValue_AppendsPipedToExplicit()
        {
            new AppRunner<App>()
                .AppendPipedInputToOperandList()
                .VerifyScenario(_testOutputHelper,
                    new Scenario
                    {
                        Given = {PipedInput = new[] {"ccc", "ddd"}},
                        WhenArgs = $"{nameof(App.List)} aaa bbb",
                        Then =
                        {
                            Outputs = {new List<string> {"aaa", "bbb", "ccc", "ddd"}}
                        }
                    });
        }

        [Fact]
        public void GivenNoListArg_PipedInputIsIgnored()
        {
            new AppRunner<App>()
                .AppendPipedInputToOperandList()
                .VerifyScenario(_testOutputHelper,
                    new Scenario
                    {
                        Given = {PipedInput = new[] {"aaa"}},
                        WhenArgs = $"{nameof(App.Single)} single",
                        Then =
                        {
                            Outputs = {"single"}
                        }
                    });
        }

        [Fact]
        public void GivenSingleAndListArg_AndNoArgValuesExplicitlyProvided_PipedInputAppendedToListArg()
        {
            new AppRunner<App>()
                .AppendPipedInputToOperandList()
                .VerifyScenario(_testOutputHelper,
                    new Scenario
                    {
                        Given = {PipedInput = new[] {"aaa", "bbb"}},
                        WhenArgs = $"{nameof(App.SingleAndList)} single",
                        Then =
                        {
                            Outputs =
                            {
                                "single",
                                new List<string> {"aaa", "bbb"}
                            }
                        }
                    });
        }

        [Fact]
        public void GivenStreamingApp_PipedInput_IsOnlyEnumeratedWithinTheCommandMethod()
        {
            var stream = new PipedInputStream("aaa", "bbb");
            new AppRunner<StreamingApp>()
                .AppendPipedInputToOperandList()
                .Configure(c =>
                    c.UseMiddleware((context, next) =>
                        {
                            stream.EnumerationIsPremature = false;
                            return next(context);
                        }, 
                        MiddlewareStages.Invoke, -1))
                .VerifyScenario(_testOutputHelper,
                    new Scenario
                    {
                        Given = {PipedInput = stream},
                        WhenArgs = $"{nameof(StreamingApp.Stream)}",
                        Then =
                        {
                            Outputs = {new List<string> {"aaa", "bbb"}}
                        }
                    });
        }

        private class PipedInputStream : IEnumerable<string>
        {
            private readonly Queue<string> _queue;
            public bool EnumerationIsPremature = true;

            public PipedInputStream(params string[] inputs)
            {
                _queue = new Queue<string>(inputs);
            }

            public IEnumerator<string> GetEnumerator()
            {
                if (EnumerationIsPremature)
                    throw new Exception("premature enumeration");

                if (!_queue.Any())
                {
                    yield break;
                }

                yield return _queue.Dequeue();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public class App
        {
            private TestOutputs TestOutputs { get; set; }

            public void Single([Operand] string singleArg)
            {
                TestOutputs.CaptureIfNotNull(singleArg);
            }

            public void List([Operand] List<string> listArgs)
            {
                TestOutputs.CaptureIfNotNull(listArgs);
            }

            public void SingleAndList([Operand] string singleArg, [Operand] List<string> listArgs)
            {
                TestOutputs.CaptureIfNotNull(singleArg);
                TestOutputs.CaptureIfNotNull(listArgs);
            }
        }

        public class StreamingApp
        {
            private TestOutputs TestOutputs { get; set; }

            public void Stream(IEnumerable<string> input)
            {
                TestOutputs.Capture(input.ToList());
            }
        }
    }
}