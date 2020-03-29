using System;
using System.Collections.Generic;
using CommandDotNet.Extensions;
using CommandDotNet.Rendering;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Prompts;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.ParseDirective
{
    public class ParseReporter_InputValues_Tests : IDisposable
    {
        private readonly ITestOutputHelper _output;
        private readonly TempFiles _tempFiles;

        public ParseReporter_InputValues_Tests(ITestOutputHelper output)
        {
            _output = output;
            _tempFiles = new TempFiles(_output.AsLogger());
        }

        public void Dispose()
        {
            _tempFiles.Dispose();
        }

        [Fact]
        public void Inputs_GivenPromptInput_DenotesSourceAs_Prompt_And_ShowsValues()
        {
            new AppRunner<App>()
                .UseParseDirective()
                .UsePrompting(promptForMissingArguments: true)
                .VerifyScenario(_output, new Scenario
                {
                    Given =
                    {
                        OnPrompt = Respond.With(
                            new Answer("opd_stuff", s => s.Contains("opd (Text):")),
                            new Answer(new[] {"one", "two", "three"}, s => !s.Contains("opd (Text):")))
                    },
                    WhenArgs = "[parse] Do",
                    Then =
                    {
                        ResultsContainsTexts =
                        {
                            @"opd <Text>
    value: opd_stuff
    inputs: [prompt] opd_stuff
    default:

  opdList <Text>
    value: one, two, three
    inputs: [prompt] one, two, three
    default:"
                        }
                    }
                });
        }

        [Fact]
        public void Inputs_GivenPipedInput_DenotesSourceAs_PipedStream()
        {
            new AppRunner<App>()
                .UseParseDirective()
                .AppendPipedInputToOperandList()
                .VerifyScenario(_output, new Scenario
                {
                    Given = {PipedInput = new[] {"one, two, three"}},
                    WhenArgs = "[parse] Do opd_stuff",
                    Then =
                    {
                        ResultsContainsTexts =
                        {
                            @"opdList <Text>
    value: one, two, three
    inputs: [piped stream] 
    default:"
                        }
                    }
                });
        }

        [Fact]
        public void Inputs_CanShowMultipleSources()
        {
            new AppRunner<App>()
                .UseParseDirective()
                .AppendPipedInputToOperandList()
                .VerifyScenario(_output, new Scenario
                {
                    Given = { PipedInput = new[] { "one, two, three" } },
                    WhenArgs = "[parse] Do opd_stuff four five six",
                    Then =
                    {
                        ResultsContainsTexts =
                        {
                            @" opdList <Text>
    value: four, five, six, one, two, three
    inputs:
      [argument] four, five, six
      [piped stream] 
    default:"
                        }
                    }
                });
        }

        [Fact]
        public void Inputs_ExpandTokenSourcesRecursively()
        {
            var file = _tempFiles.CreateTempFile("one two three -ab --lala:fishies -l red -l blue -l green");
            new AppRunner<App>()
                .UseResponseFiles()
                .UseParseDirective()
                .VerifyScenario(_output, new Scenario
                {
                    WhenArgs = $"[parse] Do @{file}",
                    Then =
                    {
                        Result = $@"command: Do

arguments:

  opd <Text>
    value: one
    inputs: one (from: @{file} -> one)
    default:

  opdList <Text>
    value: two, three
    inputs: two (from: @{file} -> two), three (from: @{file} -> three)
    default:

options:

  a <Flag>
    value: True
    inputs: true (from: @{file} -> -ab -> -a)
    default: False

  b <Flag>
    value: True
    inputs: true (from: @{file} -> -ab -> -b)
    default: False

  lala <Text>
    value: fishies
    inputs: fishies (from: @{file} -> --lala:fishies -> --lala fishies)
    default:

  l <Text>
    value: red, blue, green
    inputs: red (from: @{file} -> -l red), blue (from: @{file} -> -l blue), green (from: @{file} -> -l green)
    default:

Use [parse:t] to include token transformations."
                    }
                });

        }

        public class App
        {
            public void Do(
                IConsole console,
                [Operand] string opd,
                [Operand] List<string> opdList,
                [Option(ShortName = "a")] bool optA = false,
                [Option(ShortName = "b")] bool optB = false,
                [Option] string lala = null,
                [Option(ShortName = "l")] List<string> optList = null)
            {
                console.Out.WriteLine(new
                {
                    opd, opdList=opdList?.ToCsv(), optA, optB, lala, optList=optList?.ToCsv()
                }.ToString());
            }
        }
    }
}