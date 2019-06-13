using System;
using CommandDotNet.Attributes;
using CommandDotNet.CommandInvoker;
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

        // can read CommandInfo
        // can read ArgsFromCli
        // can read ParamsForCommandMethod
        // can read Instance

        // can modify ParamsForCommandMethod
        // can modify Instance 

        [Fact]
        public void InvokerCanReadAndUpdateInput()
        {
            string carNumber = "DEY-7776";
            string ownerName = "Jack";
            string newOwnerName = "Jill";

            Car invokedCar = null;
            string invokedOwner = null;

            void ActionBeforeInvocation(CommandInvocation context)
            {
                context.ParamsForCommandMethod.Length.Should().Be(2);
                invokedCar = (Car) context.ParamsForCommandMethod[0];
                invokedOwner = (string) context.ParamsForCommandMethod[1];

                context.ParamsForCommandMethod[1] = newOwnerName;
            }

            var result = RunInMem(ActionBeforeInvocation, carNumber, ownerName);

            result.ExitCode.Should().Be(5);
            result.TestOutputs.Get<string>().Should().Be(newOwnerName);

            invokedCar.Number.Should().Be(carNumber);
            invokedOwner.Should().Be(ownerName);
        }

        private AppRunnerResult RunInMem(Action<CommandInvocation> actionBeforeInvocation, string carNumber, string ownerName)
        {
            bool invokerWasCalled = false;

            var result = new AppRunner<App>()
                .WithCommandInvoker(inner => new Invoker(inner, invoker =>
                {
                    invokerWasCalled = true;
                    actionBeforeInvocation(invoker);
                }))
                .RunInMem(new []{ "NotifyOwner", "--Number", carNumber, "--owner", ownerName }, _testOutputHelper);
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
            public string Number { get; set; }
        }
    }
}
