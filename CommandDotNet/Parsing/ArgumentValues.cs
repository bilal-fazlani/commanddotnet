using System.Collections.Generic;
using CommandDotNet.Extensions;

namespace CommandDotNet.Parsing
{
    /// <summary>A collection of string values per argument</summary>
    public class ArgumentValues
    {
        private readonly Dictionary<IArgument, List<string>> _valuesByArgument = new Dictionary<IArgument, List<string>>();
        private readonly Dictionary<string, IArgument> _argumentsByAlias = new Dictionary<string, IArgument>();

        /// <summary>
        /// Returns the values for the given argument.
        /// The list is created if it does not exist.
        /// </summary>
        public List<string> GetOrAdd(IArgument argument)
        {
            return _valuesByArgument.GetOrAdd(argument, arg =>
            {
                arg.Aliases.ForEach(alias => _argumentsByAlias.Add(alias, arg));
                return new List<string>();
            });
        }

        /// <summary>Return the values for the given argument if it exists</summary>
        public bool TryGetValues(IArgument argument, out List<string> values)
        {
            return _valuesByArgument.TryGetValue(argument, out values);
        }

        /// <summary>Return the values for the given argument by alias if it exists</summary>
        public bool TryGetValues(string alias, out List<string> values)
        {
            if (_argumentsByAlias.TryGetValue(alias, out var argument))
            {
                return TryGetValues(argument, out values);
            }
            values = null;
            return false;
        }

        /// <summary>Returns true if values exist for the argument</summary>
        public bool Contains(IArgument argument)
        {
            return _valuesByArgument.ContainsKey(argument);
        }

        /// <summary>Returns true if values exist for the argument</summary>
        public bool Contains(string alias)
        {
            return _argumentsByAlias.ContainsKey(alias);
        }
    }
}