using System.Collections.Generic;

namespace CommandDotNet.Tokens
{
    public interface ICommandLineStringSplitter
    {
        // copied from System.CommandLine
        // https://github.com/dotnet/command-line-api/blob/master/src/System.CommandLine/Parsing/ICommandLineStringSplitter.cs

        IEnumerable<string> Split(string commandLine);
    }
}