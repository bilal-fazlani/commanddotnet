using System;
using NUnit.Framework;

namespace CommandDotNet.DocExamples.Arguments.Arguments
{
    [TestFixture]
    public class Argument_Types
    {
        public class Program
        {
            // begin-snippet: argument_types
            public void Login(IConsole console, Username username, Password password)
            {
                console.WriteLine($"u:{username.Value} p:{password}");
            }
            // end-snippet
        }

        // begin-snippet: argument_types_username
        public class Username
        {
            public string Value { get; }

            public Username(string value) => Value = value;
            public Username(string value, DateTime? validUntil = null) => Value = value;

            public static Username Parse(string value) => new(value);
            public static Username Parse(string value, DateTime? validUntil = null) => new(value, validUntil);
        }
        // end-snippet

        public static BashSnippet Login = new("argument_types_login",
            new AppRunner<Program>(), "myapp.exe", "Login roy rogers", 0,
            @"u:roy p:*****");

        [Test] public void Obligatory_test_since_snippets_cover_all_cases() => Assert.True(true);
    }
}