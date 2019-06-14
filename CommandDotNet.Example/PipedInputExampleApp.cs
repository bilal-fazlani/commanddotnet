using System;
using CommandDotNet.Attributes;

namespace CommandDotNet.Example
{
    [ApplicationMetadata(
        Name = "pipe", 
        Description = "Example of how to take piped data",
        ExtendedHelpText = "example: pipe repeat 10 hello | pipe postfix world\n" +
                           "outputs: helloworld 10 times\n" +
                           "\n" +
                           "example: pipe postfix world\n" +
                           "outputs: No input received")]
    public class PipedInputExampleApp
    {
        [PipedInput(keepEmptyLines:false)] 
        public string[] PipedInput { get; set; }

        public void Repeat(
            [Option(ShortName = "e", LongName = "outputEmptyLines")] 
            bool includeEmptyLines, 
            int count, 
            string text)
        {
            for (int i = 0; i < count; i++)
            {
                Console.Out.WriteLine(text);
                if (includeEmptyLines)
                {
                    Console.Out.WriteLine();
                }
            }
        }

        public void Postfix(string postfix)
        {
            if (PipedInput == null)
            {
                Console.Out.WriteLine("No input received");
                return;
            }

            foreach (var value in PipedInput)
            {
                Console.Out.WriteLine($"{value}{postfix}");
            }
        }
    }
}