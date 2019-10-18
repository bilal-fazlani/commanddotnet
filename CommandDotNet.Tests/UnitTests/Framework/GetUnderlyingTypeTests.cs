using System.Collections.Generic;
using CommandDotNet.Extensions;
using FluentAssertions;
using Xunit;

namespace CommandDotNet.Tests.UnitTests.Framework
{
    public class GetUnderlyingTypeTests
    {
        [Fact]
        public void GivenGenericList_ShouldBe_FirstGenericArgument()
        {
            typeof(List<int>).GetUnderlyingType().Should().Be<int>();
            typeof(List<object>).GetUnderlyingType().Should().Be<object>();
        }

        [Fact]
        public void GivenGenericEnumerable_ShouldBe_FirstGenericArgument()
        {
            typeof(IEnumerable<int>).GetUnderlyingType().Should().Be<int>();
            typeof(IEnumerable<object>).GetUnderlyingType().Should().Be<object>();
        }

        [Fact]
        public void GivenArray_ShouldBe_ElementType()
        {
            typeof(int[]).GetUnderlyingType().Should().Be<int>();
            typeof(object[]).GetUnderlyingType().Should().Be<object>();
        }
    }
}