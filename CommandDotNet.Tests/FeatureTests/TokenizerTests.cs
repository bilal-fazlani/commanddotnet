using Xunit;

namespace CommandDotNet.Tests.FeatureTests
{
    public class TokenizerTests
    {
        // Tokenizer Tests
        // - Directives identified, unless feature is disabled
        // - Directives after separator are just values

        // TokenCollection Tests
        // - indexer, count & enumerator work as expected
        // - Directives, Arguments & Separated are populated correctly
        // - Multiple separators work as expected


        [Fact]
        public void SeparatedDirectivesAreNotIncludedInDirectivesList()
        {

        }
    }
}