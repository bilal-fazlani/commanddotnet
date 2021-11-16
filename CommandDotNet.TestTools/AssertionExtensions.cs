using System;
using CommandDotNet.TestTools.Scenarios;
using static System.Environment;

namespace CommandDotNet.TestTools
{
    internal static class AssertionExtensions
    {
        internal static void ShouldBe(this string? actual, string? expected, string valueName)
        {
            if (expected?.Equals(actual) ?? actual == null)
            {
                return;
            }

            Fail(valueName, expected, actual, GetFirstDiffIndex(expected, actual));
        }

        private static int GetFirstDiffIndex(string? expected, string? actual)
        {
            if (expected == null || actual == null) return -1;

            int length = Math.Min(expected.Length, actual.Length);

            for (int i = 0; i < length; i++)
            {
                if (expected[i] != actual[i])
                {
                    return i;
                }
            }

            return length;
        }

        private static void Fail(string valueName, string? expected, string? actual, int diffIndex)
        {
            var startIndex = Math.Max(0, diffIndex-10);
            var actualSnippet = actual?.Substring(startIndex, Math.Min(actual.Length - startIndex, diffIndex + 10));
            var expectedSnippet = expected?.Substring(startIndex, Math.Min(expected.Length - startIndex, diffIndex + 10));

            throw new AssertFailedException(
                $"Mismatch at index {diffIndex}{NewLine}" +
                $"expected:{expectedSnippet}{NewLine}" +
                $"actual  :{actualSnippet}{NewLine}" +
                $"Expected value for {valueName} to be {FormatMessage(expected)} " +
                $"but was {FormatMessage(actual)}");
        }

        private static string FormatMessage(object? item) => item == null ? "<null>" : $"{NewLine}\"{item}\"{NewLine}";
    }
}