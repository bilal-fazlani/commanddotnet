using CommandDotNet.NameCasing;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.CommandDotNet.NameCasing;

public class CaseChangerTests
{
    public CaseChangerTests(ITestOutputHelper output)
    {
        Ambient.Output = output;
    }

    [Fact]
    public void Works()
    {
        new AppRunner<App>()
            .UseNameCasing(Case.KebabCase)
            .Verify(new Scenario
            {
                When = {Args = "go"},
                Then = {Output = "case-changer-tests"}
            });
    }
        
    public class App
    {
        public void Go(CommandContext context, IConsole console)
        { 
            console.Write(context.Services.GetOrThrow<CaseChanger>().ChangeCase(nameof(CaseChangerTests)));
        }
    }
}