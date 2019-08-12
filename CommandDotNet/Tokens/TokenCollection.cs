using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CommandDotNet.Tokens
{
    /// <summary>A collection of tokens parsed from command line arguments</summary>
    public class TokenCollection : IReadOnlyCollection<Token>
    {
        private readonly List<Token> _directives;
        private readonly List<Token> _arguments;
        private readonly List<Token> _separated;
        private readonly List<Token> _combined = new List<Token>();

        /// <summary>The total count of all tokens.  Includes argument separators if they exist.</summary>
        public int Count => _combined.Count;

        /// <summary>The Directives in this collection</summary>
        public IReadOnlyCollection<Token> Directives => _directives.AsReadOnly();
        /// <summary>The Arguments in this collection</summary>
        public IReadOnlyCollection<Token> Arguments => _arguments.AsReadOnly();
        /// <summary>The Arguments located after the first argument separator "--"</summary>
        public IReadOnlyCollection<Token> Separated => _separated.AsReadOnly();

        /// <summary>The indexer for this collection.  Includes argument separators if they exist.</summary>
        public Token this[int index] => _combined[index];

        private TokenCollection(List<Token> directives, List<Token> arguments, List<Token> separated)
        {
            _directives = directives;
            if (directives.Any())
            {
                _combined.AddRange(directives);
            }
            _arguments = arguments;

            if (arguments.Any())
            {
                _combined.AddRange(arguments);
            }

            _separated = separated;
            if (separated.Any())
            {
                _combined.Add(new Token(Tokenizer.SeparatorString, Tokenizer.SeparatorString, TokenType.Separator));
                _combined.AddRange(separated);
            }
        }

        public TokenCollection(IEnumerable<Token> tokens)
        {
            _directives = new List<Token>();
            _arguments = new List<Token>();
            _separated = new List<Token>();

            var directives = _directives;
            var arguments = _arguments;
            foreach (var token in tokens)
            {
                switch (token.TokenType)
                {
                    case TokenType.Directive:
                        directives.Add(token);
                        _combined.Add(token);
                        break;
                    case TokenType.Option:
                        arguments.Add(token);
                        _combined.Add(token);
                        break;
                    case TokenType.Value:
                        arguments.Add(token);
                        _combined.Add(token);
                        break;
                    case TokenType.Separator:
                        if (directives != _separated)
                        {
                            // all additional arguments will be
                            // added to the separated bucket
                            directives = _separated;
                            arguments = _separated;
                        }
                        else
                        {
                            // if multiple separators are provided
                            //   assume nested command execution and 
                            //   retain the additional separators
                            _separated.Add(token);
                        }
                        _combined.Add(token);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>A convenience method making it easier to transform a subset of the tokens by type</summary>
        public TokenCollection Transform(
            Func<Token, IEnumerable<Token>> transformation,
            bool skipDirectives = false,
            bool skipArguments = false,
            bool skipSeparated = false)
        {
            return new TokenCollection(
                skipDirectives ? _directives : _directives.SelectMany(transformation).ToList(),
                skipArguments ? _arguments : _arguments.SelectMany(transformation).ToList(),
                skipSeparated ? _separated : _separated.SelectMany(transformation).ToList()
            );
        }

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