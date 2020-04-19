using System;
using System.Collections;
using System.Reflection;
using CommandDotNet.Extensions;
using CommandDotNet.TestTools.Scenarios;

namespace CommandDotNet.TestTools
{
    internal static class AssertionExtensions
    {
        internal static void ShouldBe(this string actual, string expected, string valueName)
        {
            if (BothAreNull(actual, expected, valueName))
            {
                return;
            }

            if (!expected.Equals(actual))
            {
                Fail(actual, expected, valueName);
            }
        }

        internal static void ShouldBeEquivalentTo(this object actual, object expected, string valueName)
        {
            if (BothAreNull(actual, expected, valueName))
            {
                return;
            }

            if (ReferenceEquals(actual, expected))
            {
                return;
            }

            if (actual is IEnumerable actualList && actual.GetType() != typeof(string) && expected is IEnumerable expectedList)
            {
                int index = 0;
                var expectedEnumerator = expectedList.GetEnumerator();
                foreach (var actualItem in actualList)
                {
                    if (!expectedEnumerator.MoveNext())
                    {
                        throw new AssertFailedException($"Unexpected actual value for {valueName} at index {index}. actual=\"{actualItem}\"");
                    }

                    var expectedItem = expectedEnumerator.Current;
                    actualItem.ShouldBeEquivalentTo(expectedItem, $"{valueName}[{index}]");
                    index++;
                }

                if (expectedEnumerator.MoveNext())
                {
                    var expectedItem = expectedEnumerator.Current;
                    throw new AssertFailedException($"Missing actual value for {valueName} at index {index}. expected=\"{expectedItem}\"");
                }

                return;
            }

            var type = actual.GetType();
            if (type != expected.GetType())
            {
                Fail(type, expected.GetType(), $"{valueName} Type");
            }

            if (type.IsPrimitive || actual is string)
            {
                if (!actual.Equals(expected))
                {
                    Fail(actual, expected, valueName);
                }

                return;
            }

            type
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .ForEach(p =>
                {
                    object actualInstance;
                    object expectedInstance;
                    try
                    {
                        actualInstance = p.GetValue(actual);
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"Failed getting properties for actual: {type}", e);
                    }

                    try
                    {
                        expectedInstance = p.GetValue(expected);
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"Failed getting properties for expected: {type}", e);
                    }
                    ShouldBeEquivalentTo(actualInstance, expectedInstance, $"{valueName}.{p.Name}");
                });
        }

        private static void Fail(object actual, object expected, string valueName)
        {
            throw new AssertFailedException(
                $"expected value for {valueName} to be {Environment.NewLine}\"{expected}\"{Environment.NewLine}but was{Environment.NewLine}\"{actual}\"");
        }

        private static bool BothAreNull(object actual, object expected, string valueName)
        {
            if (expected == null)
            {
                if (actual == null)
                {
                    return true;
                }

                throw new AssertFailedException($"expected value for {valueName} to be null but was {Environment.NewLine}\"{actual}\"");
            }

            return false;
        }
    }
}