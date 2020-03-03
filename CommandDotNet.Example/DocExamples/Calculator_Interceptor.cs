using System;
using System.Threading.Tasks;

namespace CommandDotNet.Example.DocExamples
{
    public class Calculator_Interceptor
    {
        private bool _printValues;

        public Task<int> Interceptor(InterceptorExecutionDelegate next, bool printValues)
        {
            _printValues = printValues;
            return next();
        }

        [Command(Description = "Adds two numbers. duh!")]
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