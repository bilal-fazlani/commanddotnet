using System;
using CommandDotNet.TestTools;
using Xunit.Abstractions;

namespace CommandDotNet.Tests
{
    public static class Ambient
    {
        public static ITestOutputHelper? Output
        {
            get => Ambient<ITestOutputHelper>.Instance;
            set => Ambient<ITestOutputHelper>.Instance = value;
        }

        public static Action<string?> WriteLine
        {
            get
            {
                var output = Output;
                if (output == null)
                {
                    throw new InvalidOperationException($"{nameof(Ambient)}.{nameof(Output)} has not been set for the current test");
                }

                return msg =>
                {
                    // XUnit does not like null values
                    msg ??= "";
                    output.WriteLine(msg);
                };
            }
        }
    }
}