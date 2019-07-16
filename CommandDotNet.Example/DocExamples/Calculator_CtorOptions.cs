using System;

namespace CommandDotNet.Example.DocExamples
{
    public class Calculator_CtorOptions
    {
        private readonly bool _printValues;

        public Calculator_CtorOptions(bool printValues)
        {
            _printValues = printValues;
        }

        [ApplicationMetadata(Description = "Adds two numbers. duh!")]
        public void Add(int value1, int value2)
        {
            if (_printValues)
            {
                Console.WriteLine($"value1 : {value1}, value2: {value2}");
            }
            Console.WriteLine($"Answer:  {value1 + value2}");
        }

        public void Subtract(int value1, int value2)
        {
            if (_printValues)
            {
                Console.WriteLine($"value1 : {value1}, value2: {value2}");
            }
            Console.WriteLine($"Answer: {value1 - value2}");
        }
        public void Divide(int value1, int value2 = 1)
        {
            Console.WriteLine($"Answer: {value1 / value2}");
        }
    }
}