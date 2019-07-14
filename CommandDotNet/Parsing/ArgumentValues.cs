using System.Collections.Generic;
using CommandDotNet.Extensions;

namespace CommandDotNet.Parsing
{
    public class ArgumentValues
    {
        private readonly Dictionary<IArgument, List<string>> _valuesByArgument = new Dictionary<IArgument, List<string>>();
        private readonly Dictionary<string, IArgument> _argumentsByAlias = new Dictionary<string, IArgument>();

        public List<string> GetOrAdd(IArgument argument)
        {
            return _valuesByArgument.GetOrAdd(argument, arg =>
            {
                arg.Aliases.ForEach(alias => _argumentsByAlias.Add(alias, arg));
                return new List<string>();
            });
        }

        public bool TryGetValues(IArgument argument, out List<string> values)
        {
            return _valuesByArgument.TryGetValue(argument, out values);
        }

        public bool TryGetValues(string alias, out List<string> values)
        {
            if (_argumentsByAlias.TryGetValue(alias, out var argument))
            {
                return TryGetValues(argument, out values);
            }
            values = null;
            return false;
        }

        public bool Contains(IArgument argument)
        {
            return _valuesByArgument.ContainsKey(argument);
        }

        public bool Contains(string alias)
        {
            return _argumentsByAlias.ContainsKey(alias);
        }
    }
}