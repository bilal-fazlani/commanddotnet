using System;
using System.Linq;
using CommandDotNet.Extensions;
using CommandDotNet.TestTools;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.UnitTests.Localization
{
    public class ResourceProxyTests
    {
        private readonly ITestOutputHelper _output;

        internal static readonly (ResourcesDef source, ResourcesDef proxy)[] ResourcesDefs =
        {
            (ResourcesDef.Parse<Resources>(), ResourcesDef.Parse<ResourcesProxy>()),
            (ResourcesDef.Parse<DataAnnotations.Resources>(), ResourcesDef.Parse<DataAnnotations.ResourcesProxy>()),
            (ResourcesDef.Parse<FluentValidation.Resources>(), ResourcesDef.Parse<FluentValidation.ResourcesProxy>())
        };

        public ResourceProxyTests(ITestOutputHelper output)
        {
            _output = output;
        }
        
        // TODO: public comments on tooling
        // TODO: documentation on localization pattern and test tooling
        // TODO: feature list
        // TODO: localize directive

        [Fact]
        public void ResourceProxy_should_use_result_from_translate_func()
        {
            var proxy = new ResourcesProxy(text =>
            {
                _output.WriteLine(text);
                return "lala";
            });
            proxy.Command_help_description.Should().Be("lala");
            proxy.Error_assigning_value_to_argument("a","b").Should().Be("lala");
        }
        
        [Fact]
        public void ResourceProxy_uses_base_when_translate_func_returns_null()
        {
            var proxy = new ResourcesProxy(text =>
            {
                _output.WriteLine(text);
                return null;
            });
            var resources = new Resources();
            proxy.Command_help_description.Should().Be(resources.Command_help_description);
            proxy.Error_assigning_value_to_argument("a","b")
                .Should().Be(resources.Error_assigning_value_to_argument("a","b"));
        }

        [Fact]
        public void ProxyContainsAllMembersOfBase()
        {
            var missingMembers = ResourcesDefs
                .SelectMany(r => r.proxy.IsMissingMembersFrom(r.source))
                .ToCsv($"  {Environment.NewLine}");

            missingMembers.Should().BeNullOrEmpty(
                $"... But members are missing from a ResourceProxy. " +
                $"Run {nameof(ResourceGenerators)}.{nameof(ResourceGenerators.RegenerateProxyClasses)} to add them");
        }
    }
}