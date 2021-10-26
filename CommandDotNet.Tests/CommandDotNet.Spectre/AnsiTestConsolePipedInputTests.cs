using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;
using CommandDotNet.Spectre;
using CommandDotNet.Spectre.Testing;
using CommandDotNet.TestTools.Scenarios;
using FluentAssertions;
using Spectre.Console;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.CommandDotNet.Spectre
{
    public class AnsiTestConsolePipedInputTests
    {
        // If these work, then onReadLine works as well because PipedInput uses onReadLine

        public AnsiTestConsolePipedInputTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void AnsiTestConsole_works_with_TestTools()
        {
            var result = new AppRunner<App>()
                .UseSpectreAnsiConsole(new AnsiTestConsole())
                .RunInMem("Ansi lala");

            result.ExitCode.Should().Be(0);
            result.Console.AllText().Should().Be(@"lala
");
        }

        //copied from Operands_PipedInput_Tests

        [Fact]
        public void GivenPipedInputAndNoExplicitValue_PipedInputIsAppended()
        {
            new AppRunner<App>()
                .UseSpectreAnsiConsole(new AnsiTestConsole())
                .AppendPipedInputToOperandList()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(App.List)}",
                        PipedInput = new[] { "aaa", "bbb" }
                    },
                    Then =
                    {
                        Output = @"aaa,bbb
"
                    }
                });
        }

        [Fact]
        public void GivenPipedInputAndExplicitValue_AppendsPipedToExplicit()
        {
            new AppRunner<App>()
                .UseSpectreAnsiConsole(new AnsiTestConsole())
                .AppendPipedInputToOperandList()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(App.List)} aaa bbb",
                        PipedInput = new[] { "ccc", "ddd" }
                    },
                    Then =
                    {
                        Output = @"aaa,bbb,ccc,ddd
"
                    }
                });
        }

        [Fact]
        public void GivenNoListArg_PipedInputIsIgnored()
        {
            new AppRunner<App>()
                .UseSpectreAnsiConsole(new AnsiTestConsole())
                .AppendPipedInputToOperandList()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(App.Single)} single",
                        PipedInput = new[] { "aaa" }
                    },
                    Then =
                    {
                        Output = @"single
"
                    }
                });
        }

        [Fact]
        public void GivenSingleAndListArg_AndNoArgValuesExplicitlyProvided_PipedInputAppendedToListArg()
        {
            new AppRunner<App>()
                .UseSpectreAnsiConsole(new AnsiTestConsole())
                .AppendPipedInputToOperandList()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(App.SingleAndList)} single",
                        PipedInput = new[] { "aaa", "bbb" }
                    },
                    Then =
                    {
                        Output = @"single
aaa,bbb
"
                    }
                });
        }

        [Fact]
        public void GivenStreamingApp_PipedInput_IsOnlyEnumeratedWithinTheCommandMethod()
        {
            var stream = new PipedInputStream("aaa", "bbb");
            new AppRunner<StreamingApp>()
                .UseSpectreAnsiConsole(new AnsiTestConsole())
                .AppendPipedInputToOperandList()
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
                        Output = @"aaa,bbb
"
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
            public void Ansi(IAnsiConsole ansiConsole, string text)
            {
                ansiConsole.WriteLine(text);
            }

            public void Single(IAnsiConsole ansiConsole, [Operand] string singleArg)
            {
                ansiConsole.WriteLine(singleArg);
            }

            public void List(IAnsiConsole ansiConsole, [Operand] List<string> listArgs)
            {
                ansiConsole.WriteLine(listArgs.ToCsv());
            }

            public void SingleAndList(IAnsiConsole ansiConsole, [Operand] string singleArg, [Operand] List<string> listArgs)
            {
                ansiConsole.WriteLine(singleArg);
                ansiConsole.WriteLine(listArgs.ToCsv());
            }
        }

        public class StreamingApp
        {
            public void Stream(IAnsiConsole ansiConsole, IEnumerable<string> input)
            {
                ansiConsole.WriteLine(input.ToCsv());
            }
        }
    }
}