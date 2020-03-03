using System;

namespace CommandDotNet.Tokens
{
    public class TokenTransformationException : Exception
    {
        public TokenTransformationException(TokenTransformation transformation, Exception innerException) 
            : base($"{transformation}. {innerException.Message}", innerException)
        {
        }
    }
}