using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Extensions;

namespace CommandDotNet.ClassModeling
{
    internal class ValueInfo
    {
        internal bool HasValue => Values.Any();

        internal List<string> Values { get; } = new List<string>();

        internal string Value => Values?.FirstOrDefault();

        public override string ToString()
        {
            return Values.ToCsv(", ");
        }
    }
}