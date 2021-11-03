using System;
using Spectre.Console.Testing;

namespace CommandDotNet.Spectre.Testing
{
    public static class TestConsoleInputExtensions
    {
        public static void PushTextsWithEnter(this TestConsoleInput testConsoleInput, params string[] inputs)
        {
            foreach (var input in inputs)
            {
                testConsoleInput.PushTextWithEnter(input);
            }
        }

        public static void PushCharacters(this TestConsoleInput testConsoleInput, string inputs)
        {
            foreach (var input in inputs)
            {
                testConsoleInput.PushCharacter(input);
            }
        }

        public static void PushKeys(this TestConsoleInput testConsoleInput, params ConsoleKey[] inputs)
        {
            foreach (var input in inputs)
            {
                testConsoleInput.PushKey(input);
            }
        }
    }
}