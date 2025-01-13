using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.ClassCommands;

public class InvalidInterceptorSignatureTests
{
    public InvalidInterceptorSignatureTests(ITestOutputHelper output)
    {
        Ambient.Output = output;
    }

    [Fact]
    public void VoidInterceptorThrowsDescriptiveException()
    {
        new AppRunner<VoidInterceptor>().RunInMem("Do")
            .Console.ErrorText().Should()
            .Contain(
                "`CommandDotNet.Tests.FeatureTests.ClassCommands.InvalidInterceptorSignatureTests+VoidInterceptor.Intercept` " +
                "must return type of System.Threading.Tasks.Task`1[System.Int32]");
    }

    class VoidInterceptor
    {
        public void Intercept(InterceptorExecutionDelegate next)
        {
            next().Wait();
        }

        public void Do()
        {
        }
    }
}