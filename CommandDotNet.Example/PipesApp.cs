using System;
using System.Collections.Generic;

namespace CommandDotNet.Example
{
    [ApplicationMetadata(Name = "pipes", Description = "example of accepting piped input")]
    public class PipesApp
    {
        public void Echo([Operand(AppendPipedInput = true)] List<string> inputs, [Option] int times = 1)
        {
            if (inputs == null)
            {
                return;
            }

            for (int i = 0; i < times; i++)
            {
                foreach (var input in inputs)
                {
                    Console.Out.WriteLine(input);
                }
            }
        }
    }
}