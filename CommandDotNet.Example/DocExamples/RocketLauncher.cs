using System;
using System.Collections.Generic;

namespace CommandDotNet.Example.DocExamples
{
    public class RocketLauncher
    {
        public void LaunchRocket(
            [Operand(Name = "planet", Description = "Name of the planet you wish the rocket to go")]
            string planetName,
            [Option('t', "turbo", Description = "Go faster")]
            bool turbo,
            [Option('a', Description = "Abort the launch before takeoff", BooleanMode = BooleanMode.Explicit)] 
            bool abort)
        { }

        public void LaunchBigRocket(
            IEnumerable<string> planets, 
            [Option('c')] string[] crew)
        {
            Console.Out.WriteLine($"planets: {string.Join(",", planets)}");
            Console.Out.WriteLine($"crew: {string.Join(",", crew)}");
        }
    }
}