using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class Operands_PipedInput_Tests : AppendPipedInputTestsBase
    {
        public Operands_PipedInput_Tests(ITestOutputHelper output) : base(output)
        {
        }

        protected override AppRunner AppRunner<T>() where T : class =>
            new AppRunner<T>();
    }
}