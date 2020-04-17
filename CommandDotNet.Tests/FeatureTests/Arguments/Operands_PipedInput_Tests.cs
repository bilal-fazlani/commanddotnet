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
        private readonly ITestOutputHelper _output;

        public Operands_PipedInput_Tests(ITestOutputHelper output)
        {
            _output = output;
        }
        
        [Fact]
        public void GivenPipedInputAndNoExplicitValue_PipedInputIsAppended()
        {
            new AppRunner<App>()
                .AppendPipedInputToOperandList()
                .Verify(_output,
                    new Scenario
                    {
                        When =
                        {
                            Args = $"{nameof(App.List)}",
                            PipedInput = new[] {"aaa", "bbb"}
                        },
                        Then =
                        {
                            Captured = {new List<string> {"aaa", "bbb"}}
                        }
                    });
        }

        [Fact]
        public void GivenPipedInputAndExplicitValue_AppendsPipedToExplicit()
        {
            new AppRunner<App>()
                .AppendPipedInputToOperandList()
                .Verify(_output,
                    new Scenario
                    {
                        When =
                        {
                            Args = $"{nameof(App.List)} aaa bbb",
                            PipedInput = new[] {"ccc", "ddd"}
                        },
                        Then =
                        {
                            Captured = {new List<string> {"aaa", "bbb", "ccc", "ddd"}}
                        }
                    });
        }

        [Fact]
        public void GivenNoListArg_PipedInputIsIgnored()
        {
            new AppRunner<App>()
                .AppendPipedInputToOperandList()
                .Verify(_output,
                    new Scenario
                    {
                        When =
                        {
                            Args = $"{nameof(App.Single)} single",
                            PipedInput = new[] {"aaa"}
                        },
                        Then =
                        {
                            Captured = {"single"}
                        }
                    });
        }

        [Fact]
        public void GivenSingleAndListArg_AndNoArgValuesExplicitlyProvided_PipedInputAppendedToListArg()
        {
            new AppRunner<App>()
                .AppendPipedInputToOperandList()
                .Verify(_output,
                    new Scenario
                    {
                        When =
                        {
                            Args = $"{nameof(App.SingleAndList)} single",
                            PipedInput = new[] {"aaa", "bbb"}
                        },
                        Then =
                        {
                            Captured =
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
                .Verify(_output,
                    new Scenario
                    {
                        When =
                        {
                            Args = $"{nameof(StreamingApp.Stream)}",
                            PipedInput = stream
                        },
                        Then =
                        {
                            Captured = {new List<string> {"aaa", "bbb"}}
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
            private TestCaptures TestCaptures { get; set; }

            public void Single([Operand] string singleArg)
            {
                TestCaptures.CaptureIfNotNull(singleArg);
            }

            public void List([Operand] List<string> listArgs)
            {
                TestCaptures.CaptureIfNotNull(listArgs);
            }

            public void SingleAndList([Operand] string singleArg, [Operand] List<string> listArgs)
            {
                TestCaptures.CaptureIfNotNull(singleArg);
                TestCaptures.CaptureIfNotNull(listArgs);
            }
        }

        public class StreamingApp
        {
            private TestCaptures TestCaptures { get; set; }

            public void Stream(IEnumerable<string> input)
            {
                TestCaptures.Capture(input.ToList());
            }
        }
    }
}