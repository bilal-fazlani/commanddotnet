using System.Collections.Generic;

namespace CommandDotNet
{
    /// <summary>The text values provided as input to the application</summary>
    public class InputValue
    {
        /// <summary>
        /// The source of these values<br/>
        /// Sources provided by this framework can be found at <see cref="Constants.InputValueSources"/>
        /// </summary>
        public string Source { get; }

        /// <summary>
        /// True if the value is from a stream and should only be enumerated in within the command.
        /// </summary>
        public bool IsStream { get; }

        /// <summary>The text values</summary>
        public IEnumerable<string> Values { get; set; }

        public InputValue(string source, bool isStream, IEnumerable<string> values)
        {
            Source = source;
            IsStream = isStream;
            Values = values;
        }
    }
}