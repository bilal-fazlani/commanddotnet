using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandDotNet.Execution;

namespace CommandDotNet.Tokens
{
    internal static class ExpandResponseFilesTransformation
    {
        internal static AppRunner UseResponseFiles(AppRunner appRunner)
        {
            return appRunner.Configure(c => c.UseTokenTransformation(nameof(ExpandResponseFile), 1, Transform));
        }

        private static TokenCollection Transform(CommandContext commandContext, TokenCollection tokenCollection)
        {
            return tokenCollection.Transform(ExpandResponseFile, skipDirectives: true, skipSeparated: true);
        }

        private static IEnumerable<Token> ExpandResponseFile(Token token)
        {
            var filePath = GetFilePath(token);
            if (filePath == null)
            {
                yield return token;
            }
            else
            {
                var splitter = CommandLineStringSplitter.Instance;
                var tokens = File
                    .ReadAllLines(filePath)
                    .Where(l => !l.IsCommentOrNullOrWhitespace())
                    .SelectMany(l => splitter.Split(l))
                    .Tokenize(includeDirectives: false);

                foreach (var newToken in tokens)
                {
                    yield return newToken;
                }
            }
        }

        private static string GetFilePath(Token token)
        {
            if (token.TokenType == TokenType.Value && token.Value.StartsWith("@"))
            {
                return token.Value.Substring(1);
            }

            return null;
        }

        private static bool IsCommentOrNullOrWhitespace(this string l)
        {
            return l.IsNullOrWhitespace() || l.StartsWith("#");
        }
    }
}