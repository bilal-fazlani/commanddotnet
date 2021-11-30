using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using CommandDotNet.Builders;
using CommandDotNet.Execution;
using CommandDotNet.TestTools;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class DefaultArityTests
    {
        private static readonly ArgumentArity ZeroToOne = new(0, 1);
        private static readonly ArgumentArity OneToOne = new(1, 1);
        private static readonly ArgumentArity ZeroToMany = new(0, int.MaxValue);
        private static readonly ArgumentArity OneToMany = new(1, int.MaxValue);

        public DefaultArityTests(ITestOutputHelper output) => Ambient.Output = output;

        [Fact]
        public void MethodParameters_WithoutDefaults_AreRequired_UnlessNullable()
        {
            var operands = GetOperands(nameof(App.Params));

            operands!.Boolean.Arity.Should().Be(OneToOne);
            operands.NullableBoolean.Arity.Should().Be(ZeroToOne);
            operands.Number.Arity.Should().Be(OneToOne);
            operands.NullableNumber.Arity.Should().Be(ZeroToOne);
            operands.Text.Arity.Should().Be(OneToOne);
            operands.Uri.Arity.Should().Be(OneToOne);
            operands.Texts.Arity.Should().Be(OneToMany);
        }

        [Fact]
        public void MethodParameters_WithDefaults_AreNotRequired()
        {
            var operands = GetOperands(nameof(App.DefaultParams));

            operands!.Boolean.Arity.Should().Be(ZeroToOne);
            operands.NullableBoolean.Arity.Should().Be(ZeroToOne);
            operands.Number.Arity.Should().Be(ZeroToOne);
            operands.NullableNumber.Arity.Should().Be(ZeroToOne);
            operands.Text.Arity.Should().Be(ZeroToOne);
            operands.Uri.Arity.Should().Be(ZeroToOne);
            operands.Texts.Arity.Should().Be(ZeroToMany);
        }

        [Fact]
        public void ModelProperties_WithoutDefaults_AreRequired()
        {
            var operands = GetOperands(nameof(App.Model));

            operands!.Boolean.Arity.Should().Be(OneToOne);
            operands.Number.Arity.Should().Be(OneToOne);
            operands.Text.Arity.Should().Be(OneToOne);
            operands.Uri.Arity.Should().Be(OneToOne);
            operands.Texts.Arity.Should().Be(OneToMany);
        }

        [Fact]
        public void NRT_ModelProperties_WithoutDefaults_AreNotRequired()
        {
            var operands = GetOperands(nameof(App.NrtModel));

            operands!.Boolean.Arity.Should().Be(ZeroToOne);
            operands.Number.Arity.Should().Be(ZeroToOne);
            operands.Text.Arity.Should().Be(ZeroToOne);
            operands.Uri.Arity.Should().Be(ZeroToOne);
            operands.Texts.Arity.Should().Be(ZeroToMany);
        }

        [Fact]
        public void ModelProperties_WithDefaults_OfNonDefaultValues_AreNotRequired()
        {
            var operands = GetOperands(nameof(App.Model), new TestDependencyResolver
            {
                new ArgModel
                {
                    Boolean = true,
                    Number = 1,
                    Text = "lala",
                    Uri = new Uri("http://google.com"),
                    Texts = new[] {"one"}
                }
            });

            operands!.Boolean.Arity.Should().Be(ZeroToOne);
            operands.Number.Arity.Should().Be(ZeroToOne);
            operands.Text.Arity.Should().Be(ZeroToOne);
            operands.Uri.Arity.Should().Be(ZeroToOne);
            operands.Texts.Arity.Should().Be(ZeroToMany);
        }

        [Fact]
        public void NRT_ModelProperties_WithDefaults_OfNonDefaultValues_AreNotRequired()
        {
            var operands = GetOperands(nameof(App.NrtModel), new TestDependencyResolver
            {
                new ArgModel
                {
                    Boolean = true,
                    Number = 1,
                    Text = "lala",
                    Uri = new Uri("http://google.com"),
                    Texts = new[] {"one"}
                }
            });

            operands!.Boolean.Arity.Should().Be(ZeroToOne);
            operands.Number.Arity.Should().Be(ZeroToOne);
            operands.Text.Arity.Should().Be(ZeroToOne);
            operands.Uri.Arity.Should().Be(ZeroToOne);
            operands.Texts.Arity.Should().Be(ZeroToMany);
        }

        [Fact]
        public void ModelProperties_WithDefaults_OfDefaultValues_AreRequired()
        {
            var operands = GetOperands(nameof(App.Model), new TestDependencyResolver
            {
                new ArgModel
                {
                    Boolean = false,
                    Number = 0,
                    Text = null!,
                    Uri = null!,
                    Texts = null!
                }
            });

            operands!.Boolean.Arity.Should().Be(OneToOne);
            operands.Number.Arity.Should().Be(OneToOne);
            operands.Text.Arity.Should().Be(OneToOne);
            operands.Uri.Arity.Should().Be(OneToOne);
            operands.Texts.Arity.Should().Be(OneToMany);
        }

        private Operands? GetOperands(string methodName, IDependencyResolver? dependencyResolver = null)
        {
            var appRunner = new AppRunner<App>();
            if (dependencyResolver != null)
            {
                appRunner.UseDependencyResolver(dependencyResolver);
            }
            return appRunner.GetFromContext(methodName.SplitArgs(),
                    ctx => Operands.FromCommand(ctx.ParseResult!.TargetCommand),
                    middlewareStage: MiddlewareStages.PostParseInputPreBindValues);
        }

        class App
        {
            public void Params(
                bool boolean, bool? nullableBoolean,
                int number, int? nullableNumber,
                string text, Uri uri,
                IEnumerable<string> texts)
            { }

            public void DefaultParams(
                bool boolean = true, bool? nullableBoolean = null,
                int number = 1, int? nullableNumber = null,
                string? text = null, Uri? uri = null,
                IEnumerable<string>? texts = null)
            { }

            public void Model(ArgModel model) { }

            public void NrtModel(NrtArgModel model) { }
        }

        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
        class ArgModel : IArgumentModel
        {
            [Operand]
            public bool Boolean { get; set; }
            [Operand]
            public int Number { get; set; }
            [Operand]
            public string Text { get; set; } = null!;
            [Operand]
            public Uri Uri { get; set; } = null!;
            [Operand]
            public IEnumerable<string> Texts { get; set; } = null!;
        }

        class NrtArgModel : IArgumentModel
        {
            [Operand]
            public bool? Boolean { get; set; }
            [Operand]
            public int? Number { get; set; }
            [Operand]
            public string? Text { get; set; }
            [Operand]
            public Uri? Uri { get; set; }
            [Operand]
            public IEnumerable<string>? Texts { get; set; }
        }

        class Operands
        {
            private static readonly Dictionary<string, PropertyInfo> Properties = typeof(Operands)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);

            public Operand Boolean { get; set; } = null!;
            public Operand Number { get; set; } = null!;
            public Operand Text { get; set; } = null!;
            public Operand Uri { get; set; } = null!;
            public Operand Texts { get; set; } = null!;
            public Operand NullableBoolean { get; set; } = null!;
            public Operand NullableNumber { get; set; } = null!;

            public static Operands FromCommand(Command command)
            {
                var model = new Operands();
                foreach (var operand in command.Operands)
                {
                    var property = Properties[operand.Name];
                    property.SetValue(model, operand);
                }
                return model;
            }
        }
    }
}