using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Execution;
using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public abstract class AppendPipedInputTestsBase
    {
        protected AppendPipedInputTestsBase(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        protected abstract AppRunner AppRunner<T>() where T : class;

        [Fact]
        public void GivenPipedInputAndNoExplicitValue_PipedInputIsAppended()
        {
            AppRunner<App>()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(App.List)}",
                        PipedInput = new[] {"aaa", "bbb"}
                    },
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe(new List<string> {"aaa", "bbb"})
                    }
                });
        }

        [Fact]
        public void GivenPipedInputAndExplicitValue_AppendsPipedToExplicit()
        {
            AppRunner<App>()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(App.List)} aaa bbb",
                        PipedInput = new[] {"ccc", "ddd"}
                    },
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe(new List<string> {"aaa", "bbb", "ccc", "ddd"})
                    }
                });
        }

        [Fact]
        public void GivenNoListArg_PipedInputIsIgnored()
        {
            AppRunner<App>()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(App.Single)} single",
                        PipedInput = new[] {"aaa"}
                    },
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe("single")
                    }
                });
        }

        [Fact]
        public void GivenSingleAndListArg_AndNoArgValuesExplicitlyProvided_PipedInputAppendedToListArg()
        {
            AppRunner<App>()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(App.SingleAndList)} single",
                        PipedInput = new[] {"aaa", "bbb"}
                    },
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe("single", new List<string> {"aaa", "bbb"})
                    }
                });
        }

        [Fact]
        public void GivenStreamingApp_PipedInput_IsOnlyEnumeratedWithinTheCommandMethod()
        {
            var stream = new PipedInputStream("aaa", "bbb");
            AppRunner<StreamingApp>()
                .Configure(c =>
                    c.UseMiddleware((context, next) =>
                        {
                            stream.EnumerationIsPremature = false;
                            return next(context);
                        },
                        MiddlewareStages.Invoke, -1))
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(StreamingApp.Stream)}",
                        PipedInput = stream
                    },
                    Then =
                    {
                        // stream is read-once so we can't evaluate it with a second time with ParamValuesShouldBe
                        AssertContext = ctx => ctx.GetCommandInvocationInfo<StreamingApp>()
                            .Instance!.StreamedInput.Should().BeEquivalentTo(new List<string> {"aaa", "bbb"})
                    }
                });
        }

        [Fact]
        public void GivenStreamingApp_PipedInput_ConvertsType_And_IsOnlyEnumeratedWithinTheCommandMethod()
        {
            var stream = new PipedInputStream("1", "2");
            AppRunner<StreamingApp>()
                .Configure(c =>
                    c.UseMiddleware((context, next) =>
                        {
                            stream.EnumerationIsPremature = false;
                            return next(context);
                        },
                        MiddlewareStages.Invoke, -1))
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(StreamingApp.StreamNumbers)}",
                        PipedInput = stream
                    },
                    Then =
                    {
                        // stream is read-once so we can't evaluate it with a second time with ParamValuesShouldBe
                        AssertContext = ctx => ctx.GetCommandInvocationInfo<StreamingApp>()
                            .Instance!.StreamedNumbers.Should().BeEquivalentTo(new List<int> {1,2})
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

        private class App
        {
            public void Single([Operand] string singleArg)
            {
            }

            public void List([Operand] List<string> listArgs)
            {
            }

            public void SingleAndList([Operand] string singleArg, [Operand] List<string> listArgs)
            {
            }
        }

        private class StreamingApp
        {
            public List<string> StreamedInput { get; set; } = null!;
            public List<int> StreamedNumbers { get; set; } = null!;

            public void Stream(IEnumerable<string> input)
            {
                StreamedInput = input.ToList();
            }

            public void StreamNumbers(IEnumerable<int> input)
            {
                StreamedNumbers = input.ToList();
            }
        }
    }
}