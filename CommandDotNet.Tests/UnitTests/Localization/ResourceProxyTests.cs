using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandDotNet.Extensions;
using CommandDotNet.TestTools;
using FluentAssertions;
using FluentAssertions.Common;
using Xunit;
using Xunit.Abstractions;
using AssemblyExtensions = CommandDotNet.Tests.Utils.AssemblyExtensions;

namespace CommandDotNet.Tests.UnitTests.Localization
{
    public class ResourceProxyTests
    {
        private readonly ITestOutputHelper _output;

        internal static readonly (ResourcesDef source, ResourcesDef proxy)[] ResourcesDefs =
            AssemblyExtensions.GetAllCommandDotNetAssemblies()
                .SelectMany(a => a.ExportedTypes)
                .Where(t => t.Name == nameof(ResourcesProxy))
                .Select(t => (new ResourcesDef(t.BaseType!), new ResourcesDef(t)))
                .ToArray();

        public static IEnumerable<object[]> ResourceDefsTestData =>
            ResourcesDefs.Select(t => new[] { t.source, t.proxy });

        public ResourceProxyTests(ITestOutputHelper output)
        {
            Ambient.Output = _output = output;
        }
        
        // TODO: public comments on tooling

        [Fact]
        public void ResourceProxy_should_use_result_from_translate_func()
        {
            var proxy = new ResourcesProxy(text =>
            {
                _output.WriteLine(text);
                return "lala";
            });
            proxy.Command_help_description.Should().Be("lala");
            proxy.Parse_Assigning_value_to_argument("a","b").Should().Be("lala");
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
            proxy.Parse_Assigning_value_to_argument("a","b")
                .Should().Be(resources.Parse_Assigning_value_to_argument("a","b"));
        }

        [Theory]
        [MemberData(nameof(ResourceDefsTestData))]
        public void Proxies_should_contain_all_members_of_base(ResourcesDef source, ResourcesDef proxy)
        {
            var missingMembers = proxy.IsMissingMembersFrom(source).ToCsv();

            missingMembers.Should().BeNullOrEmpty(
                "... But members are missing from a ResourceProxy. " +
                $"Run {nameof(ResourceGenerators)}.{nameof(ResourceGenerators.RegenerateProxyClasses)} to add them");
        }

        [Theory]
        [MemberData(nameof(ResourceDefsTestData))]
        public void Assembly_contains_method_to_override_resources(ResourcesDef source, ResourcesDef proxy)
        {
            var assembly = source.Type.Assembly;
            if (assembly.GetName().Name == "CommandDotNet")
            {
                // this assembly using AppSetting.Localize
                return;
            }

            var hasResourceOverride = assembly.ExportedTypes
                .Where(t => t.IsStaticClass())
                .SelectMany(t => t.GetMethods(BindingFlags.Static|BindingFlags.Public)
                    .Where(m =>
                    {
                        var parameters = m.GetParameters();
                        return parameters.Length > 1
                               && parameters.First().ParameterType == typeof(AppRunner)
                               && parameters.Any(p => p.ParameterType.Name == nameof(Resources));
                    }))
                .Any();

            hasResourceOverride.Should().BeTrue(
                $"... Assembly should contain method to override localization Resources. {assembly}");
        }
    }
}