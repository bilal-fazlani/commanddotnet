using CommandDotNet.Attributes;

namespace CommandDotNet.Tests.BddTests.Apps
{
    public class SubCommandApp
    {
        [SubCommand]
        public SingleCommandApp Math { get; set; }
    }
}