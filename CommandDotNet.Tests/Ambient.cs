using System.Threading;
using Xunit.Abstractions;

namespace CommandDotNet.Tests
{
    public static class Ambient
    {
        private static readonly AsyncLocal<ITestOutputHelper> TestOutputHelper = new AsyncLocal<ITestOutputHelper>();

        public static ITestOutputHelper Output
        {
            get => TestOutputHelper.Value;
            set => TestOutputHelper.Value = value;
        }
    }
}