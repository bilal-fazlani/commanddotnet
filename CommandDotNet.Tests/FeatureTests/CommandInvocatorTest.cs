using System;
using System.Linq;
using CommandDotNet.Invocation;
using CommandDotNet.Tests.Utils;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class CommandInvokerTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public CommandInvokerTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void InvokerCanReadAndModifyParamsForCommandMethod()
        {
            void ActionBeforeInvocation(CommandInvocation context)
            {
                context.ParamsForCommandMethod.Length.Should().Be(2);
                var invokedCar = (Car) context.ParamsForCommandMethod[0];
                var invokedOwner = (string) context.ParamsForCommandMethod[1];

                invokedCar.Number.Should().Be(1);
                invokedCar.Number = 2;
                invokedOwner.Should().Be("Jack");
                context.ParamsForCommandMethod[1] = "Jill";
            }

            var result = RunInMem(ActionBeforeInvocation,1, "Jack");

            result.ExitCode.Should().Be(5);
            result.TestOutputs.Get<Car>().Number.Should().Be(2);
            result.TestOutputs.Get<string>().Should().Be("Jill");
        }

        [Fact]
        public void InvokerCanReadArgsFromCli()
        {
            void ActionBeforeInvocation(CommandInvocation context)
            {
                context.ArgsFromCli.Count.Should().Be(2);
                var carNumber = context.ArgsFromCli.First();
                var ownerName = context.ArgsFromCli.Last();

                carNumber.PropertyOrParameterName.Should().Be(nameof(Car.Number));
                carNumber.ValueInfo.Value.Should().Be("1");

                ownerName.PropertyOrParameterName.Should().Be("owner");
                ownerName.ValueInfo.Value.Should().Be("Jack");
            }

            var result = RunInMem(ActionBeforeInvocation, 1, "Jack");
        }

        [Fact]
        public void InvokerCanReadCommandInfo()
        {
            void ActionBeforeInvocation(CommandInvocation context)
            {
                context.CommandInfo.Should().NotBeNull();
                context.CommandInfo.MethodName.Should().Be(nameof(App.NotifyOwner));
            }

            var result = RunInMem(ActionBeforeInvocation, 1, "Jack");
        }

        [Fact]
        public void InvokerCanReadAndActOnInstance()
        {
            var guid = Guid.NewGuid();

            void ActionBeforeInvocation(CommandInvocation context)
            {
                context.Instance.Should().NotBeNull();
                var app = (App) context.Instance;

                app.TestOutputs.Capture(guid);
                context.CommandInfo.MethodName.Should().Be(nameof(App.NotifyOwner));
            }

            var result = RunInMem(ActionBeforeInvocation, 1, "Jack");
            result.TestOutputs.Get<Guid>().Should().Be(guid);
        }

        private AppRunnerResult RunInMem(Action<CommandInvocation> actionBeforeInvocation, int carNumber, string ownerName)
        {
            bool invokerWasCalled = false;

            var result = new AppRunner<App>()
                .WithCommandInvoker(inner => new Invoker(inner, invoker =>
                {
                    invokerWasCalled = true;
                    actionBeforeInvocation(invoker);
                }))
                .RunInMem($"NotifyOwner --Number {carNumber} --owner {ownerName}".SplitArgs(), _testOutputHelper);
            invokerWasCalled.Should().BeTrue();

            return result;
        }

        public class Invoker : ICommandInvoker
        {
            private readonly ICommandInvoker _inner;
            private readonly Action<CommandInvocation> _actionBeforeInvocation;

            public Invoker(ICommandInvoker inner, Action<CommandInvocation> actionBeforeInvocation)
            {
                _inner = inner;
                _actionBeforeInvocation = actionBeforeInvocation;
            }

            public object Invoke(CommandInvocation commandInvocation)
            {
                _actionBeforeInvocation(commandInvocation);
                return _inner.Invoke(commandInvocation);
            }
        }

        public class App
        {
            [InjectProperty]
            public TestOutputs TestOutputs { get; set; }

            public int NotifyOwner(Car car, [Option] string owner)
            {
                TestOutputs.Capture(car);
                TestOutputs.Capture(owner);
                return 5;
            }
        }

        public class Car : IArgumentModel
        {
            [Option]
            public int Number { get; set; }
        }
    }
}
