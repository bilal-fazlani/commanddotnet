using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandDotNet.Parsing
{
    /// <summary>
    /// Use this class to get input piped from the console.
    /// Empty entries are removed.
    /// </summary>
    public class PipedInput
    {
        public bool InputWasPiped { get; }
        public IEnumerable<string> Values { get; }

        /// <inheritdoc />
        private PipedInput(string[] lines)
        {
            Values = lines;
            
            // this will only be null when Console.IsInputRedirected == false
            InputWasPiped = lines != null;
        }

        public static PipedInput GetPipedInput(bool keepEmptyLines = false)
        {
            if (Console.IsInputRedirected)
            {
                var input = Console.In.ReadToEnd()
                    .Split(
                        new[] {"\r\n", "\r", "\n"},
                        keepEmptyLines
                            ? StringSplitOptions.None
                            : StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .ToArray();
                
                return new PipedInput(input);
            }

            return new PipedInput(null);
        }
    }
}