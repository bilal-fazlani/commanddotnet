using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CommandDotNet.Extensions;
using CommandDotNet.TestTools;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.UnitTests
{
    public class ResourceProxyTests
    {
        private readonly ITestOutputHelper _output;

        private readonly (ResourcesDef source, ResourcesDef proxy)[] _resourcesDefs =
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
        // TODO: release notes
        // TODO: generate json files
        // TODO: generate resx files
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
            var missingMembers = _resourcesDefs
                .SelectMany(r => r.proxy.IsMissingMembersFrom(r.source))
                .ToCsv($"  {Environment.NewLine}");

            missingMembers.Should().BeNullOrEmpty(
                $"... But members are missing from a ResourceProxy. " +
                $"Run {nameof(RegenerateProxyClasses)} to add them");
        }

        [Fact(Skip = "run to regenerate the ResourceProxy classes")]
        //[Fact]
        public void RegenerateProxyClasses()
        {
            var solutionRoot = new DirectoryInfo(Environment.CurrentDirectory).Parent!.Parent!.Parent!.Parent!.FullName;

            var sources = _resourcesDefs.Select(r => r.source);
            var errors = sources.SelectMany(s => s.Validate()).ToList();
            if (errors.Any())
            {
                errors.ForEach(e => _output.WriteLine(
                    $"{e.error}:{Environment.NewLine}" +
                    $"  {e.members.ToCsv($"{Environment.NewLine}  ")}"));
                Assert.True(false);
            }

            var header = "// this file generated by " +
                         $"{GetType().Name}.{nameof(RegenerateProxyClasses)}{Environment.NewLine}";
            sources.ForEach(GenerateFor);
            
            void GenerateFor(ResourcesDef resourcesDef)
            {
                var path = Path.Combine(solutionRoot, resourcesDef.Type.Namespace, "ResourcesProxy.cs");
                //File.Exists(path).Should().BeTrue("class should exist: {0}", path);
                var proxyClass = resourcesDef.GenerateProxyClass();
                
                File.WriteAllText(path, $"{header}{proxyClass}");
            }
        }
    }
}