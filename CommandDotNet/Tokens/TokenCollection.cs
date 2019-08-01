using System;
using System.Collections;
using System.Collections.Generic;

namespace CommandDotNet.Tokens
{
    public class TokenCollection : IReadOnlyCollection<Token>
    {
        private readonly List<Token> _directives = new List<Token>();
        private readonly List<Token> _arguments = new List<Token>();
        private readonly List<Token> _separated = new List<Token>();
        private readonly List<Token> _combined = new List<Token>();

        public int Count => _combined.Count;

        public IReadOnlyCollection<Token> Directives => _directives.AsReadOnly();
        public IReadOnlyCollection<Token> Arguments => _arguments.AsReadOnly();
        public IReadOnlyCollection<Token> Separated => _separated.AsReadOnly();

        public TokenCollection(IEnumerable<Token> tokens)
        {
            var directives = _directives;
            var parsed = _arguments;
            foreach (var token in tokens)
            {
                switch (token.TokenType)
                {
                    case TokenType.Directive:
                        directives.Add(token);
                        _combined.Add(token);
                        break;
                    case TokenType.Option:
                        parsed.Add(token);
                        _combined.Add(token);
                        break;
                    case TokenType.Value:
                        parsed.Add(token);
                        _combined.Add(token);
                        break;
                    case TokenType.Separator:
                        if (directives != _separated)
                        {
                            directives = _separated;
                            parsed = _separated;
                        }
                        else
                        {
                            // if multiple separators are provided
                            //   assume nested command execution and 
                            //   retain the additional separators
                            _separated.Add(token);
                            _combined.Add(token);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public Token this[int index] => _combined[index];

        public IEnumerator<Token> GetEnumerator()
        {
            return _combined.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}