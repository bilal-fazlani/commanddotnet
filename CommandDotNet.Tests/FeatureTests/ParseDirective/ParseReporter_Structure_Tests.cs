using System.Threading.Tasks;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.ParseDirective
{
    public class ParseReporter_Structure_Tests 
    {
        private readonly ITestOutputHelper _output;

        public ParseReporter_Structure_Tests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Includes_Operands_Options_InheritedOptions_And_Operands_AreCalled_Arguments()
        {
            new AppRunner<App>()
                .UseParseDirective()
                .VerifyScenario(_output, new Scenario
                {
                    WhenArgs = "[parse] Do",
                    Then =
                    {
                        Result = @"command: Do

arguments:

  operand1 <Text>
    value:
    inputs:
    default:

options:

  option1 <Text>
    value:
    inputs:
    default:

  iOption1 <Text>
    value:
    inputs:
    default:

Use [parse:t] to include token transformations."
                    }
                });
        }

        public class App
        {
            public Task<int> Interceptor(InterceptorExecutionDelegate next,
                [Option] string iOption1) => next();

            public void Do([Operand] string operand1, [Option] string option1)
            {

            }
        }
    }
}