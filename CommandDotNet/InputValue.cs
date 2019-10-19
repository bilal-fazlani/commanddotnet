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

        /// <summary>The text values</summary>
        public IEnumerable<string> Values { get; set; }

        public InputValue(string source, IEnumerable<string> values)
        {
            Source = source;
            Values = values;
        }
    }
}