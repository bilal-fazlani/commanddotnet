using System.Collections.Generic;

namespace CommandDotNet.Example.DocExamples
{
    public class RocketLauncher
    {
        public void LaunchRocket1([Operand(
            Name = "planet",
            Description = "Name of the planet you wish the rocket to go")] string planetName)
        { }

        public void LaunchRocket2([Option(
            Name = "planet",
            ShortName = "p",
            Description = "Name of the planet you wish the rocket to go")] string planetName)
        { }

        public void LaunchRocket3([Option(ShortName = "p")] List<string> planets)
        { }

        public void LaunchRocket4(List<string> planets)
        { }
    }
}