using System;

namespace CommandDotNet
{
    public class AppRunnerException : Exception
    {
        public AppRunnerException(string message) : base(message)
        {

        }
        public AppRunnerException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}