using System;

namespace CommandDotNet.Example.DocExamples
{
    public class Calculator_GettingStarted
    {
        [Command(Description = "Adds two numbers. duh!")]
        public void Add(int value1, int value2)
        {
            Console.WriteLine($"Answer:  {value1 + value2}");
        }

        public void Subtract(int value1, int value2)
        {
            Console.WriteLine($"Answer:  {value1 - value2}");
        }
    }
}