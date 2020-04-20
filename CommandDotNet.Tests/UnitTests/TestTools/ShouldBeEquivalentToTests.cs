using System.Collections.Generic;
using Xunit;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using FluentAssertions;

namespace CommandDotNet.Tests.UnitTests.TestTools
{
    public class ShouldBeEquivalentToTests
    {
        [Fact]
        public void Given_Equal_Strings_Approves()
        {
            "lala".ShouldBeEquivalentTo("lala", "string");
        }

        [Fact]
        public void Given_Unequal_Strings_Throws()
        {
            Assert.Throws<AssertFailedException>(() =>
                    "lala".ShouldBeEquivalentTo("blahblah", "string"))
                .Message.Should().Be(@"expected value for string to be 
""blahblah""
but was
""lala""");
        }

        [Fact]
        public void Given_Equal_IEnumerables_Approves()
        {
            new []{"one","two","three"}.ShouldBeEquivalentTo(new List<string> { "one", "two", "three" }, "Enumerable");
        }

        [Fact]
        public void Given_Unequal_IEnumerables_DifferentItems_Throws()
        {
            Assert.Throws<AssertFailedException>(() =>
                new[] { "one", "two", "four" }.ShouldBeEquivalentTo(new List<string> { "one", "two", "three" }, "Enumerable"))
                .Message.Should().Be(@"expected value for Enumerable[2] to be 
""three""
but was
""four""");
        }

        [Fact]
        public void Given_Unequal_IEnumerables_MissingItem_Throws()
        {
            Assert.Throws<AssertFailedException>(() =>
                    new[] { "one", "two", "three" }.ShouldBeEquivalentTo(new List<string> { "one", "two", "three", "four" }, "Enumerable"))
                .Message.Should().Be(@"Missing actual value for Enumerable at index 3. expected=""four""");
        }

        [Fact]
        public void Given_Unequal_IEnumerables_UnexpectedItem_Throws()
        {
            Assert.Throws<AssertFailedException>(() =>
                    new[] { "one", "two", "three", "four" }.ShouldBeEquivalentTo(new List<string> { "one", "two", "three" }, "Enumerable"))
                .Message.Should().Be(@"Unexpected actual value for Enumerable at index 3. actual=""four""");
        }

        [Fact]
        public void Given_Equal_Objects_Approves()
        {
            var meal1 = new Meal {Drink = "water", Food = "celery", NextMeal = new Meal {Drink = "coke", Food = "crisps"}};
            var meal2 = new Meal { Drink = "water", Food = "celery", NextMeal = new Meal { Drink = "coke", Food = "crisps" } };
            meal1.ShouldBeEquivalentTo(meal2, nameof(Meal));
        }

        [Fact]
        public void Given_Unequal_Objects_Throws()
        {
            var meal1 = new Meal { Drink = "water", Food = "celery", NextMeal = new Meal { Drink = "coke", Food = "crisps" } };
            var meal2 = new Meal { Drink = "water", Food = "celery", NextMeal = new Meal { Drink = "coke", Food = "fries" } };
            Assert.Throws<AssertFailedException>(() =>
                    meal1.ShouldBeEquivalentTo(meal2, nameof(Meal)))
                .Message.Should().Be(@"expected value for Meal.NextMeal.Food to be 
""fries""
but was
""crisps""");
        }

        [Fact]
        public void Honors_Equals_Override()
        {
            var meal1 = new Meal { Drink = "water", Food = "celery", NextMeal = new Meal { Drink = "coke", Food = "crisps" } };
            new EqualsAnything().ShouldBeEquivalentTo(meal1, nameof(Meal));
        }

        public class Meal
        {
            public string Drink { get; set; }
            public string Food { get; set; }
            public Meal NextMeal { get; set; }
        }

        public class EqualsAnything
        {
            public override bool Equals(object obj)
            {
                return true;
            }
        }
    }
}