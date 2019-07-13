using System.Collections.Generic;
using CommandDotNet.ClassModeling;
using CommandDotNet.Extensions;

namespace CommandDotNet.Parsing
{
    public class ArgumentValues
    {
        private readonly Dictionary<IArgument, List<string>> _valuesByArgument = new Dictionary<IArgument, List<string>>();
        private readonly Dictionary<string, IArgument> _argumentsByAlias = new Dictionary<string, IArgument>();

        public List<string> GetArgValues(IArgument argument)
        {
            return _valuesByArgument.GetOrAdd(argument, arg =>
            {
                arg.Aliases.ForEach(alias => _argumentsByAlias.Add(alias, arg));

                // link values w/ ValueInfo until we can remove it completely
                var argInfo = argument.ContextData.Get<ArgumentInfo>();
                return argInfo != null
                    ? argInfo.ValueInfo.Values
                    : new List<string>();
            });
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