using System.Collections;
using System.Collections.Generic;

namespace CommandDotNet.Parsing
{
    public class Tokens : IEnumerable<Token>
    {
        private readonly List<Token> _tokens = new List<Token>();

        public int SeparatorIndex { get; set; } = -1;

        public int Count => _tokens.Count;

        public Tokens(IEnumerable<Token> tokens)
        {
            foreach (var token in tokens)
            {
                if (token.TokenType == TokenType.Separator)
                {
                    SeparatorIndex = _tokens.Count;
                }
                _tokens.Add(token);
            }
        }

        public Token this[int index] => _tokens[index];

        public IEnumerator<Token> GetEnumerator()
        {
            return _tokens.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}