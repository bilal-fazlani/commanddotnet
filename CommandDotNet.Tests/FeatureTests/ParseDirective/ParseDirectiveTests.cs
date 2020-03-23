using System;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.ParseDirective
{
    public class ParseDirectiveTests : IDisposable
    {
        // ReSharper disable InconsistentNaming
        private const int ExitCode_MethodNotCalled = 0;
        private const int ExitCode_MethodCalled = 0;
        // ReSharper restore InconsistentNaming

        private readonly ITestOutputHelper _output;
        private readonly TempFiles _tempFiles;

        public ParseDirectiveTests(ITestOutputHelper output)
        {
            _output = output;
            _tempFiles = new TempFiles(_output.AsLogger());
        }

        public void Dispose()
        {
            _tempFiles.Dispose();
        }

        #region Test Cases
        /*
        - structure
          - command
            - no args
            - no operands
            - no options
            - args
            - subcommand
          - arguments:operands,options,inherited options
        - values  
          - value:single,multi,Password
          - inputs
            - source: arg,pipe,prompt,multi
            - transformations: clubbing,response-file
            - value: single,multi,Password
          - defaults
            - source: app,app-setting
            - value: single,multi,Password
          - passwords not exposed
            - when arg
            - when prompt
            - when rsp file
            
        - [parse:t]
          - when requested
          - when not requested but error caused other report not to trigger
            - parse error
            - exception
        */
        #endregion

        [Fact]
        public void Should_EchoAllArgs_OnNewLine_WithIndent()
        {
            new AppRunner<App>()
                .UseParseDirective()
                .VerifyScenario(_output,
                    new Scenario
                    {
                        WhenArgs = "[parse:t] Some -ab args to echo",
                        Then =
                        {
                            ExitCode = ExitCode_MethodNotCalled,
                            Result = @"command: Some

arguments:

  args <Text>
    value: args
    inputs: args
    default:

  to <Text>
    value: to
    inputs: to
    default:

  echo <Text>
    value: echo
    inputs: echo
    default:

options:

  a <Flag>
    value: True
    inputs: true (from: -ab -> -a)
    default:

  b <Flag>
    value: True
    inputs: true (from: -ab -> -b)
    default:

  v <Text>
    value:
    inputs:
    default:


token transformations:

>>> from shell
  Directive: [parse:t]
  Value    : Some
  Option   : -ab
  Value    : args
  Value    : to
  Value    : echo
>>> after: expand-clubbed-flags
  Directive: [parse:t]
  Value    : Some
  Option   : -a
  Option   : -b
  Value    : args
  Value    : to
  Value    : echo
>>> after: split-option-assignments (no changes)"
                        }
                    });
        }

        [Fact]
        public void Should_SpecifyWhenTransformation_DoesNotMakeChanges()
        {
            new AppRunner<App>()
                .UseParseDirective()
                .VerifyScenario(_output,
                    new Scenario
                    {
                        WhenArgs = "[parse:t] Some args to echo",
                        Then =
                        {
                            ExitCode = ExitCode_MethodNotCalled,
                            Result = @"command: Some

arguments:

  args <Text>
    value: args
    inputs: args
    default:

  to <Text>
    value: to
    inputs: to
    default:

  echo <Text>
    value: echo
    inputs: echo
    default:

options:

  a <Flag>
    value:
    inputs:
    default:

  b <Flag>
    value:
    inputs:
    default:

  v <Text>
    value:
    inputs:
    default:


token transformations:

>>> from shell
  Directive: [parse:t]
  Value    : Some
  Value    : args
  Value    : to
  Value    : echo
>>> after: expand-clubbed-flags (no changes)
>>> after: split-option-assignments (no changes)"
                        }
                    });
        }

        class App
        {
            public int Some(
                [Option(ShortName = "a")] bool opt1,
                [Option(ShortName = "b")] bool opt2,
                [Option(ShortName = "v")] string optionValue,
                string args, string to, string echo)
            {
                return ExitCode_MethodCalled;
            }

            public int Secure(
                [Option] string username,
                [Option] Password password)
            {
                return ExitCode_MethodCalled;
            }

            public int NoArgs()
            {
                return ExitCode_MethodCalled;
            }
        }
    }
}